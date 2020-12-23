using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
    public GameObject targetGameObject;

    public ROPE_TYPE type;

    //TODO: some way to auto configure for convenience?
    public float maxLength = 1.0f;

    public bool addDistanceConstraint = false;

    public float linkScale = 1.0f;

    public float linkMass = 1.0f;

    public CollisionDetectionMode collisionMode;

    public bool enablePrePro = false;

    public bool enableProjection = true;

    /************************************************/

    public BaseConnection activeConnection = null;

    void Start()
    {
        if (transform.Find("TargetPosition") == null)
        {
            GameObject target = new GameObject("TargetPosition");
            target.transform.parent = transform;
            target.transform.localPosition = Vector3.zero;
            target.transform.localScale = Vector3.one;
        }
        Debug.Assert(targetGameObject.tag == "Untagged" || targetGameObject.tag == "DynamicRopeTarget",
            "Target Collider already had a different Tag and was overridden: " + gameObject.name);
        Debug.Assert(targetGameObject.layer == 0 || targetGameObject.layer == LayerMask.NameToLayer("RopeTarget"),
            "Target Collider already had a different Layer and was overridden: " + gameObject.name);
        if (type == ROPE_TYPE.DYNAMIC_DISTANCE || type == ROPE_TYPE.DYNAMIC_LINK)
        {
            targetGameObject.tag = "DynamicRopeTarget";
        }
        else
        {
            targetGameObject.tag = "Untagged";
        }
        targetGameObject.layer = LayerMask.NameToLayer("RopeTarget");

    }
}