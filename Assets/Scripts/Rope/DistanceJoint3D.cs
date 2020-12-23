using UnityEngine;

//from https://www.youtube.com/watch?v=ft6s09cq7DM

public class DistanceJoint3D : MonoBehaviour
{

    public Transform TargetTransform;
    public bool DetermineDistanceOnStart = true;
    public float Distance;
    public float Spring = 0.1f;
    public float Damper = 5f;

    protected Rigidbody Rigidbody;

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (DetermineDistanceOnStart && TargetTransform != null)
            Distance = Vector3.Distance(Rigidbody.position, TargetTransform.position);
    }

    void FixedUpdate()
    {

        var connection = Rigidbody.position - TargetTransform.position;
        var distanceDiscrepancy = Distance - connection.magnitude;

        Rigidbody.position += distanceDiscrepancy * connection.normalized;

        var velocityTarget = connection + (Rigidbody.velocity + Physics.gravity * Spring);
        var projectOnConnection = Vector3.Project(velocityTarget, connection);
        Rigidbody.velocity = (velocityTarget - projectOnConnection) / (1 + Damper * Time.fixedDeltaTime);


    }
}