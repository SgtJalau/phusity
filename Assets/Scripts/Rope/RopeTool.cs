using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RopeToolState
{
    SELECTED_NONE,
    SELECTED_DYNAMIC,
    SELECTED_STATIC
}

public class RopeTool : MonoBehaviour
{   
    public Material distanceMat;
    
    public GameObject ropePrefab;

    public GameObject linkPrefab;
    private Vector3 linkSize;

    GameObject debugSphere = null;

    LayerMask ropeTargetLayerMask;

    RopeToolState toolState = RopeToolState.SELECTED_NONE;
    RaycastHit hit1;
    RopeTarget hit1Target;
    Transform hit1TargetTransform;
    RaycastHit hit2;
    RopeTarget hit2Target;
    Transform hit2TargetTransform;

    SoftJointLimitSpring limSpring;
    SoftJointLimit limLin;

    private InputMaster _input;

    public RopeToolState getState()
    {
        return toolState;
    }

    // Start is called before the first frame update
    void Start()
    {

        ropeTargetLayerMask = LayerMask.GetMask("RopeTarget");

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

        linkSize = linkPrefab.GetComponent<MeshRenderer>().bounds.size;
    }

    void ShootRope()
    {
        Camera cam = Camera.main;
        Vector3 wPos = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth*0.5f, cam.pixelHeight*0.5f, 0));
        RaycastHit result;
        if (Physics.SphereCast(origin: wPos, 0.1f, transform.forward, out result, Mathf.Infinity, ropeTargetLayerMask))
        {
            bool hitDynamic = result.collider.CompareTag("DynamicRopeTarget");
            //hit1 should be the static target, hit2 the dynamic (or static) one
            if (toolState == RopeToolState.SELECTED_NONE)
            {
                if (hitDynamic)
                {
                    hit2 = result;
                    toolState = RopeToolState.SELECTED_DYNAMIC;
                }
                else
                {
                    hit1 = result;
                    toolState = RopeToolState.SELECTED_STATIC;
                }
            }
            else
            {
                if (toolState == RopeToolState.SELECTED_DYNAMIC)
                {
                    if (hitDynamic)
                    {
                        return;
                    }
                    hit1 = result;
                }
                else
                {
                    hit2 = result;
                }

                /* Test if its possible to connect the two hits */
                //dont connect Object to itself
                if (hit1.transform == hit2.transform)
                {
                    return;
                }
                hit1TargetTransform = hit1.transform.Find("TargetPosition");
                Vector3 p1 = hit1TargetTransform.position;
                hit2TargetTransform = hit2.transform.Find("TargetPosition");
                Vector3 p2 = hit2TargetTransform.position;
                //Is something obstructing the connection between the two points?
                //have to test for all in case a target also defines a magnet (or similar), ie it contains another collider with a different layer
                RaycastHit[] hits = Physics.RaycastAll(
                    p1, (p2 - p1).normalized, (p2 - p1).magnitude, ~(ropeTargetLayerMask | LayerMask.GetMask("StaticRope"))); //maybe SphereCastAll
                foreach(var hit in hits)
                {
                    if (hit.rigidbody != hit1.rigidbody && hit.rigidbody != hit2.rigidbody)
                    {
                        return;
                    }
                }

                /* Destroy active connection if exists */
                //unsure if ...InChildern is needed given the script should be attached to the highest level object (?)
                hit1Target = hit1.transform.gameObject.GetComponentInChildren<RopeTarget>();
                hit2Target = hit2.transform.gameObject.GetComponentInChildren<RopeTarget>();
                if (hit1Target.activeConnection != null)
                {
                    hit1Target.activeConnection.DestroyConnection();
                }
                if (hit2Target.activeConnection != null)
                {
                    hit2Target.activeConnection.DestroyConnection();
                }

                /* Now actually create the Rope */
                if (hitDynamic || toolState == RopeToolState.SELECTED_DYNAMIC)
                {
                    createDynamicRope();
                }
                else
                {
                    createStaticRope();
                }
                toolState = RopeToolState.SELECTED_NONE;
            }
        }
    }

    void createStaticRope()
    {

        Vector3 p1 = hit1TargetTransform.position;
        Vector3 p2 = hit2TargetTransform.position;

        StaticConnection newConnection = new StaticConnection();
            
        GameObject rope = Instantiate(ropePrefab, (p1+p2) * 0.5f, Quaternion.LookRotation(p2 - p1, Vector3.up));
        rope.transform.localScale = new Vector3(1, 1, (p2-p1).magnitude);

        newConnection.connection = rope;
        newConnection.end1 = hit1Target;
        newConnection.end2 = hit2Target;
        hit1Target.activeConnection = newConnection;
        hit2Target.activeConnection = newConnection;

    }

    void createDynamicRope()
    {
        Transform staticTransform = hit1TargetTransform;
        Transform dynamicTransform = hit2TargetTransform;
        Vector3 staticPoint = staticTransform.position;
        Vector3 dynamicPoint = dynamicTransform.position;
        Vector3 directionNorm = (dynamicPoint - staticPoint).normalized;

        ROPE_TYPE type = hit2Target.type;

        if (type == ROPE_TYPE.DYNAMIC_DISTANCE)
        {
            DistanceConnection newConnection = new DistanceConnection();

            SpringJoint joint = hit1.transform.gameObject.AddComponent<SpringJoint>();
            joint.connectedBody = hit2.rigidbody;
            joint.autoConfigureConnectedAnchor = false;
            joint.anchor = staticTransform.localPosition;
            joint.connectedAnchor = dynamicTransform.localPosition;
            joint.spring = 10000;
            joint.damper = 1.0f;
            joint.enableCollision = true;
            joint.minDistance = 0.0f;
            joint.maxDistance = hit2Target.autoConfigureMaxLength ? (staticPoint-dynamicPoint).magnitude : hit2Target.maxLength;

            GameObject renderEmpty = new GameObject("DistanceConnectionRenderEmpty");
            LineRenderer renderer = renderEmpty.AddComponent<LineRenderer>();
            renderer.material = distanceMat;
            DistanceConnectionRenderer renderUpdater = renderEmpty.AddComponent<DistanceConnectionRenderer>();
            renderUpdater.lineRenderer = renderer;
            renderUpdater.t1 = staticTransform;
            renderUpdater.t2 = dynamicTransform;

            newConnection.end1 = hit1Target;
            newConnection.end2 = hit2Target;
            newConnection.springJoint = joint;
            newConnection.rendererEmpty = renderEmpty;
            hit1Target.activeConnection = newConnection;
            hit2Target.activeConnection = newConnection;
        }
        else
        {

            var emptyForStorage = new GameObject("Link Empty");

            LinkConnection newConnection = new LinkConnection();
            newConnection.emptyWithLinks = emptyForStorage;
            
            float linkOffset = (linkSize.y-0.2f)* hit2Target.linkScale;
            int amountOfLinks = Mathf.RoundToInt((staticPoint - dynamicPoint).magnitude / linkOffset);
            float trueLength = (staticPoint - dynamicPoint).magnitude / amountOfLinks;
            float scale = trueLength / (linkSize.y-0.2f);

            Rigidbody[] links = new Rigidbody[amountOfLinks]; //TODO: dont actually need full array, only most recent, last & first link
            GameObject lastLink = null;

            //connections between links
            for (int i = 0; i < amountOfLinks; i++)
            {
                GameObject link = GameObject.Instantiate(linkPrefab);
                link.GetComponent<MeshRenderer>().lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off; //TODO: no real fix but hides annoying widget in UI
                link.transform.localScale = new Vector3(scale, scale, scale);
                link.transform.position = staticPoint + (i + 0.5f) * trueLength * directionNorm;
                link.transform.rotation = Quaternion.FromToRotation(Vector3.up, directionNorm);
                if (i % 2 == 1)
                {
                    link.transform.rotation *= Quaternion.FromToRotation(Vector3.forward, Vector3.right);
                }

                //hide in hierarchy by parenting to empty
                link.transform.parent = emptyForStorage.transform;
                link.layer = LayerMask.NameToLayer("RopeLink");

                var rb = link.AddComponent<Rigidbody>();
                rb.collisionDetectionMode = hit2Target.collisionMode;
                rb.mass = hit2Target.linkMass;
                links[i] = rb;
                if (i != 0)
                {
                    //var joint = link.AddComponent<CharacterJoint>();
                    var joint = lastLink.AddComponent<ConfigurableJoint>();
                    joint.connectedBody = rb;
                    joint.anchor = 0.1375f * Vector3.up; //0.1375 is result of 'joint0.anchor = links[0].transform.InverseTransformPoint(staticPoint);' (check in inspector)
                    applyJointPreset(ref joint);
                }
                lastLink = link;
            }
            //connection to static target
            //var joint0 = links[0].gameObject.AddComponent<CharacterJoint>();
            var joint0 = hit1.transform.gameObject.AddComponent<ConfigurableJoint>();
            joint0.connectedBody = links[0];
            joint0.anchor = joint0.transform.Find("TargetPosition").localPosition;
            applyJointPreset(ref joint0);
            newConnection.startJoint = joint0;

            //connection to dynamic target
            //var jointN = hit2.transform.gameObject.AddComponent<CharacterJoint>();
            var jointN = links[amountOfLinks-1].gameObject.AddComponent<ConfigurableJoint>();
            jointN.connectedBody = hit2.rigidbody;
            jointN.anchor = links[amountOfLinks - 1].transform.InverseTransformPoint(dynamicPoint);
            applyJointPreset(ref jointN);
            newConnection.endJoint = jointN;

            //avoid stretching by adding additional stiff spring joint from start to end
            if (hit2Target.addDistanceConstraint)
            {
                SpringJoint springJoint = hit1.transform.gameObject.AddComponent<SpringJoint>();
                springJoint.connectedBody = hit2.rigidbody;
                springJoint.autoConfigureConnectedAnchor = false;
                springJoint.anchor = staticTransform.localPosition;
                springJoint.connectedAnchor = dynamicTransform.localPosition;
                springJoint.spring = 1000;
                springJoint.damper = Mathf.Infinity;
                springJoint.enableCollision = true;
                springJoint.minDistance = 0.0f;
                springJoint.maxDistance = (dynamicPoint - staticPoint).magnitude;
            }

            newConnection.end1 = hit1Target;
            newConnection.end2 = hit2Target;
            hit1Target.activeConnection = newConnection;
            hit2Target.activeConnection = newConnection;
        }
    }

    void applyJointPreset(ref ConfigurableJoint joint)
    {
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        joint.enablePreprocessing = hit2Target.enablePrePro;
        joint.projectionMode = hit2Target.enableProjection ? 
            JointProjectionMode.PositionAndRotation : JointProjectionMode.None;
    }

    void applyJointPreset(ref CharacterJoint joint)
    {
        //TODO: store as member of class dont alawys construct
        joint.highTwistLimit = new SoftJointLimit
        {
            bounciness = 0.0f,
            contactDistance = 0.0f,
            limit = 177
        };
        joint.lowTwistLimit = new SoftJointLimit
        {
            bounciness = 0.0f,
            contactDistance = 0.0f,
            limit = -177
        };
        joint.swing1Limit = new SoftJointLimit
        {
            bounciness = 0.0f,
            contactDistance = 0.0f,
            limit = 177
        };
        joint.swing2Limit = new SoftJointLimit
        {
            bounciness = 0.0f,
            contactDistance = 0.0f,
            limit = 177
        };
        joint.enableProjection = true;
    }

    void Awake()
    {
        _input = new InputMaster();
        _input.Player.Interact.performed += _ => ShootRope(); 
    }

    void OnEnable()
    {
        _input.Player.Enable();
    }

    void OnDisable()
    {
        _input.Player.Disable();
    }
}
