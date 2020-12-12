using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform camTransform;
    public float speed = 6f;
    public float jumpHeight = 3f;
    public float gravity = -9.81f;

    public float turnSmoothTime = 0.1f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private float turnSmoothVelocity;
    private float dashSmoothVelocity;
    private Vector3 velocity;
    private bool isGrounded;
    private bool doubleJump;
    private bool dashready;
    private bool dashactive;
    private float currentSpeed;
    private float dashspeed;

    void Start() 
    {
        currentSpeed = speed;
        velocity = new Vector3(0f, -9.81f, 0f);
        doubleJump = false;
        dashready = true;
        dashactive = false;
        dashspeed = 1f;
    }
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 moveDir = new Vector3(0f, 0f, 0f);

        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            velocity.x = moveDir.normalized.x * currentSpeed;
            velocity.z = moveDir.normalized.z * currentSpeed;
        }else if(!dashactive){
            velocity.x = 0f;
            velocity.z = 0f;
        }

        if(dashready && !dashactive && direction.magnitude >= 0.1f){
            if(Input.GetKeyDown(KeyCode.LeftShift)){
                dashspeed = 8f;
                dashready = false;
                dashactive = true;
            }
        }
        if(!dashready && dashactive){
            dashspeed = Mathf.SmoothDamp(dashspeed, 1f, ref dashSmoothVelocity, 0.15f);
            if (dashspeed <= 1.1f)
            {
                dashspeed = 1f;
                dashready = true;
                dashactive = false;
            }
        }
        
        if(doubleJump)
        {
            if(Input.GetButtonDown("Jump"))
            {
                doubleJump = false;
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        if (isGrounded)
        {
            currentSpeed = speed;
            doubleJump = false;
            if(velocity.y < 0)
            {
                velocity.y = -9.81f;
            }
            if(Input.GetButtonDown("Jump"))
            {
                doubleJump = true;
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            if(currentSpeed > (speed/2.0f))
                currentSpeed -= (speed/2.0f) * Time.deltaTime;
            velocity.y += gravity * Time.deltaTime;
        }


        controller.Move(new Vector3(velocity.x * dashspeed, velocity.y, velocity.z * dashspeed) * Time.deltaTime);
    }
}
