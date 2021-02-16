using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class PlayerTutorial : MonoBehaviour
{
    private PlayerObject _playerObject;

    private TutorialState _tutorialState = TutorialState.Movement;

    private InputMaster _inputMaster;

    void Start()
    {
        _playerObject = GetComponent<PlayerObject>();
        _playerObject.PlayerGUI.ShowTextbox();
        
        _inputMaster = _playerObject.ThirdPersonMovement.InputMaster;
        
        UpdateState();

        EventManager.Jump.OnPerformed.Add(Jump);
        EventManager.DoubleJump.OnPerformed.Add(DoubleJump);
        EventManager.Movement.OnPerformed.Add(Movement);
        EventManager.Dash.OnPerformed.Add(Dash);
    }

    private void Movement()
    {
        if (_tutorialState == TutorialState.Movement) 
        {
            _tutorialState = TutorialState.Jump;
            UpdateState();
        }
    }

    void UpdateState()
    {
        String headline;
        String text;

        switch (_tutorialState)
        {
            case TutorialState.Movement:
                headline = "Movement";
                text = "Use " + _playerObject.ThirdPersonMovement.GetReadableKeyName(_inputMaster.Player.Movement) +  " to move your character around";
                break;
            case TutorialState.Jump:
                headline = "Jump";
                text = "Press " + _playerObject.ThirdPersonMovement.GetReadableKeyName(_inputMaster.Player.Jump) +  " to jump";
                break;
            case TutorialState.DoubleJump:
                headline = "Double Jump";
                text = "Tap your " + _playerObject.ThirdPersonMovement.GetReadableKeyName(_inputMaster.Player.Jump) +  " twice with a slight delay to double jump";
                break;
            case TutorialState.Dash:
                headline = "Dash";
                text = "Press " + _playerObject.ThirdPersonMovement.GetReadableKeyName(_inputMaster.Player.Dash) +  " while in the air to dash forward";
                break;
            default:
                _playerObject.PlayerGUI.HideTextbox();
                return;
        }

        _playerObject.PlayerGUI.SetTextbox(headline, text);
    }

    void Jump()
    {
        if (_tutorialState == TutorialState.Jump)
        {
            _tutorialState = TutorialState.DoubleJump;
            UpdateState();
        }
    }
    
    void DoubleJump()
    {
        if (_tutorialState == TutorialState.DoubleJump )
        {
            _tutorialState = TutorialState.Dash;
            UpdateState();
        }
    }

    void Dash()
    {
        if (_tutorialState == TutorialState.Dash)
        {
            _tutorialState = TutorialState.End;
            UpdateState();
        }
    }

    public enum TutorialState
    {
        Movement,
        Jump,
        DoubleJump,
        Dash,
        End,
    }
}