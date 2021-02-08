using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class BuoyancyItem : MonoBehaviour
{
    [SerializeField, Tooltip("Increase value to make object more buoyant, default 8.")]
    float buoyantForce = 8f;

    [SerializeField, Tooltip("0 no underwater pressure, 1 = double underwater pressure"),
     Range(0f, 1f)]
    float depthPower = 1f;

    [SerializeField, Tooltip("Center of mass on Y axis (kind of), default 0.")]
    float offsetY = 0f;

    private Rigidbody rb;
    private Collider coll;
    private WaterBody waterBody;
    private float yBound;


    private List<WaterBody> _waterCount = new List<WaterBody>();


    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
    }

    private void Update()
    {
        if (_waterCount.Count == 0)
        {
            waterBody = null;
        }
    }

    private void OnTriggerEnter(Collider water)
    {
        WaterBody colliderBody = water.GetComponent<WaterBody>();

        if (!colliderBody)
        {
            return;
        }

        _waterCount.Add(colliderBody);

        waterBody = _waterCount[0];
    }

    private void OnTriggerStay(Collider water)
    {
        WaterBody colliderBody = water.GetComponent<WaterBody>();
        
        if (!colliderBody || colliderBody != waterBody)
        {
            Debug.Log(colliderBody  + " " + waterBody);
            return;
        }
        

        if (transform.position.x < water.bounds.max.x
            && transform.position.z < water.bounds.max.z
            && transform.position.x > water.bounds.min.x
            && transform.position.z > water.bounds.min.z)
        {
            //Check if we are still within the bounds of the water height
            //This should roughly keep a realistic amount of body above water
            var bounds = coll.bounds;
            float objectYValue = bounds.min.y + 0.3f * (bounds.center.y - bounds.min.y);

            yBound = waterBody.GetYBound();

            if (objectYValue < yBound)
            {
                float buoyantForceMass = buoyantForce * rb.mass;

                //Min of 0 and max of 1
                float underWaterBuoyantForce = Mathf.Clamp01((yBound - objectYValue) * depthPower);
                float buoyency = buoyantForceMass + (buoyantForceMass * underWaterBuoyantForce);

                //Add the current velocity on top inverted so we start slowly floating at the top of the water
                rb.AddForce(0, -rb.velocity.y, 0);

                //Add buoyency
                rb.AddForce(0f, buoyency, 0f);
            }
        }
    }

    private void OnTriggerExit(Collider water)
    {
        WaterBody colliderBody = water.GetComponent<WaterBody>();

        if (!colliderBody)
        {
            return;
        }

        _waterCount.Remove(colliderBody);

        if (_waterCount.Count > 0)
        {
            waterBody = _waterCount[0];
            Debug.Log("colliderBody  + \" \" + waterBody");
        }
    }
}