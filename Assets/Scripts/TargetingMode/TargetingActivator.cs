using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Assertions;
//needed for the vignette, PP is pipeline specific
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class TargetingActivator : MonoBehaviour
{
    public int priorityBoost = 10;
    public Volume globalPostProcessVolume;

    public Mesh simpleQuad;
    public Material ropeTargetMat;

    Cinemachine.CinemachineVirtualCameraBase vcam;
    bool modeActive = false;

    private InputMaster _input;

    void Awake()
    {
        _input = new InputMaster();
        _input.Player.ToggleTargeting.performed += _ => toggleStatus();
    }

    private void OnEnable()
    {
        _input.Player.Enable();
    }

    private void OnDisable()
    {
        _input.Player.Disable();
    }

    void Start()
    {
        vcam = GetComponent<Cinemachine.CinemachineVirtualCameraBase>();
        Assert.IsNotNull(vcam);
    }

    private void toggleStatus()
    {
        if (!modeActive)
        {
            globalPostProcessVolume.profile.TryGet<Vignette>(out Vignette vig);
            vig.active = true;
            vcam.Priority += priorityBoost;
            modeActive = true;
        }
        else
        {
            globalPostProcessVolume.profile.TryGet<Vignette>(out Vignette vig);
            vig.active = false;
            vcam.Priority -= priorityBoost;
            modeActive = false;
        }
    }

    void Update()
    {
        if (modeActive)
        {
            Vector3 camXWS = Camera.main.transform.right;
            Vector3 camYWS = Camera.main.transform.up;

            //--------------------------- "FINDING" TARGETS AND DRAWING OVERLAY ELEMENT FOR EACH --------------------------------//
            var hits = Physics.OverlapSphere(transform.position, 100.0f, LayerMask.GetMask("RopeTarget"));
            //Drawing every target hit. Not doing any culling for now, amount of targets in radius should be rather low anyways
            Matrix4x4[] matrices = new Matrix4x4[hits.Length];
            for (var i = 0; i < hits.Length; i++)
            {
                //constructing "rotation" Matrix from camera axis in world space
                Matrix4x4 rotMat = Matrix4x4.identity;
                rotMat.SetColumn(1, camYWS);
                rotMat.SetColumn(2, camXWS);
                //the following can be avoided if we require the "TargetPosition" Empty to not also define the target collider
                Transform targetTransform = hits[i].name == "TargetPosition" ? hits[i].transform : hits[i].transform.Find("TargetPosition");
                matrices[i] =
                    Matrix4x4.Translate(targetTransform.position) * rotMat;
            }
            Graphics.DrawMeshInstanced(simpleQuad, 0, ropeTargetMat, matrices, matrices.Length,
                properties: null,
                castShadows: UnityEngine.Rendering.ShadowCastingMode.Off,
                receiveShadows: false,
                layer: LayerMask.NameToLayer("UI"), //may break stuff, will see when we have actual UI elements
                camera: null,
                lightProbeUsage: UnityEngine.Rendering.LightProbeUsage.Off);
        }
    }
}
