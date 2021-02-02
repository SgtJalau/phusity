using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnCollision : MonoBehaviour
{
    public string soundName;
    public string targetLayer;
    public bool playOnlyOnce;

    private bool _alreadyPlayed = false;
    
    private AudioManager _audioManager;

    private void Awake()
    {
        _audioManager = FindObjectOfType<AudioManager>();
    }

    // Start is called before the first frame update
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(targetLayer))
        {
            if (playOnlyOnce && !_alreadyPlayed)
            {
                _audioManager.Play(soundName);
                _alreadyPlayed = true;
            }
            else if (!playOnlyOnce)
            {
                _audioManager.Play(soundName);
            }
        }
    }
}