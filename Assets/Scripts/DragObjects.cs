using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObjects : MonoBehaviour
{
    public float triggerDistance = 1.5f;
    private float timeout = 0.0f;
    private bool isDragging = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        if(timeout > 0f)
            timeout -= Time.fixedDeltaTime;
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if ((Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, triggerDistance) || 
            Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward * 3 + Vector3.up), out hit, triggerDistance) || 
            Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward * 3 + Vector3.down), out hit, triggerDistance) || 
            Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward * 3 + Vector3.left), out hit, triggerDistance) ||
            Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward * 3 + Vector3.right), out hit, triggerDistance))
            && !isDragging && timeout <= 0f)
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward * 3 + Vector3.up).normalized * hit.distance, Color.red);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward * 3 + Vector3.down).normalized * hit.distance, Color.red);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward * 3 + Vector3.right).normalized * hit.distance, Color.red);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward * 3 + Vector3.left).normalized * hit.distance, Color.red);

            if (hit.rigidbody != null && hit.transform.gameObject.tag == "Dragable" && Input.GetKey(KeyCode.E)){
                hit.transform.position += transform.TransformDirection(Vector3.forward) * (1.1f-hit.distance);
                hit.transform.position += Vector3.up * 0.1f;
                gameObject.AddComponent<CharacterJoint>(); 
                gameObject.GetComponent<CharacterJoint>().connectedBody = hit.rigidbody;
                isDragging = true;
                timeout = 0.2f;
            }
        } 
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * triggerDistance, Color.yellow);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward * 3 + Vector3.up).normalized * triggerDistance, Color.yellow);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward * 3 + Vector3.down).normalized * triggerDistance, Color.yellow);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward * 3 + Vector3.right).normalized  * triggerDistance, Color.yellow);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward * 3 + Vector3.left).normalized  * triggerDistance, Color.yellow);
            if (isDragging && Input.GetKey(KeyCode.E) && timeout <= 0f){
                Destroy(gameObject.GetComponent<CharacterJoint>());
                isDragging = false;
                timeout = 0.2f;
            }
        }
    }
}
