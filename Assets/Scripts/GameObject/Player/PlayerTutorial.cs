using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class PlayerTutorial : MonoBehaviour
{
    private PlayerObject _playerObject;
    
    private TutorialState _tutorialState = TutorialState.Movement;
    
    public TutorialState CurrentState
    {
        get => _tutorialState;
        set => _tutorialState = value;
    }
    
    private Queue<TutorialState> _queuedStates = new Queue<TutorialState>();
    
    public Queue<TutorialState> QueuedStates
    {
        get => _queuedStates;
        set => _queuedStates = value;
    }

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
        EventManager.DragObject.OnPerformed.Add(Drag);
        EventManager.SwitchAbility.OnPerformed.Add(SwitchAbility);

        _inputMaster.Player.TargetMode.performed += _ => TargetModeSwitch();
    }

    private void Movement()
    {
        if (_tutorialState == TutorialState.Movement) 
        {
            SetTutorialState(TutorialState.Dash);
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
            case TutorialState.PushBox:
                headline = "Push and Pull Objects";
                text = "Press " + _playerObject.ThirdPersonMovement.GetReadableKeyName(_inputMaster.Player.Drag) +  " to drag objects and push against them to move them around";
                break;
            case TutorialState.SwitchAbility:
                headline = "Switch Active Ability";
                text = "Press " + _playerObject.ThirdPersonMovement.GetReadableKeyName(_inputMaster.Player.AbilityChange) +  " to switch your active ability to the rope ability";
                break;
            case TutorialState.TargetRope:
                headline = "Target Rope Anchor";
                text = "Press " + _playerObject.ThirdPersonMovement.GetReadableKeyName(_inputMaster.Player.TargetMode) +  " to toggle rope target mode";
                break;
            case TutorialState.SelectRopeTarget:
                headline = "Select Rope Anchor";
                text = "Press " + _playerObject.ThirdPersonMovement.GetReadableKeyName(_inputMaster.Player.Interact) +  " to select two rope anchors for your rope while in target mode";
                break;
            default:
                _playerObject.PlayerGUI.HideTextbox();
                CheckForQueuedStates();
                return;
        }

        _playerObject.PlayerGUI.ShowTextbox();
        _playerObject.PlayerGUI.SetTextbox(headline, text);
    }

    void Jump()
    {
        if (_tutorialState == TutorialState.Jump)
        {
            SetTutorialState(TutorialState.DoubleJump);
        }
    }
    
    void DoubleJump()
    {
        if (_tutorialState == TutorialState.DoubleJump)
        {
            SetTutorialState(TutorialState.DoubleJumpFinished);
        }
    }

    void Dash()
    {
        if (_tutorialState == TutorialState.Dash)
        {
            SetTutorialState(TutorialState.DashFinished);
        }
    }

    void Drag()
    {
        if (_tutorialState == TutorialState.PushBox)
        {
            SetTutorialState(TutorialState.PushBoxFinished);
        }
    }
    
    void SwitchAbility()
    {
        if (_tutorialState == TutorialState.SwitchAbility)
        {
            if (_playerObject.GetActiveAbility() == Ability.Rope)
            {
                SetTutorialState(TutorialState.TargetRope);
            }
        }
    }

    void TargetModeSwitch()
    {
        if (_tutorialState == TutorialState.TargetRope)
        {
            if (_playerObject.GetActiveAbility() == Ability.Rope)
            {
                SetTutorialState(TutorialState.SelectRopeTarget);
            }
        }
    }


   public void SetTutorialState(TutorialState tutorialState)
   {
       _tutorialState = tutorialState;
       UpdateState();
   }

   public void CheckForQueuedStates()
   {
       if (_queuedStates.Count > 0)
       {
           SetTutorialState(_queuedStates.Dequeue());
       }
   }
    
    public enum TutorialState
    {
        Movement,
        Dash,
        DashFinished,
        Jump,
        DoubleJump,
        DoubleJumpFinished,
        PushBox,
        PushBoxFinished,
        SwitchAbility,
        TargetRope,
        SelectRopeTarget,
        End,
    }
}