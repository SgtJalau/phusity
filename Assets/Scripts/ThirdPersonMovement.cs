using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ThirdPersonMovement : MonoBehaviour
{
    public Rigidbody rb;
    public Transform camTransform;
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    public float gravityMultiplyer = 1.0f;
    public float jumpHeight = 6.0f;
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


    private GameStateHandler _gameStateHandler;
    private float lastDash;


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
        doublejumpTimeout = 0.5f;

        _gameStateHandler = new GameStateHandler();

        //Deactivate gravity since we handle it here in our own way
        rb.useGravity = false;
    }

    void Update()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit);
        
        if(hit.distance > 1.05 || !hit.collider || hit.collider.isTrigger)
        {
            isGrounded = false;
        }
        else if(hit.distance <= 1.05)
        {
            jump = true;
            doubleJump = false;
            isGrounded = true;
        }
    

        //Quick save and load (inside frame update, because key press can be ignored by fixed update)
        if (Input.GetKeyDown(KeyCode.F1))
        {
            _gameStateHandler.SaveGameState();
            Debug.Log("Quick saved state");
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            _gameStateHandler.QuickLoadGameState();
            Debug.Log("Quick loaded state");
        }
    }

    void FixedUpdate()
    {

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        direction = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 moveDir = new Vector3(0f, 0f, 0f);

        if (doublejumpTimeout > 0.0f)
        {
            doublejumpTimeout -= Time.fixedDeltaTime;
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

        //Check if player movement should be applied
        if (direction.magnitude >= 0.1F)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            moveDir = (Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward).normalized * currentSpeed;

            float maxVelocityChange = speed;
            var velocityChange = (moveDir - rb.velocity);

            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            
            //stair handling
            bool isFirstCheck = false;
            bool canMove = true;
            for (int i = stairDetail; i >= 1; i--)
            {
                Collider[] c = Physics.OverlapBox(transform.position - new Vector3(0, i * maxStepHeight / stairDetail, 0), new Vector3(1.05f, maxStepHeight / stairDetail / 2, 1.05f), Quaternion.identity, stepMask);
                if(new Vector2(velocityChange.x, velocityChange.z) != Vector2.zero){
                    if(c.Length > 0 && i == stairDetail){
                        isFirstCheck = true;
                        if(!isGrounded){
                            canMove = false;
                        }
                    }
                    if(c.Length > 0 && !isFirstCheck){
                        transform.position += new Vector3(0, i * maxStepHeight / stairDetail, 0); 
                        break;
                    }
                }

            }

            // handeling walls
            RaycastHit hitWall;
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 5f, Color.red);
            if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitWall)){
                if(hitWall.distance < 1.0f && !isGrounded){
                    canMove = false;
                    Debug.Log("Can Move "+canMove);
                }
            }

            if(canMove)
                rb.velocity += velocityChange;
        } else if (isGrounded)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }


        if (Input.GetKey(KeyCode.Space) && jump && isGrounded)
        {
            rb.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y * gravityMultiplyer),
                ForceMode.Impulse);
            jump = false;
            doubleJump = true;
            doublejumpTimeout = 0.5f;
        }
        else if (Input.GetKey(KeyCode.Space) && doubleJump && !isGrounded && doublejumpTimeout <= 0.0f)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y * gravityMultiplyer), ForceMode.Impulse);
            jump = false;
            doubleJump = false;
        }
        else if (Input.GetKey(KeyCode.LeftControl) && _glidingEnabled && direction.y <= 0)
        {
            _gliding = true;
            _glidingEnabled = false;
            Debug.Log("Gliding enabled");
        }

        velocity.y = rb.velocity.y;

        if (Time.realtimeSinceStartup - lastDash > 0.3)
        {
            dash = true;
            currentSpeed = speed;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && dash)
        {
            dash = false;
            lastDash = Time.realtimeSinceStartup;
            currentSpeed = 20.0f;
        }

    }

    public IEnumerator LookAtLocation(Transform vector3, long millis)
    {
        Transform previousFollow = _virtualCamera.ActiveVirtualCamera.Follow;
        Transform previousLookAt = _virtualCamera.ActiveVirtualCamera.LookAt;
        
        //_virtualCamera.OutputCamera.transform.LookAt(vector3);
        _virtualCamera.ActiveVirtualCamera.Follow = vector3;
        _virtualCamera.ActiveVirtualCamera.LookAt = vector3;

      
        
        yield return new WaitForSeconds(millis/1000F);
        
        _virtualCamera.ActiveVirtualCamera.Follow = previousFollow;
        _virtualCamera.ActiveVirtualCamera.LookAt = previousLookAt;
    }
}