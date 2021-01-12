using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class GameStateComponent : MonoBehaviour
{

    /**
     * Holds the unique id for that objects so it can be restored and identified
     */
    public string guid = System.Guid.NewGuid().ToString();

    /**
     * Do we respawn the object on collision with another object that has the respawn tag (like water, lava etc.)
     */
    public bool respawnOnCollide = false;
    
    private GameStateHandler.GameObjectState gameObjectState;
    private GameStateComponent _gameStateComponent;

    private Transform _transform;

    void Start()
    {
        //Save initial state in memory (when object is spawned in)
        _gameStateComponent = gameObject.GetComponent<GameStateComponent>();
        gameObjectState = new GameStateHandler.GameObjectState(_gameStateComponent);
        
        _transform = transform;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Respawn") && respawnOnCollide)
        {
            //StartCoroutine(RespawnGameObject(0, 1));
            gameObjectState.RestoreState(_gameStateComponent);
        }
    }
    
    //Broken for some reason
    private IEnumerator RespawnGameObject(float timeToDespawn, float timeToRespawn) {
        yield return new WaitForSeconds(timeToDespawn);
        gameObject.SetActive(false);
        yield return new WaitForSeconds(timeToRespawn);
        gameObjectState.RestoreState(_gameStateComponent);
        gameObject.SetActive(true);
    }
    
    void OnCollisionExit(Collision collision)
    {
        //Check if we are out of the collision of a moving platform
        if (collision.gameObject.GetComponent<MovingPlatform>() != null)
        {
            //Remove parent platform so we now longer move along with it
            _transform.parent = null;
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        //Check if we are colliding with a moving platform
        if (collision.gameObject.GetComponent<MovingPlatform>() != null)
        {
            //This will make the player a child of the platform
            _transform.parent = collision.gameObject.transform;
        }
    }


    
}
