using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObjects : MonoBehaviour
{
    public float triggerDistance = 1.5f;
    private float timeout = 0.0f;
    private bool isDragging = false;


    private PlayerObject _playerObject;
    private InputMaster _inputMaster;
    
    
    public bool IsDragging
    {
        get => isDragging;
        set => isDragging = value;
    }

    void Start()
    {
        _playerObject = GetComponent<PlayerObject>();
        _playerObject.PlayerGUI.ShowTextbox();

        _inputMaster = _playerObject.ThirdPersonMovement.InputMaster;

        _inputMaster.Player.Drag.performed += _ => ToggleDrag();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timeout > 0f)
            timeout -= Time.fixedDeltaTime;
    }

    void ToggleDrag()
    {
        RaycastHit hit;

        // Does the ray intersect any objects excluding the player layer
        if ((Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit,
                 triggerDistance) ||
             Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward * 3 + Vector3.up),
                 out hit, triggerDistance) ||
             Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward * 3 + Vector3.down),
                 out hit, triggerDistance) ||
             Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward * 3 + Vector3.left),
                 out hit, triggerDistance) ||
             Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward * 3 + Vector3.right),
                 out hit, triggerDistance))
            && !isDragging && timeout <= 0f)
        {
            if (hit.rigidbody != null && hit.transform.gameObject.tag == "Dragable")
            {
                hit.transform.position += transform.TransformDirection(Vector3.forward) * (1.1f - hit.distance);
                hit.transform.position += Vector3.up * 0.1f;
                gameObject.AddComponent<CharacterJoint>();
                gameObject.GetComponent<CharacterJoint>().connectedBody = hit.rigidbody;
                isDragging = true;
                timeout = 0.2f;
                EventManager.DragObject.Perform();
            }
        }
        else if (isDragging && timeout <= 0f)
        {
            Destroy(gameObject.GetComponent<CharacterJoint>());
            isDragging = false;
            timeout = 0.2f;
        }
    }
}