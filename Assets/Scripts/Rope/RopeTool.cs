using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ROPE_STATE
{
    NONE,
    ONE_STATIC,
    ONE_DYNAMIC
}

public class RopeTool : MonoBehaviour
{
    public Camera cam;
    public Material mat;
    public GameObject ropePrefab;

    GameObject debugSphere = null;

    LayerMask layerMask;
    ROPE_STATE state = ROPE_STATE.NONE;
    RaycastHit hit1;
    RaycastHit hit2;
    SpringJoint activeJoint = null;

    // Start is called before the first frame update
    void Start()
    {
        layerMask = LayerMask.GetMask("RopeTarget");

        debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(debugSphere.GetComponent<Collider>());
        debugSphere.transform.localScale = new Vector3(0.3f,0.3f,0.3f);
        debugSphere.GetComponent<Renderer>().material = mat;
        debugSphere.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 wPos = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth*0.5f, cam.pixelHeight*0.5f, 0));
            RaycastHit result;
            if (Physics.SphereCast(origin: wPos, 0.1f, transform.forward, out result, Mathf.Infinity, layerMask))
            {
                bool hitDynamic = result.collider.CompareTag("DynamicRopeTarget");
                switch (state)
                {
                    case ROPE_STATE.NONE:
                        hit1 = result;
                        state = hitDynamic ? ROPE_STATE.ONE_DYNAMIC : ROPE_STATE.ONE_STATIC;
                        break;
                    case ROPE_STATE.ONE_STATIC:
                        hit2 = result;
                        if (hitDynamic)
                        {
                            createDynamicRope();
                        }
                        else
                        {
                            createStaticRope();   
                        }
                        state = ROPE_STATE.NONE;
                        break;
                    case ROPE_STATE.ONE_DYNAMIC:
                        if (hitDynamic)
                        {
                            state = ROPE_STATE.NONE;
                        }
                        else
                        {
                            hit2 = hit1;
                            hit1 = result;
                            createDynamicRope();
                        }
                        state = ROPE_STATE.NONE;
                        break;
                }
                debugSphere.SetActive(true);
                debugSphere.transform.position = result.point;
            }
        }
    }

    void createStaticRope()
    {
        if (!Physics.Raycast(hit1.point, hit2.point, Mathf.Infinity, ~layerMask)) //maybe SphereCast
        {
            GameObject rope = Instantiate(ropePrefab, (hit1.point + hit2.point) * 0.5f, Quaternion.LookRotation(hit2.point - hit1.point, Vector3.up));
            rope.transform.localScale = new Vector3(1, 1, (hit2.point - hit1.point).magnitude);
        }
    }

    void createDynamicRope()
    {
        if (activeJoint != null) //destroy currently active connection
        {
            Destroy(activeJoint);
        }
        GameObject dynamicObjectRoot = hit2.transform.gameObject;
        activeJoint = dynamicObjectRoot.AddComponent<SpringJoint>();
        activeJoint.connectedBody = hit1.rigidbody; //TODO: could be null (although all rope targets should have a rigidbody)
        activeJoint.autoConfigureConnectedAnchor = false;
        activeJoint.connectedAnchor = Vector3.zero;
        activeJoint.anchor = dynamicObjectRoot.transform.GetChild(2).localPosition; //TODO: Dont just refernce by hierarchy index
        activeJoint.spring = 100;
        activeJoint.damper = 1.0f;
        activeJoint.minDistance = 0.0f;
        activeJoint.maxDistance = (hit1.point - hit2.point).magnitude+1; //TODO: offset as parameter?
    }
}
