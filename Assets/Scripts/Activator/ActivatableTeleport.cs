using UnityEngine;

public class ActivatableTeleport : Activatable
{
    public TeleportLocation teleportLocation;
    public GameObject toTeleportObject;
    
    public override void activate()
    {
        teleportLocation.TeleportToLocation(toTeleportObject);
    }

    public override void deactivate()
    {
        
    }
}