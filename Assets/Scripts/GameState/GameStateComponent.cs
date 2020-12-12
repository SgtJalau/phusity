using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class GameStateComponent : MonoBehaviour
{

    /**
     * Holds the unique id for that objects so it can be restored and identified
     */
    public System.Guid guid = System.Guid.NewGuid();
    
}
