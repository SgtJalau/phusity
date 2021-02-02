using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyPad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    void OnCollisionEnter(Collision other)
    {

        Rigidbody rigidbody = other.gameObject.GetComponent<Rigidbody>();
        
        //Check if rigidbody exists
        if (rigidbody != null)
        {
            rigidbody.useGravity = false;
            rigidbody.velocity = Vector3.zero;
            
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
            
            //Unfreeze
            // rigidbody.constraints = RigidbodyConstraints.None;
        }
    }
}
