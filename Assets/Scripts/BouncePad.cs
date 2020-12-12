using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public int speed = 100;

    void OnCollisionEnter(Collision other)
    {
        //Check if rigidbody exists
        if (other.gameObject.GetComponent<Rigidbody>() != null)
        {
            //Vector3 v3Velocity = other.gameObject.GetComponent<Rigidbody>().velocity;
            //Vector3 newDirection = Vector3.Reflect(v3Velocity, other.contacts[0].normal);
            //other.gameObject.GetComponent<Rigidbody>().useGravity = false;
            //other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            
            //other.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.right*speed*v3Velocity.x);
            //other.gameObject.GetComponent<Rigidbody>().velocity = (newDirection*speed);
            //other.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.forward*speed*v3Velocity.z);
        }
    }
}