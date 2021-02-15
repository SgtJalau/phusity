﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions;
using Cinemachine;
using UnityEngine.InputSystem;

public class ThirdPersonMovement : MonoBehaviour
{
    [SerializeField, Tooltip("The movement speed of the player")]
    public float speed = 6f;
    [SerializeField, Tooltip("The movement speed of the player in the air")]
    public float speedInAir = 3f;
    [SerializeField, Tooltip("Changing direction greater than this angle will cancel active freefall")]
    public float freeFallAngleLimit = 45f;

    public float turnSmoothTime = 0.1f;
    public float gravityMultiplyer = 1.0f;

    //-------- JUMP VARIABLES ------------//
    [Header("Jump")]
    public float jumpHeight = 6.0f;
    private bool canJumpGround = false; //TODO: not sure if this is needed, since canJumpGround = isGrounded atm
    private bool canJumpAir = false;

    //-------- DASH VARIABLES ------------//
    [Header("Dash")]
    [SerializeField, Tooltip("The movement speed that is applied if the player dashes")]
    public float dashDistance = 2f;
    [SerializeField, Tooltip("The time it takes to complete the Dash (if uninterrupted")]
    public float dashDuration = 0.2f;
    private float dashSpeed = 0.0f;
    [SerializeField, Tooltip("Time the dash needs to recharge")]
    public float dashCooldown = 1.0f;
    private float timeSinceDash = 10.0f; //some high number, dash possible by default
    private bool canDash = true;
    private bool isDashing = false;
    private float timeInDash = 0.0f;
    private bool touchedGroundSinceLastDash = true; //in case we only touch ground for a moment after dashing, the dash should be reset anyways

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.5f;

    [Header("Stair Handling")]
    public float maxStepHeight = 0.25f;
    public int stairDetail = 10;
    public LayerMask stepMask;

    [Header("Coins")]
    public int points = 0;



    /**
     * If gliding is enabled we can press a key to start gliding
     */
    private bool _glidingEnabled = false;

    private CinemachineBrain _virtualCamera;

    public bool GlidingEnabled
    {
        get => _glidingEnabled;
        set => _glidingEnabled = value;
    }

    private Rigidbody playerRigidbody = null;
    private Transform camTransform = null;

    private bool _gliding;

    private float turnSmoothVelocity;
    private Vector3 velocity;
    private bool isGrounded;
    private Vector3 direction;
    private float currentSpeed;

    //--------- AIR MOVEMENT VARIABLES ---------//
    private bool isInFreeFall = false;
    private bool wasGrounded = false;
    private Vector3 freeFallVelocity = Vector3.zero;

    private float stepSoundTimer;


    private GameStateHandler _gameStateHandler;

    private AudioManager _audioManager;
    private InputMaster _input;

    private List<HighlightableActivator> _highlighting;

    private void Awake()
    {
        _input = new InputMaster();
        _input.Gameplay.QuickLoad.performed += _ => LoadGameState();
        _input.Gameplay.QuickSave.performed += _ => SaveGameState();
        _input.Player.Jump.performed += _ => Jump();
        _input.Player.Dash.performed += _ => Dash();
        _input.Player.Glide.performed += _ => Glide();

        _audioManager = FindObjectOfType<AudioManager>();

        _highlighting = new List<HighlightableActivator>();
    }

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        camTransform = Camera.main.transform;

        velocity = new Vector3(0f, 0f, 0f);
        direction = new Vector3(0f, 0f, 0f);
        isGrounded = false;
        currentSpeed = speed;

        dashSpeed = dashDistance / dashDuration;

        _gameStateHandler = new GameStateHandler();

        _virtualCamera = Camera.main.GetComponent<CinemachineBrain>();
        Assert.IsNotNull(_virtualCamera);

