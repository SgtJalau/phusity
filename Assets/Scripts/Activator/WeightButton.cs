using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightButton : Activator
{
    public float massLimit = 10.0f;

    private List<Collider> colliders = new List<Collider>();
    private bool lastFrame = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!colliders.Contains(other)) 
        { 
            colliders.Add(other); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        colliders.Remove(other);
    }

    void FixedUpdate()
    {
        float totalMass = 0.0f;
        foreach (Collider c in colliders)
        {
            Rigidbody cRb = c.attachedRigidbody;
            if (cRb.IsSleeping())
            {
                totalMass += cRb.mass;
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
