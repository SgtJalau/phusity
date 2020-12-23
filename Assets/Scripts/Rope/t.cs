using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class t : MonoBehaviour
{

    public float speed = 0.0f;
    public float offset = 0.0f;
    public float dir = 1.0f;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {
        rb.position += speed * dir * Vector3.right * Time.fixedDeltaTime;
        offset += speed * dir * Time.fixedDeltaTime;
        if (Mathf.Abs(offset) > 4)
            dir *= -1f;
    }
}
