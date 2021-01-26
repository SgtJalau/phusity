using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightButton : Activator
{
    public float massLimit = 10.0f;
    
    [Tooltip("The amount of millis objects that have previously slept and are now moving will still be accounted for in mass")]
    public float sleepingToleranceMillis = 1000;

    //Holds all colliders that are currently colliding with our plate and the last time they were resting on the plate in millis
    private Dictionary<Collider, long> colliders = new Dictionary<Collider, long>();
    
    private bool lastFrame = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!colliders.ContainsKey(other)) 
        { 
            colliders.Add(other, 0); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        colliders.Remove(other);
    }

    void FixedUpdate()
    {
        float totalMass = 0.0f;
        
        //We need to clone the keys to a separate list so we can modify the dictionary inside the loop
        List<Collider> keys = new List<Collider>(colliders.Keys);
        
        foreach (Collider c in keys)
        {
            Rigidbody cRb = c.attachedRigidbody;
            var sleeping = cRb.IsSleeping();
            
            //Check for sleeping or tolerance
            if (sleeping || colliders[c] >= DateTimeOffset.Now.ToUnixTimeMilliseconds() - sleepingToleranceMillis)
            {
                totalMass += cRb.mass;
                
                if (sleeping)
                {
                    //If we are sleeping update the last sleep time
                    colliders[c] = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                }
            }
        }
        bool currentFrame = totalMass >= massLimit;
        if(currentFrame != lastFrame)
        {
            if (currentFrame)
            {
                activate();
            }
            else
            { 
                deactivate();
            }
        }
        lastFrame = currentFrame;
    }
}
