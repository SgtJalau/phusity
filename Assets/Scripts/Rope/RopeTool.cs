﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//If performance problems come up then its probably better to refactor the whole code

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

    public GameObject linkPrefab;
    private Vector3 linkSize;
    public float linkScale = 1.0f;

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

        linkSize = linkPrefab.GetComponent<MeshRenderer>().bounds.size;
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

        Vector3 p1 = hit1.transform.Find("TargetPosition").position;
        Vector3 p2 = hit2.transform.Find("TargetPosition").position;

        //TODO: raycast not 100% reliable
        if (!Physics.Raycast(p1, p2, Mathf.Infinity, ~layerMask)) //maybe SphereCast
        {
            //TODO: unsure if ...InChildern is needed given the script should be attached to the highest level object (?)
            RopeTarget target1 = hit1.transform.gameObject.GetComponentInChildren<RopeTarget>();
            RopeTarget target2 = hit2.transform.gameObject.GetComponentInChildren<RopeTarget>();
            if (target1.activeConnection != null)
            {
                target1.activeConnection.DestroyConnection();
            }
            if (target2.activeConnection != null)
            {
                target2.activeConnection.DestroyConnection();
            }

            StaticConnection newConnection = new StaticConnection();
            
            GameObject rope = Instantiate(ropePrefab, (p1+p2) * 0.5f, Quaternion.LookRotation(p2 - p1, Vector3.up));
            rope.transform.localScale = new Vector3(1, 1, (p2-p1).magnitude);

            newConnection.connection = rope;
            newConnection.end1 = target1;
            newConnection.end2 = target2;
            target1.activeConnection = newConnection;
            target2.activeConnection = newConnection;
        }

    }

    void createDynamicRope()
    {
        //TODO: also raycast here to test if connection even possible
        //TODO: either: snap to target when current distance > specified distance (currently)
        //      or:     dont even allow connection

        Transform staticTransform = hit1.transform.Find("TargetPosition");
        Transform dynamicTransform = hit2.transform.Find("TargetPosition");
        Vector3 staticPoint = staticTransform.position;
        Vector3 dynamicPoint = dynamicTransform.position;
        Vector3 directionNorm = (dynamicPoint - staticPoint).normalized;

        //TODO: unsure if ...InChildern is needed given the script should be attached to the highest level object (?)
        RopeTarget target1 = hit1.transform.gameObject.GetComponentInChildren<RopeTarget>();
        RopeTarget target2 = hit2.transform.gameObject.GetComponentInChildren<RopeTarget>();
        //delete existing connection
        if (target1.activeConnection != null)
        {
            target1.activeConnection.DestroyConnection();
        }
        if (target2.activeConnection != null)
        {
            target2.activeConnection.DestroyConnection();
        }
        ROPE_TYPE type = target2.type;

        if (type == ROPE_TYPE.DYNAMIC_DISTANCE)
        {
            DistanceConnection newConnection = new DistanceConnection();

            SpringJoint joint = hit1.transform.gameObject.AddComponent<SpringJoint>();
            joint.connectedBody = hit2.rigidbody;
            joint.autoConfigureConnectedAnchor = false;
            joint.anchor = staticTransform.localPosition;
            joint.connectedAnchor = dynamicTransform.localPosition;
            joint.spring = 1000;
            joint.damper = 1.0f;
            joint.enableCollision = true;
            joint.minDistance = 0.0f;
            joint.maxDistance = target2.maxLength;

            GameObject renderEmpty = new GameObject("DistanceConnectionRenderEmpty");
            LineRenderer renderer = renderEmpty.AddComponent<LineRenderer>();
            DistanceConnectionRenderer renderUpdater = renderEmpty.AddComponent<DistanceConnectionRenderer>();
            renderUpdater.lineRenderer = renderer;
            renderUpdater.t1 = staticTransform;
            renderUpdater.t2 = dynamicTransform;

            newConnection.end1 = target1;
            newConnection.end2 = target2;
            newConnection.springJoint = joint;
            newConnection.rendererEmpty = renderEmpty;
            target1.activeConnection = newConnection;
            target2.activeConnection = newConnection;
        }
        else
        {

            var emptyForStorage = new GameObject("Link Empty");

            LinkConnection newConnection = new LinkConnection();
            newConnection.emptyWithLinks = emptyForStorage;
            
            float linkOffset = (linkSize.y-0.2f)*linkScale;
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
                rb.mass = 1f;
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

            newConnection.end1 = target1;
            newConnection.end2 = target2;
            target1.activeConnection = newConnection;
            target2.activeConnection = newConnection;
        }
    }

    void applyJointPreset(ref ConfigurableJoint joint)
    { 
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        //joint.xMotion = ConfigurableJointMotion.Limited;
        //joint.yMotion = ConfigurableJointMotion.Limited;
        //joint.zMotion = ConfigurableJointMotion.Limited;
        //joint.linearLimitSpring = limSpring; //TODO: rework for Links if needed -> less bounce and tolerance
        //joint.linearLimit = limLin;
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
}
