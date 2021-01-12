using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HingeJoint))]
public class SimpleDoor : Activatable
{
    public float startingVelocity = 10.0f;

    private HingeJoint joint;
    private JointMotor motor;

    void Start()
    {
        joint = gameObject.GetComponent<HingeJoint>();
        motor = joint.motor;
    }

    public override void deactivate()
    {
        motor.targetVelocity = startingVelocity;
        joint.motor = motor;
    }

    public override void activate()
    {
        motor.targetVelocity = -startingVelocity;
        joint.motor = motor;
    }

}
