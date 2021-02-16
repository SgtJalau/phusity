using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Activatable : MonoBehaviour
{
    public abstract void activate();
    public abstract void deactivate();
    
    protected IEnumerator LookAtObject(long initialDelayMillis, long durationMillis)
    {
        yield return new WaitForSeconds(initialDelayMillis/1000F);
        
        var objects = Object.FindObjectsOfType<PlayerObject>();
        PlayerObject player = objects[0];
        StartCoroutine(player.LookAtLocation(transform, durationMillis));
    }
}
