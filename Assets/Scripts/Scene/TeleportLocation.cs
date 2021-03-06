﻿using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class TeleportLocation
{
    public Vector3 teleportLocation;

    public int scene;
    
    public bool TeleportToLocation(GameObject gameObject)
    {
        SceneManager.LoadScene(scene);
        gameObject.transform.position = teleportLocation;

        return true;
    }
}