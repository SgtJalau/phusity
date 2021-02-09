using System;
using UnityEngine;

public class HighlightableActivator : Activator
{
    protected bool _active = false;

    private Color _realColor;
    private Renderer _renderer;
    
    public Mesh mesh;
    public Material material;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _realColor = _renderer.material.color;
    }

    public void Update()
    {
        if (_active)
        {
            Vector3 camXWS = Camera.main.transform.right;
            Vector3 camYWS = Camera.main.transform.up;

            //Drawing every target hit. Not doing any culling for now, amount of target in radius should be rather low anyways
            Matrix4x4[] matrices = new Matrix4x4[1];
            //constructing "rotation" Matrix from camera axis in world space
            Matrix4x4 rotMat = Matrix4x4.identity;
            rotMat.SetColumn(1, camYWS);
            rotMat.SetColumn(2, camXWS);
            matrices[0] = Matrix4x4.Translate(gameObject.transform.position) * rotMat;

            Graphics.DrawMeshInstanced(mesh, 0, material, matrices, matrices.Length,
                properties: null,
                castShadows: UnityEngine.Rendering.ShadowCastingMode.Off,
                receiveShadows: false,
                layer: LayerMask.NameToLayer("UI"), //may break stuff, will see when we have actual UI elements
                camera: null,
                lightProbeUsage: UnityEngine.Rendering.LightProbeUsage.Off);
        }
    }

    public void Highlight()
    {
        this._active = true;
        _renderer.material.color = Color.yellow;
    }

    public void Restore()
    {
        this._active = false;
        _renderer.material.color = _realColor;
    }
}