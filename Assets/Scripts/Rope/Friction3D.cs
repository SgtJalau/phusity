using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friction3D : MonoBehaviour
{
    public float friction = 0.0f;

    Rigidbody rb = null;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity *= (1.0f - friction);
        rb.angularVelocity *= (1.0f - friction);
    }
}
