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
    public LayerMask groundMask;

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
    private Vector3 cachedVelocity;


    private GameStateHandler _gameStateHandler;
    private float lastDash;


    void Start()
    {
        velocity = new Vector3(0f, 0f, 0f);
        cachedVelocity =  new Vector3(0f, 0f, 0f);
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

    void OnCollisionEnter(Collision collision)
    {
        if ((groundMask & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer){
            jump = true;
            doubleJump = false;
            isGrounded = true;
        }else{
            rb.velocity = cachedVelocity;
            rb.AddForce(Physics.gravity * gravityMultiplyer, ForceMode.Acceleration);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if ((groundMask & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer){
            isGrounded = false;
        }else{
            rb.velocity = cachedVelocity;
            rb.AddForce(Physics.gravity * gravityMultiplyer, ForceMode.Acceleration);
        }
    }

    void Update()
    {
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
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity,
                turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            moveDir = (Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward).normalized * currentSpeed;

            //Vector3 movementVector = new Vector3();
            float maxVelocityChange = speed;
            //float movementX = moveDir.x;
            //float movementZ = moveDir.z;
            //velocity.x = Math.Max(velocity.x, movementX);
            //velocity.z = Math.Max(velocity.z, movementZ);
            //velocity.x = moveDir.normalized.x * currentSpeed;
            //velocity.z = moveDir.normalized.z * currentSpeed;

            // Apply a force that attempts to reach our target velocity
            var velocityChange = (moveDir - rb.velocity);

            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            
            //Add it (without the function, since the function seems to be messy)
            rb.velocity += velocityChange;
        } else if (isGrounded)
        {
            //Player is on ground and not moving, so we can set velocity to zero
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }


        //rb.AddForce(velocityChange, ForceMode.VelocityChange);
        //rb.AddForce(_addedForce, ForceMode.Force);


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
            rb.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y * gravityMultiplyer),
                ForceMode.Impulse);
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
        cachedVelocity = rb.velocity;
        //rb.AddForce(velocity, ForceMode.VelocityChange);
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