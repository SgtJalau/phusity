using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventManager
{
    public static readonly Event Jump = new Event();
    public static readonly Event DoubleJump = new Event();
    public static readonly Event Dash = new Event();
    public static readonly Event Movement = new Event();
    public static readonly Event DragObject = new Event();
    public static readonly Event SwitchAbility = new Event();
    
    public class Event
    {
        private readonly List<Action> _onPerformed = new List<Action>();

        public List<Action> OnPerformed => _onPerformed;

        public void Perform()
        {
            foreach (var action in _onPerformed)
            {
                action.Invoke();
            }
        }
    }
}