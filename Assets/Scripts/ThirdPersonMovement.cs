using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public Rigidbody rb;
    public Transform camTransform;
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    public float gravityMultiplyer = 1.0f;
    public float jumpHeight = 6.0f;

    /**
     * If gliding is enabled we can press a key to start gliding
     */
    private bool _glidingEnabled = false;

    public bool GlidingEnabled
    {
        get => _glidingEnabled;
        set => _glidingEnabled = value;
    }

    private bool _gliding = false;

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

    }

    void OnCollisionEnter(Collision collision)
    {
        jump = true;
        doubleJump = false;
        isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
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

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity,
                turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            velocity.x = moveDir.normalized.x * currentSpeed;
            velocity.z = moveDir.normalized.z * currentSpeed;
        }
        else
        {
            velocity.x = 0f;
            velocity.z = 0f;
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
            rb.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y * gravityMultiplyer),
                ForceMode.Impulse);
            jump = false;
            doubleJump = false;
        }
        else if (Input.GetKey(KeyCode.LeftShift) && _glidingEnabled && direction.y <= 0)
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

        rb.velocity = velocity;
    }
}