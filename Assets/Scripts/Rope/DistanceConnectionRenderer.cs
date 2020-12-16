using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceConnectionRenderer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform t1;
    public Transform t2;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.widthMultiplier = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, t1.position);
        lineRenderer.SetPosition(1, t2.position);
    }
}
