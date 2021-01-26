using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WaterBody : MonoBehaviour
{
    public Vector3 flowVector;

    [SerializeField, Tooltip("Check this to enable a custom surface level")]
    bool customSurfaceLevel = false;

    [SerializeField, Tooltip("Surface level of waterbody in Y axis")]
    float surfaceLevel = 0f;

    private Collider coll;
    
    private List<Rigidbody> colliders = new List<Rigidbody>();

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        foreach (var collider in colliders)
        {
            //Add flow of water force
            var velocity = collider.velocity;

            var resultingVector = new Vector3();
            
            if (velocity.x < flowVector.x)
            {
                resultingVector.x = flowVector.x;
            }
            
            if (velocity.y < flowVector.y)
            {
                resultingVector.y = flowVector.y;
            }
            
            if (velocity.z < flowVector.z)
            {
                resultingVector.z = flowVector.z;
            }
            
            collider.AddForce(resultingVector, ForceMode.Acceleration);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody caught = other.GetComponent<Rigidbody>();


        if (!caught)
        {
            return;
        }

        if (!colliders.Contains(caught))
        {
            colliders.Add(caught);
            
            //Check if it already exists (in case we are already in another water source, or we pre assigned it to change a few values)
            BuoyancyItem buoyancy = other.GetComponent<BuoyancyItem>();

            if (buoyancy)
            {
                return;
            }
            
            //Add the buoyancy component
            caught.gameObject.AddComponent<BuoyancyItem>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody caught = other.GetComponent<Rigidbody>();
        BuoyancyItem buoyancy = other.GetComponent<BuoyancyItem>();

        if (!caught || !buoyancy)
        {
            return;
        }

        colliders.Remove(caught);
        
        //Remove buoyancy from game object
        Destroy(buoyancy);
    }


    /*
     * Get surface y level of water
     */
    public float GetYBound()
    {
        if (!customSurfaceLevel)
        {
            surfaceLevel = coll.bounds.max.y;
        }

        return surfaceLevel;
    }
}