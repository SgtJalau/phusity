using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportCollider : MonoBehaviour
{
    public TeleportLocation teleportLocation;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            teleportLocation.TeleportToLocation(other.gameObject);
        }
    }
}