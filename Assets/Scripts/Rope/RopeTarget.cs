using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ROPE_TYPE
{
    [InspectorName("Static")]
    STATIC,
    [InspectorName("Simulate Links")]
    DYNAMIC_LINK,
    [InspectorName("Limit Distance")]
    DYNAMIC_DISTANCE,
}

[ExecuteInEditMode]
public class RopeTarget : MonoBehaviour
{

    [Header("GameObject defining Target (requires Collider)")]
    public GameObject target;

    [Header("Type of Target")]
    public ROPE_TYPE type;

    [Header("Max length used for type 'Limit Distance'")]
    //TODO: some way to auto configure for convenience?
    public float maxLength = 1.0f;

    void Start()
    {
        if (transform.Find("TargetPosition") == null)
        {
            GameObject target = new GameObject("TargetPosition");
            target.transform.parent = transform;
            target.transform.localPosition = Vector3.zero;
            target.transform.localScale = Vector3.one;
        }
        if (type == ROPE_TYPE.DYNAMIC_DISTANCE || type == ROPE_TYPE.DYNAMIC_LINK)
        {
            target.tag = "DynamicRopeTarget";
        }
        else
        {
            target.tag = "Untagged";
        }
        target.layer = LayerMask.NameToLayer("RopeTarget");

    }

    void Update()
    {
        
    }
}
