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
    public float linkSize = 0.05f;

    GameObject debugSphere = null;

    LayerMask layerMask;
    ROPE_STATE state = ROPE_STATE.NONE;
    RaycastHit hit1;
    RaycastHit hit2;

    SoftJointLimitSpring limSpring;
    SoftJointLimit limLin;

    // Start is called before the first frame update
    void Start()
    {
        layerMask = LayerMask.GetMask("RopeTarget");

        debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(debugSphere.GetComponent<Collider>());
        debugSphere.transform.localScale = new Vector3(0.3f,0.3f,0.3f);
        debugSphere.GetComponent<Renderer>().material = mat;
        debugSphere.SetActive(false);

        limSpring = new SoftJointLimitSpring
        {
            damper = 1.0f,
            spring = 1000f
        };
        limLin = new SoftJointLimit
        {
            limit = 0.01f,
            bounciness = 0.0f
        };
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
        var emptyForStorage = new GameObject("Link Empty");

        Vector3 staticPoint = hit1.transform.position;
        Vector3 dynamicPoint = hit2.transform.GetChild(2).position; //TODO: dont rely on GetChild(2) to check if the target empty exists
        Vector3 directionNorm = (dynamicPoint - staticPoint).normalized;
        
        int amountOfLinks = Mathf.RoundToInt((staticPoint - dynamicPoint).magnitude / (2*linkSize));
        float trueLength = (staticPoint - dynamicPoint).magnitude / amountOfLinks;

        Rigidbody[] links = new Rigidbody[amountOfLinks];

        //connections between links
        for (int i = 0; i < amountOfLinks; i++)
        {
            GameObject link = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            link.GetComponent<MeshRenderer>().lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off; //TODO: no real fix but hides annoying widget in UI
            link.transform.localScale = new Vector3(linkSize, trueLength*0.5f, linkSize); //*0.5f since base capsule is 2 unity high
            link.transform.position = staticPoint + (i+0.5f)*trueLength*directionNorm;
            link.transform.rotation = Quaternion.FromToRotation(Vector3.up, directionNorm);
            
            link.transform.parent = emptyForStorage.transform; //hide in hierarchy by parenting to empty
            link.layer = LayerMask.NameToLayer("RopeLink");

            var rb = link.AddComponent<Rigidbody>();
            rb.mass = 0.1f;
            links[i] = rb;
            if (i != 0)
            {
                var joint = link.AddComponent<ConfigurableJoint>();
                joint.connectedBody = links[i - 1];
                joint.anchor = -1f*Vector3.up;
                //joint.autoConfigureConnectedAnchor = false; //enabling both lines should result in the same behaviour
                //joint.connectedAnchor = Vector3.up;
                applyJointPreset(ref joint);
            }
        }
        //connection to static target
        var joint0 = links[0].gameObject.AddComponent<ConfigurableJoint>();
        joint0.connectedBody = hit1.rigidbody;
        joint0.anchor = -1f * Vector3.up;
        //joint0.autoConfigureConnectedAnchor = false;
        //joint0.connectedAnchor = Vector3.zero;
        applyJointPreset(ref joint0);

        //connection to dynamic target
        var jointN = hit2.transform.gameObject.AddComponent<ConfigurableJoint>();
        jointN.connectedBody = links[amountOfLinks-1];
        jointN.anchor = jointN.transform.GetChild(2).localPosition;
        applyJointPreset(ref jointN);
    }

    //Sets all joint properties excluding anchor properties
    void applyJointPreset(ref ConfigurableJoint joint)
    {
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        //joint.xMotion = ConfigurableJointMotion.Limited;
        //joint.yMotion = ConfigurableJointMotion.Limited;
        //joint.zMotion = ConfigurableJointMotion.Limited;
        //joint.linearLimitSpring = limSpring;
        //joint.linearLimit = limLin;
    }
}