        //Deactivate gravity since we handle it here in our own way
        playerRigidbody.useGravity = false;
    }

    void OnDrawGizmos()
    {
        Handles.Label(transform.position + Vector3.up, "Grounded: "+isGrounded);
        Handles.Label(transform.position + Vector3.up * 0.8f, "In Free Fall: " + isInFreeFall);
        if(playerRigidbody != null)
            Handles.Label(transform.position + Vector3.up * 0.65f, "Horizontal Speed: " + new Vector2(playerRigidbody.velocity.x, playerRigidbody.velocity.z).magnitude);
    }

    void Update() //not sure why this is done in update()? maybe FixedUpdate also enough
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit); //just Vector3.down enough? Player cant rotate
        const double hitDistance = 0.85;
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * (float)hitDistance,Color.red);

        //TODO: this kind of sucks, doable with OnCollisionEnter and normal comparison?
        if (hit.distance > hitDistance || !hit.collider || hit.collider.isTrigger)
        {
            isGrounded = false;
            if (wasGrounded)
            { 
                isInFreeFall = true;
            }
        }
        else if (hit.distance <= hitDistance)
        {
            canJumpGround = true;
            canJumpAir = true; //TODO: could lead to errors: jump directly after pressing jump but still close enough to ground -> triple jump
            isGrounded = true;
            touchedGroundSinceLastDash = true;
            _gliding = false;

            //Keep the last velocity saved in case we leave the ground
            isInFreeFall = false;
            freeFallVelocity = playerRigidbody.velocity;

            //Check if we are moving on ground and play sound if we are
            if (playerRigidbody.velocity.magnitude >= 0.1F)
            {
                if (stepSoundTimer <= 0)
                {
                    stepSoundTimer = 0.4f;

                    Material material = GetMaterialAtHit(hit);

                    if (material != null)
                    {
                        if (material.name == ("Mat_Gestein (Instance)") || material.name == ("Mat_Felsen (Instance)") ||
                            material.name == "Mat_Bruecke (Instance)" || material.name == "Mat_Ruine (Instance)")
                        {
                            _audioManager.Play(SoundType.StepRock);
                        }
                        else
                        {
                            _audioManager.Play(SoundType.StepGras);
                        }
                    }
                    else
                    {
                        _audioManager.Play(SoundType.StepGras);
                    }
                }
                else
                {
                    stepSoundTimer -= Time.deltaTime;
                }
            }
        }
        wasGrounded = isGrounded;
    }

    void FixedUpdate()
    {
        Collider target;
        bool targetFound = GetTargetCollider(5, out target);

        HighlightableActivator activator = null;
        if (targetFound)
        {
            if (target.TryGetComponent(out activator))
            {
                //Interact
                if (Input.GetKey(KeyCode.F)) //TODO: new InputSystem
                {
                    activator.activate();
                } else if (!_highlighting.Contains(activator))
                {
                    activator.Highlight();
                }
            }
            else
            {
                //We did not find anything we want to highlight
                targetFound = false;
            }
        }

        //Remove all highlights that aren't the current highlighted
        foreach (HighlightableActivator highlightable in _highlighting)
        {
            if (highlightable != activator)
            {
                highlightable.Restore();
                Debug.Log("Removed Highlighting");
            }
        }

        //Clear list
        _highlighting.Clear();

        //Add back highlight target if we found some
        if (targetFound)
        {
            _highlighting.Add(activator);
        }

        if (isDashing)
        {
            timeInDash += Time.fixedDeltaTime;
            if (timeInDash >= dashDuration
                || playerRigidbody.velocity.magnitude < dashSpeed)
            {
                isDashing = false;
                timeSinceDash = 0.0f;
                canDash = false;
                isInFreeFall = true;
                playerRigidbody.velocity = playerRigidbody.velocity.normalized * speed; //not sure if we want to use speedInAir here, handle this the same as starting free fall from a jump
                freeFallVelocity = playerRigidbody.velocity; 
            }   
        }
        else //unsure if else is enough, maybe test if !isDashing so dash can be cancelled and normal physics calculated in the same timestep
        {
            timeSinceDash += Time.fixedDeltaTime;
            if (timeSinceDash >= dashCooldown && touchedGroundSinceLastDash)
            {
                canDash = true;
            }

            velocity.y = playerRigidbody.velocity.y;

            float gravity = gravityMultiplyer;

            if (_gliding)
            {
                gravity /= 3;
            }

            playerRigidbody.AddForce(Physics.gravity * gravity, ForceMode.Acceleration);

            // Player movement is handeled here
            Vector2 move = _input.Player.Movement.ReadValue<Vector2>();
            direction = new Vector3(move.x, 0f, move.y).normalized; //TODO: normalizing breaks controller input
            Vector3 moveDir = new Vector3(0f, 0f, 0f);

            //Check if player movement should be applied
            if (direction.magnitude >= 0.1F)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity,
                    turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                moveDir = (Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward).normalized;
                float deltaAngle = Vector3.Angle(moveDir, new Vector3(freeFallVelocity.x,0,freeFallVelocity.z));
                //Debug.DrawRay(transform.position, moveDir, Color.red);
                //Debug.DrawRay(transform.position, new Vector3(freeFallVelocity.x, 0, freeFallVelocity.z), Color.blue);
                                                                                          //player couldve entered freeFall by jumping straight up
                bool overwriteFreeFall = isInFreeFall && (deltaAngle < freeFallAngleLimit && freeFallVelocity.xz().magnitude > 0.1f);
                float maxSpeed = (isGrounded || overwriteFreeFall) ? speed : speedInAir;
                moveDir *= maxSpeed;


                var velocityChange = (moveDir - playerRigidbody.velocity);

                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxSpeed, maxSpeed);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxSpeed, maxSpeed);
                velocityChange.y = 0;


                ////stair handling
                //bool isFirstCheck = false;
                //bool canMove = true;
                //for (int i = stairDetail; i >= 1; i--)
                //{
                //    Collider[] c = Physics.OverlapBox(
                //        transform.position - new Vector3(0, i * maxStepHeight / stairDetail, 0),
                //        new Vector3(1.05f, maxStepHeight / stairDetail / 2, 1.05f), Quaternion.identity, stepMask);
                //    if (new Vector2(velocityChange.x, velocityChange.z) != Vector2.zero)
                //    {
                //        if (c.Length > 0 && i == stairDetail)
                //        {
                //            isFirstCheck = true;
                //            if (!isGrounded)
                //            {
                //                canMove = false;
                //            }
                //        }

                //        if (c.Length > 0 && !isFirstCheck)
                //        {
                //            transform.position += new Vector3(0, i * maxStepHeight / stairDetail, 0);
                //            break;
                //        }
                //    }
                //}

                ////TODO: what does this actually do? -> needed in dash branch? -yes-> move outside of if-else
                //// handeling walls
                //RaycastHit hitWall;
                //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 5f, Color.red);
                //if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitWall))
                //{
                //    if (hitWall.distance < 1.0f && !isGrounded)
                //    {
                //        canMove = false;
                //        Debug.Log("Can Move " + canMove);
                //    }
                //}

                //if (canMove)
                //{
                //    playerRigidbody.velocity += velocityChange;
                //}

                //if (!isInFreeFall || actualVelocityChange > 0.5f )
                ////if (!isInFreeFall || (actualSpeedChange > 0.5f || deltaAngle > 5))
                //{
                //    playerRigidbody.velocity += velocityChange;
                //    isInFreeFall = false;
                //}
                playerRigidbody.velocity += velocityChange;
                if (overwriteFreeFall)
                {
                    //stay in free fall, maybe change direction a tiny bit
                    freeFallVelocity = playerRigidbody.velocity;
                }
                else
                {
                    isInFreeFall = false;
                }
            }
            else
            {
                if (isInFreeFall)
                {
                    playerRigidbody.velocity = new Vector3(freeFallVelocity.x, playerRigidbody.velocity.y, freeFallVelocity.z);
                }
                else
                { 
                    playerRigidbody.velocity = new Vector3(0, playerRigidbody.velocity.y, 0);
                }
            }
        }
    }

    void LoadGameState()
    {
        _gameStateHandler.QuickLoadGameState();
        Debug.Log("Quick loaded state");
    }

    void SaveGameState()
    {
        _gameStateHandler.SaveGameState();
        Debug.Log("Quick saved state");
    }

    void Jump()
    {
        if (isGrounded && canJumpGround)
        {
            performJump();
            isGrounded = false;
            canJumpGround = false; 
            //these two probably not doing anything tbh. player is still too close to the ground
            //and in the next update the player will be set to isGrounded again, still: figure out better solution than timeout
        }
        else if (!isGrounded && canJumpAir)
        {
            performJump();
            canJumpAir = false;
            isInFreeFall = true;
            freeFallVelocity = playerRigidbody.velocity;
            _audioManager.Play(SoundType.DoubleJump);
        }
    }

    void performJump()
    {
        playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x,
                Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y * gravityMultiplyer), playerRigidbody.velocity.z);
    }

    void Dash()
    {
        if (canDash)
        { 
            //TODO: Maybe dont just dash forward. If on ground: Dash along slope so dash doesnt instantly get cancelled
            playerRigidbody.velocity = transform.forward * dashSpeed;
            isDashing = true;
            timeInDash = 0.0f;
            canDash = false;
            touchedGroundSinceLastDash = false;
            _audioManager.Play(SoundType.PlayerDash);
        }
    }

    void Glide()
    {
        if (_glidingEnabled && direction.y <= 0)
        {
            _gliding = true;
            _glidingEnabled = false;
            Debug.Log("Gliding enabled");
        }
    }


    void OnGUI() //Coin Gui
    {
        GUI.Label(new Rect(10, 10, 100, 20), "Score : " + points);
    }
    public IEnumerator LookAtLocation(Transform vector3, long millis)
    {
        Transform previousFollow = _virtualCamera.ActiveVirtualCamera.Follow;
        Transform previousLookAt = _virtualCamera.ActiveVirtualCamera.LookAt;

        //_virtualCamera.OutputCamera.transform.LookAt(vector3);
        _virtualCamera.ActiveVirtualCamera.Follow = vector3;
        _virtualCamera.ActiveVirtualCamera.LookAt = vector3;


        yield return new WaitForSeconds(millis / 1000F);

        _virtualCamera.ActiveVirtualCamera.Follow = previousFollow;
        _virtualCamera.ActiveVirtualCamera.LookAt = previousLookAt;
    }

    private Material GetMaterialAtHit(RaycastHit hit)
    {
        if (hit.collider.material)
        {
            if (hit.collider.gameObject.TryGetComponent(out MeshFilter mesh) && mesh.mesh.isReadable)
            {
                var index = hit.triangleIndex;
                var count = mesh.mesh.subMeshCount;

                //Debug.Log(index + " | " + count);

                var meshRenderer = hit.collider.GetComponent<MeshRenderer>();
                for (var x = 0; x < count; x++)
                {
                    var triangles = mesh.mesh.GetTriangles(x);

                    for (var y = 0; y < triangles.Length; y++)
                    {
                        if (triangles[y] == index)
                        {
                            //Debug.Log(triangles[y] + " | " + y + " | Mat Index: " + x + " | " +  hit.collider.GetComponent<MeshRenderer>().materials[x + 1]);
                            var materials = meshRenderer.materials;

                            //No idea why the materials are shifted by one but for some reason they are
                            var matIndex = x + 1 < materials.Length ? x + 1 : materials.Length - 1;

                            return materials[matIndex];
                        }
                    }
                }
            }

            //Debug.Log(hit.collider.gameObject.GetComponent<MeshRenderer>().materials.Length + " test");
        }

        return null;
    }

    public bool GetTargetCollider(float maxDistance, out Collider collider)
    {
        RaycastHit hit;

        var raysToCheck = new List<Vector3>
        {
            transform.TransformDirection(Vector3.forward),
            transform.TransformDirection(Vector3.forward * 3 + Vector3.up),
            transform.TransformDirection(Vector3.forward * 3 + Vector3.down),
            transform.TransformDirection(Vector3.forward * 3 + Vector3.left),
            transform.TransformDirection(Vector3.forward * 3 + Vector3.right),
            transform.TransformDirection(Vector3.forward * 3 + Vector3.down * 2),
            transform.TransformDirection(Vector3.forward * 3 + Vector3.down + Vector3.left * 2),
            transform.TransformDirection(Vector3.forward * 3 + Vector3.down + Vector3.right * 2)
        };

        foreach (Vector3 rayDirection in raysToCheck)
        {
            Debug.DrawRay(transform.position, rayDirection, Color.green);

            if (Physics.Raycast(transform.position, rayDirection, out hit, maxDistance,
                LayerMask.GetMask("Targetable")))
            {
                collider = hit.collider;
                return true;
            }
        }

        collider = null;
        return false;
    }

    void OnEnable()
    {
        _input.Player.Enable();
        _input.Gameplay.Enable();
    }

    void OnDisable()
    {
        _input.Player.Disable();
        _input.Gameplay.Disable();
    }
}