using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public List<PlatformPoint> vectors;
    private float _lastDistance = 0f;
    private int _currentIndex = 0;
    private Vector3 _currentAbsoluteTargetPoint;
    private Vector3 _startPosition;
    private Transform _transform;

    /**
     * Should the movement of the platform be reversed/mirrored after reaching last point
     */
    public bool mirrorMovement = false;

    /**
     * Are we currently reversing?
     */
    private bool _inReverse = false;

    private float currentWait = 0;

    void Awake()
    {
        this._transform = transform;
        this._startPosition = _transform.position;
        CalculateTargetPoint();
    }

    // Update is called once per fixed frame (framerate independent)
    private void FixedUpdate()
    {
        if (currentWait > 0)
        {
            currentWait -= Time.deltaTime;
            return;
        }
        
        var currentPoint = GETCurrentPlatformPoint();

        //If we are currently reversing reverse direction
        float reverse = _inReverse ? -1 : 1;

        //Move Platform using speed and reverse direction, speed in meter/s
        _transform.Translate(Vector3.Normalize(currentPoint.vector3) * ((currentPoint.speed * Time.deltaTime) * reverse));
        
        //Check if we are moving away from target if so we reached the target position
        if (Vector3.Distance(_transform.localPosition, _currentAbsoluteTargetPoint) > _lastDistance)
        {
            GoToNextPoint();
        }

        _lastDistance = Vector3.Distance(_transform.localPosition, _currentAbsoluteTargetPoint);
    }


    private PlatformPoint GETCurrentPlatformPoint()
    {
        //If we are reversing we want to be one point ahead for the reverse direction than our current index
        return _inReverse? GETPlatformPoint(_currentIndex + 1) : GETPlatformPoint(_currentIndex);
    }

    private PlatformPoint GETPlatformPoint(int index)
    {
        //-1 == start point for reversing
        return index == -1 ? vectors[0] : vectors[index];
    }

    private void GoToNextPoint()
    {
 
        
        if (_inReverse)
        {
            //Reset if we would go below start point
            if (_currentIndex == -1)
            {
                _inReverse = false;
                _currentIndex = 0;
            }
            else
            {
                //If we are reversing currently go back one point
                _currentIndex--;
            }
        }
        else
        {
            currentWait = GETCurrentPlatformPoint().waitSecondsAfter;
            
            //Reset if we would go above index
            if (_currentIndex == vectors.Count - 1)
            {
                if (mirrorMovement)
                {
                    _inReverse = true;
                    
                    //Go back one point
                    _currentIndex--;
                }
                else
                {
                    _currentIndex = 0;
                    _transform.localPosition = _startPosition;
                }
            }
            else
            {
                _currentIndex++;
            }
        }
        
        CalculateTargetPoint();
    }

    private void CalculateTargetPoint()
    {
        //Clone so we don't modify initial point
        var initialPoint = new Vector3(_startPosition.x, _startPosition.y, _startPosition.z);

        //Add all points up to the current index for absolute position
        for (var x = 0; x <= _currentIndex; x++)
        {
            initialPoint += vectors[x].vector3;
        }

        _currentAbsoluteTargetPoint = initialPoint;

        //Recalculate needed distance
        _lastDistance = Vector3.Distance(_transform.localPosition, _currentAbsoluteTargetPoint);
    }
}

[Serializable]
public class PlatformPoint
{
    public Vector3 vector3;
    public float speed = 1f;
    public int waitSecondsAfter = 0;

    public PlatformPoint(float x, float y, float z, float speed, int waitSecondsAfter)
    {
        this.vector3 = new Vector3(x, y, z);
        this.speed = speed;
        this.waitSecondsAfter = waitSecondsAfter;
    }
}