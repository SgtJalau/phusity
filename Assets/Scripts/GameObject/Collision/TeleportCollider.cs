using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportCollider : MonoBehaviour
{
    public TeleportLocation teleportLocation;

    private void OnTriggerEnter(Collider other)
    {
        ThirdPersonMovement thirdPersonMovement = null;

        if (other.gameObject.TryGetComponent(out thirdPersonMovement))
        {
            teleportLocation.TeleportToLocation(other.gameObject);
        }
    }
}