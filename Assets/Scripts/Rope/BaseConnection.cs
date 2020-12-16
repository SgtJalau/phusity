using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseConnection
{
    public RopeTarget end1;
    public RopeTarget end2;

    public virtual void DestroyConnection() 
    {
        end1.activeConnection = null;
        end2.activeConnection = null;
    }
}
