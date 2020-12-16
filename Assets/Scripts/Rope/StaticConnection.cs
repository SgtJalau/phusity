using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticConnection : BaseConnection
{
    public GameObject connection;
    
    public override void DestroyConnection()
    {
        base.DestroyConnection();
        Object.Destroy(connection);
    }
}
