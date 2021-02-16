using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerObject : MonoBehaviour
{
    
    private CinemachineBrain _virtualCamera;

    void Start()
    {
        _virtualCamera = Camera.main.GetComponent<CinemachineBrain>();
        Assert.IsNotNull(_virtualCamera);
    }
        
    
    public bool GetTargetCollider(float maxDistance, out Collider targetCollider)
    {
        RaycastHit hit;

        var raysToCheck = new List<Vector3>
        {
            transform.TransformDirection(Vector3.forward),
            transform.TransformDirection(Vector3.forward * 3 + Vector3.up),
            transform.TransformDirection(Vector3.forward * 3 + Vector3.down),
            transform.TransformDirection(Vector3.forward * 3 + Vector3.left),
            transform.TransformDirection(Vector3.forward * 3 + Vector3.right),
            transform.TransformDirection(Vector3.forward * 3 + Vector3.down * 2),
            transform.TransformDirection(Vector3.forward * 3 + Vector3.down + Vector3.left * 2),
            transform.TransformDirection(Vector3.forward * 3 + Vector3.down + Vector3.right * 2)
        };

        foreach (Vector3 rayDirection in raysToCheck)
        {
            Debug.DrawRay(transform.position, rayDirection, Color.green);

            if (Physics.Raycast(transform.position, rayDirection, out hit, maxDistance,
                LayerMask.GetMask("Targetable")))
            {
                targetCollider = hit.collider;
                return true;
            }
        }

        targetCollider = null;
        return false;
    }
    
    public IEnumerator LookAtLocation(Transform vector3, long millis)
    {
        Transform previousFollow = _virtualCamera.ActiveVirtualCamera.Follow;
        Transform previousLookAt = _virtualCamera.ActiveVirtualCamera.LookAt;

        //_virtualCamera.OutputCamera.transform.LookAt(vector3);
        _virtualCamera.ActiveVirtualCamera.Follow = vector3;
        _virtualCamera.ActiveVirtualCamera.LookAt = vector3;


        yield return new WaitForSeconds(millis / 1000F);

        _virtualCamera.ActiveVirtualCamera.Follow = previousFollow;
        _virtualCamera.ActiveVirtualCamera.LookAt = previousLookAt;
    }
}