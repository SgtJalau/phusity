using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activator : MonoBehaviour
{
    public List<Activatable> activatableTargets = new List<Activatable>();

    public void activate()
    {
        foreach (Activatable target in activatableTargets)
            target.activate();
    }

    public void deactivate()
    {
        foreach (Activatable target in activatableTargets)
            target.deactivate();
    }
}
