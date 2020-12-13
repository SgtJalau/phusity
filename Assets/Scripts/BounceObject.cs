using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceObject : MonoBehaviour
{
    
    private Vector3 previousVelocity;
    
    private Rigidbody rigibody;


    // Start is called before the first frame update
    void Start()
    {
        rigibody = GetComponent<Rigidbody>();
    }
    
        
    void FixedUpdate()
    {
        previousVelocity = rigibody.velocity;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnCollisionEnter (Collision c) {
        //Only bounce on surfaces that are tagged as such
        if (c.gameObject.CompareTag("BounceSurface") && rigibody != null)
        {
            ContactPoint cp = c.contacts[0];

            // calculate with Vector3.Reflect
            rigibody.velocity = Vector3.Reflect(previousVelocity, cp.normal);
        }
    }
}
