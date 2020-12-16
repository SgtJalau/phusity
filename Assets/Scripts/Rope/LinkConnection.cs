using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkConnection : BaseConnection
{
    public GameObject emptyWithLinks;
    public Joint endJoint;
    public Joint startJoint;

    public override void DestroyConnection()
    {
        base.DestroyConnection();
        Object.Destroy(emptyWithLinks);
        Object.Destroy(startJoint);
        Object.Destroy(endJoint);
    }
}
