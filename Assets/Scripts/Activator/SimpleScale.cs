using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleScale : Activator
{
    private bool lastFrame = false;
    private Transform t;
    
    void Start()
    {
        t = gameObject.transform;
    }

    void Update()
    {
        bool thisFrame = Mathf.Abs(t.eulerAngles.z) < 5.0f;
        if (thisFrame != lastFrame)
        {
            if (thisFrame)
                activate();
            else
            {
                deactivate();
            }
        }
        lastFrame = thisFrame;
    }
}
