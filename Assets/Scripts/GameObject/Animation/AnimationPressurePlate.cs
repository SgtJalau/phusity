using System;
using UnityEngine;

public class AnimationPressurePlate : Activatable
{

    [Tooltip("The y amount that will differ active from inactive state")]
    public float yChange = 0.1F;
    
    [Tooltip("The speed of the animation in 1/10 per game frame")]
    public float animationSpeed = 0.05F;
    
    private float _yChanged = 0;
    private bool _active = false;
    
    public override void activate()
    {
        _active = true;
    }

    public void Update()
    {
        //If the animation is set to active state and we haven't reached the targeted value yet continue aninmation
        if (_active && yChange >= _yChanged*-1)
        {
            float change = animationSpeed / 10;
            var transform1 = transform;
            var position = transform1.position;
            position = new Vector3(position.x, position.y - change, position.z);
            transform1.position = position;
            
            _yChanged -= change;
        } else if (!_active && _yChanged < 0) 
        {
            //If we are inactive and haven't reset back to 0 yet continue animation
            float change = animationSpeed / 10;
            var transform1 = transform;
            var position = transform1.position;
            position = new Vector3(position.x, position.y + change, position.z);
            transform1.position = position;
            
            _yChanged += change;
        }
    }

    public override void deactivate()
    {
        _active = false;
    }
}