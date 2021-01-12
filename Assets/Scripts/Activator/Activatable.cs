using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Activatable : MonoBehaviour
{
    public abstract void activate();
    public abstract void deactivate();
}
