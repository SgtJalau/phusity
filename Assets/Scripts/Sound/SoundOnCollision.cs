using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnCollision : MonoBehaviour
{
    public string soundName;
    public string targetLayer;
    public bool playOnlyOnce;

    private bool allreadyPlayed = false;
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.layer == LayerMask.NameToLayer(targetLayer) )
        {
            if (playOnlyOnce == true && allreadyPlayed == false)
            {
                FindObjectOfType<AudioManager>().Play(soundName);
                allreadyPlayed = true;
            }
            else if(playOnlyOnce == false){
                FindObjectOfType<AudioManager>().Play(soundName);
            }
        }
    }
}
