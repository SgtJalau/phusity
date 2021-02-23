using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTutorialCollision : MonoBehaviour
{

    public PlayerTutorial.TutorialState tutorialStateToSet;
    public PlayerTutorial.TutorialState requiredState;

    private bool _triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        PlayerTutorial playerTutorial;
        
        if (!_triggered && other.gameObject.TryGetComponent(out playerTutorial))
        {
            Debug.Log(playerTutorial.CurrentState);
            if (playerTutorial.CurrentState == requiredState)
            {
                playerTutorial.SetTutorialState(tutorialStateToSet);
                _triggered = true;
            }
            else
            {
                playerTutorial.QueuedStates.Enqueue(tutorialStateToSet);
                _triggered = true;
            }
        }
    }
}
