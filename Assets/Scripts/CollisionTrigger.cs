using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTrigger : MonoBehaviour
{
    public ParticleSystem OnHitEffect;
    ParticleSystem OnHitInit;
    //public Vector3 pos;

    void Start()
    {
        OnHitInit = Instantiate(OnHitEffect, GameObject.Find("SteinAnSeil").transform);
        Debug.Log(GameObject.Find("SteinAnSeil").transform);
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("CollisionCheck"))
        {
            Debug.Log("ReachedIf");
            OnHitInit.Play();
        }

    }
}


