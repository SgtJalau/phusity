﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public Rigidbody rb;
    public Transform camTransform;
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;

    private bool jump;
    private bool dash;
    private float turnSmoothVelocity;
    private Vector3 velocity;
    private bool isGrounded;
    private Vector3 direction;
    private float currentSpeed;

    private float lastDash;
    
    void Start() 
    {
        velocity = new Vector3(0f, 0f, 0f);
        direction = new Vector3(0f, 0f, 0f);
        jump = true;
        dash = true;
        isGrounded = false;
        lastDash = Time.realtimeSinceStartup;
        currentSpeed = speed;
    }

    void OnCollisionEnter(Collision collision){
        jump = true;
        isGrounded = true;
    }

    void OnCollisionExit(Collision collision){
        isGrounded = false;
    }

    void Update(){

    }

    void FixedUpdate() {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        direction = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 moveDir = new Vector3(0f, 0f, 0f);

        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            velocity.x = moveDir.normalized.x * currentSpeed;
            velocity.z = moveDir.normalized.z * currentSpeed;
        }else{
            velocity.x = 0f;
            velocity.z = 0f;
        }
        if (Input.GetKey(KeyCode.Space) && jump && isGrounded)
        {
            rb.AddForce(Vector3.up * 10.0f, ForceMode.Impulse);
            jump = false;
        }
        velocity.y = rb.velocity.y;

        if(Time.realtimeSinceStartup - lastDash > 0.3){
            dash = true;
            currentSpeed = speed;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && dash){
            dash = false;
            lastDash = Time.realtimeSinceStartup;
            currentSpeed = 20.0f;
        }
        rb.velocity = velocity;
    }
}
