using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ROPE_STATE
{
    NONE,
    ONE_STATIC,
    ONE_DYNAMIC,
    ACTIVE
}

public class RopeTool : MonoBehaviour
{
    public Camera cam;
    public Material mat;
    public GameObject ropePrefab;

    LayerMask layerMask;
    GameObject debugSphere = null;
    ROPE_STATE state = ROPE_STATE.NONE;
    Vector3 p1;
    Vector3 p2;

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
        //while(Input.GetMouseButton(0))
        {
            Vector3 wPos = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth*0.5f, cam.pixelHeight*0.5f, 0));
            RaycastHit result;
            if (Physics.SphereCast(origin: wPos, 0.1f, transform.forward, out result, Mathf.Infinity, layerMask))
            {
                switch (state)
                {
                    case ROPE_STATE.NONE:
                        p1 = result.point;
                        state = ROPE_STATE.ONE_STATIC;
                        break;
                    case ROPE_STATE.ONE_STATIC:
                        p2 = result.point;
                        if (!Physics.Raycast(p1, p2, Mathf.Infinity, ~layerMask)) //maybe SphereCast
                        {
                            GameObject rope = Instantiate(ropePrefab, (p1 + p2) * 0.5f, Quaternion.LookRotation(p2 - p1, Vector3.up));
                            rope.transform.localScale = new Vector3(1, 1, (p2 - p1).magnitude);
                        }
                        state = ROPE_STATE.NONE;
                        break;
                }
                debugSphere.SetActive(true);
                debugSphere.transform.position = result.point;
            }
        }
    }
}
