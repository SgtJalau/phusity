using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceConnection : BaseConnection
{
    public SpringJoint springJoint;
    public GameObject rendererEmpty;

    public override void DestroyConnection()
    {
        base.DestroyConnection();
        Object.Destroy(springJoint);
        Object.Destroy(rendererEmpty);
    }
}
