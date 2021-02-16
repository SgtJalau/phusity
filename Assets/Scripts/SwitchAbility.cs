using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Assertions;

public enum Ability {Rope = 0, Magnet, Water, Fire};
public class SwitchAbility : MonoBehaviour
{
    
    public Ability activeAbility = Ability.Rope;
    private InputMaster _input;

    private void Awake() {
        _input = new InputMaster();
        _input.Player.AbilityChange.performed += _ => toggleAbility();
    }
    private void OnEnable()
    {
        _input.Player.Enable();
    }

    private void OnDisable()
    {
        _input.Player.Disable();
    }
    private void toggleAbility()
    {
        if (activeAbility == Ability.Rope)
        {
            activeAbility = Ability.Magnet;
        }
        else if (activeAbility == Ability.Magnet)
        {
            activeAbility = Ability.Water;
        }
        else if (activeAbility == Ability.Water)
        {
            activeAbility = Ability.Fire;
        }
        else if (activeAbility == Ability.Fire)
        {
            activeAbility = Ability.Rope;
        }
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
