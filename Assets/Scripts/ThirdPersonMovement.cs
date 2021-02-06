using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class ThirdPersonMovement : MonoBehaviour
{
    public Rigidbody rb;
    public Transform camTransform;

    [SerializeField, Tooltip("The movement speed of the player")]
    public float speed = 6f;

    [SerializeField, Tooltip("The movement speed of the player in the air")]
    public float airMovementSpeed = 1f;

    public float turnSmoothTime = 0.1f;
    public float gravityMultiplyer = 1.0f;
    public float jumpHeight = 6.0f;

    [SerializeField, Tooltip("The movement speed that is applied if the player dashes")]
    public float dashSpeed = 20f;

    public Transform groundCheck;
    public float groundDistance = 0.5f;


    public float maxStepHeight = 0.25f;
    public int stairDetail = 10;
    public LayerMask stepMask;

    /**
     * If gliding is enabled we can press a key to start gliding
     */
    private bool _glidingEnabled = false;

    public CinemachineBrain _virtualCamera;

    public bool GlidingEnabled
    {
        get => _glidingEnabled;
        set => _glidingEnabled = value;
    }

    private bool _gliding;

    private bool jump;
    private bool doubleJump;
    private bool dash;
    private float turnSmoothVelocity;
    private Vector3 velocity;
    private bool isGrounded;
    private Vector3 direction;
    private float currentSpeed;
    private float doublejumpTimeout;

    private float stepSoundTimer;


    private GameStateHandler _gameStateHandler;
    private float lastDash;

    private AudioManager _audioManager;
    private InputMaster _input;

    private void Awake()
    {
        _input = new InputMaster();
        _input.Gameplay.QuickLoad.performed += _ => LoadGameState();
        _input.Gameplay.QuickSave.performed += _ => SaveGameState();
        _input.Player.Jump.performed += _ => Jump();
        _input.Player.Dash.performed += _ => Dash();
        _input.Player.Glide.performed += _ => Glide();

        _audioManager = FindObjectOfType<AudioManager>();
    }

    void Start()
    {
        velocity = new Vector3(0f, 0f, 0f);
        direction = new Vector3(0f, 0f, 0f);
        jump = true;
        doubleJump = false;
        dash = true;
        isGrounded = false;
        lastDash = Time.realtimeSinceStartup;
        currentSpeed = speed;
        doublejumpTimeout = 0.3f;

        _gameStateHandler = new GameStateHandler();

        //Deactivate gravity since we handle it here in our own way
        rb.useGravity = false;
    }

    void Update()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit);

        const double hitDistance = 0.95;

        if (hit.distance > hitDistance || !hit.collider || hit.collider.isTrigger)
        {
            isGrounded = false;
        }
        
        //Player is only grounded if we are within distance and the player has not enabled a double jump and is still falling (otherwise the user can't double jump anymore if jump height is too low)
        else if (hit.distance <= hitDistance && doublejumpTimeout <= 0 && !(doubleJump && rb.velocity.y < 0))
        {

            jump = true;
            doubleJump = false;
            isGrounded = true;
    

            //Check if we are moving on ground
            if (rb.velocity.magnitude >= 0.1F)
            {
                if (stepSoundTimer <= 0)
                {
                    stepSoundTimer = 0.4f;

                    Material material = GetMaterialAtHit(hit);

                    if (material != null)
                    {
                        if (material.name == ("Mat_Gestein (Instance)") || material.name == ("Mat_Felsen (Instance)") || material.name == "Mat_Bruecke (Instance)" || material.name == "Mat_Ruine (Instance)")
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
    }

    void FixedUpdate()
    {
        if (doublejumpTimeout > 0.0f)
        {
            doublejumpTimeout -= Time.fixedDeltaTime;
        }

        velocity.y = rb.velocity.y;

        if (Time.realtimeSinceStartup - lastDash > 2 && !dash && isGrounded)
        {
            dash = true;
            currentSpeed = speed;
        }

        float gravity = gravityMultiplyer;

        if (_gliding)
        {
            if (isGrounded)
            {
                _gliding = false;
            }
            else
            {
                gravity /= 3;
                Debug.Log("Changed gravity enabled");
            }
        }

        rb.AddForce(Physics.gravity * gravity, ForceMode.Acceleration);

        // Player movement is handeled here
        Vector2 move = _input.Player.Movement.ReadValue<Vector2>();
        direction = new Vector3(move.x, 0f, move.y).normalized;
        Vector3 moveDir = new Vector3(0f, 0f, 0f);

        //Check if player movement should be applied
        if (direction.magnitude >= 0.1F)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity,
                turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            moveDir = (Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward).normalized * currentSpeed;

            float maxVelocityChange = speed;

            if (!isGrounded)
            {
                maxVelocityChange = airMovementSpeed;
            }

            var velocityChange = (moveDir - rb.velocity);

            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            //stair handling
            bool isFirstCheck = false;
            bool canMove = true;
            for (int i = stairDetail; i >= 1; i--)
            {
                Collider[] c =
                    Physics.OverlapBox(transform.position - new Vector3(0, i * maxStepHeight / stairDetail, 0),
                        new Vector3(1.05f, maxStepHeight / stairDetail / 2, 1.05f), Quaternion.identity, stepMask);
                if (new Vector2(velocityChange.x, velocityChange.z) != Vector2.zero)
                {
                    if (c.Length > 0 && i == stairDetail)
                    {
                        isFirstCheck = true;
                        if (!isGrounded)
                        {
                            canMove = false;
                        }
                    }

                    if (c.Length > 0 && !isFirstCheck)
                    {
                        transform.position += new Vector3(0, i * maxStepHeight / stairDetail, 0);
                        break;
                    }
                }
            }

            // handeling walls
            RaycastHit hitWall;
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 5f, Color.red);
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitWall))
            {
                if (hitWall.distance < 1.0f && !isGrounded)
                {
                    canMove = false;
                    Debug.Log("Can Move " + canMove);
                }
            }

            if (canMove)
            {
                rb.velocity += velocityChange;
            }
        }
        else if (isGrounded)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
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
        if (jump && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x,
                Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y * gravityMultiplyer), rb.velocity.z);

            isGrounded = false;
            jump = false;
            doubleJump = true;
            doublejumpTimeout = 0.3f;
            
            Debug.Log("Jump " + jump + " double jump: " + doubleJump + " ground: " + isGrounded + " out: " + doublejumpTimeout + DateTime.Now.ToString("yyyyMMddHHmmssffff"));
        }
        else if (doubleJump && !isGrounded && doublejumpTimeout <= 0.0f)
        {
            rb.velocity = new Vector3(rb.velocity.x,
                Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y * gravityMultiplyer), rb.velocity.z);

            isGrounded = false;
            jump = false;
            doubleJump = false;
            _audioManager.Play(SoundType.DoubleJump);
        }
    }

    void Dash()
    {
        if (dash)
        {
            dash = false;
            lastDash = Time.realtimeSinceStartup;
            currentSpeed = dashSpeed;
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