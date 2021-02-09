using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstancedDrawing : MonoBehaviour
{
    public Mesh mesh;
    public Material material;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.G))
        {
            Vector3 camXWS = Camera.main.transform.right;
            Vector3 camYWS = Camera.main.transform.up;
            var hits = Physics.OverlapSphere(transform.position, 100.0f, LayerMask.GetMask("RopeTarget"));
            //Drawing every target hit. Not doing any culling for now, amount of target in radius should be rather low anyways
            Matrix4x4[] matrices = new Matrix4x4[hits.Length];
            for (var i=0; i<hits.Length; i++)
            {
                //constructing "rotation" Matrix from camera axis in world space
                Matrix4x4 rotMat = Matrix4x4.identity;
                rotMat.SetColumn(1, camYWS);
                rotMat.SetColumn(2, camXWS);
                matrices[i] = 
                    Matrix4x4.Translate(hits[i].gameObject.transform.position) * rotMat;
            }
            Graphics.DrawMeshInstanced(mesh, 0, material, matrices, matrices.Length,
                properties: null,
                castShadows: UnityEngine.Rendering.ShadowCastingMode.Off,
                receiveShadows: false,
                layer: LayerMask.NameToLayer("UI"), //may break stuff, will see when we have actual UI elements
                camera: null,
                lightProbeUsage: UnityEngine.Rendering.LightProbeUsage.Off);
        }
    }
}
