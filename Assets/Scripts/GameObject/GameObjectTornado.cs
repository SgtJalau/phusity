using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectTornado : MonoBehaviour
{
    
    [Tooltip("Distance after which the rotation physics starts")]
    public float maxDistance = 20;

    [Tooltip("The axis that the caught objects will rotate around")]
    public Vector3 rotationAxis = new Vector3(0, 1, 0);

    [Tooltip("Angle that is added to the object's velocity (higher lift -> quicker on top)")]
    [Range(0, 90)]
    public float lift = 45;

    [Tooltip("The force that will drive the caught objects around the tornado's center")]
    public float rotationStrength = 50;

    [Tooltip("Tornado pull force")]
    public float tornadoStrength = 2;

    private Rigidbody _rigibody;

    private List<Rigidbody> _caughtObjects = new List<Rigidbody>();
    
    // Start is called before the first frame update
    void Start()
    {
        //Normalize the rotation axis given by the user
        rotationAxis.Normalize();

        _rigibody = GetComponent<Rigidbody>();
        //_rigibody.isKinematic = true;
    }
    
    void FixedUpdate()
    {

        //Apply force to caught objects
        for (int i = 0; i < _caughtObjects.Count; i++)
        {
            if(_caughtObjects[i] != null)
            {
     
                Vector3 pull = transform.position - _caughtObjects[i].transform.position;
                if (pull.magnitude > maxDistance)
                {
                    _caughtObjects[i].AddForce(pull.normalized * pull.magnitude, ForceMode.Force);
                    //_caughtObjects[i].enabled = false;
                }
                else
                {
                    UpdateCaughtObject(_caughtObjects[i]);
                    //_caughtObjects[i].enabled = true;
                }
            }
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
       
        if (!other.attachedRigidbody)
        {
       
            return;
        }

        if (other.attachedRigidbody.isKinematic)
        {
         
            return;
        }

        //Add caught object to the list
        Rigidbody caught = other.GetComponent<Rigidbody>();
        

        //caught.Init(this, r, tornadoStrength);

        if (!_caughtObjects.Contains(caught))
        {
            Debug.Log("collide4");
            _caughtObjects.Add(caught);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Rigidbody caught = other.GetComponent<Rigidbody>();

        //Release caught object
        if (caught)
        {
            //caught.Release();

            if (_caughtObjects.Contains(caught))
            {
                _caughtObjects.Remove(caught);
                Debug.Log("collide5");
            }
        }
    }

    void UpdateCaughtObject(Rigidbody rigidbody)
    {
        //Rotate object around tornado center
        Vector3 direction = rigidbody.transform.position - transform.position;
        //Project
        Vector3 projection = Vector3.ProjectOnPlane(direction, rotationAxis);
        projection.Normalize();
        Vector3 normal = Quaternion.AngleAxis(130, rotationAxis) * projection;
        normal = Quaternion.AngleAxis(lift, projection) * normal;
        rigidbody.AddForce(normal * rotationStrength, ForceMode.Force);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
