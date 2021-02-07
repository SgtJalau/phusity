﻿using System.Collections;
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

    private RopeTool ropeTool;

    private InputMaster _input;

    void Awake()
    {
        _input = new InputMaster();
        _input.Player.TargetMode.performed += _ => toggleStatus();
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
        ropeTool = Camera.main.GetComponent<RopeTool>();
        Assert.IsNotNull(ropeTool);
    }

    private void toggleStatus()
    {
        globalPostProcessVolume.profile.TryGet<Vignette>(out Vignette vig);
        vig.active = !vig.active;
        vcam.Priority += priorityBoost;
        priorityBoost *= -1;
        modeActive = !modeActive;
    }

    void Update()
    {
        if (modeActive)
        {
            Vector3 camXWS = Camera.main.transform.right;
            Vector3 camYWS = Camera.main.transform.up;

            //--------------------------- "FINDING" TARGETS AND DRAWING OVERLAY ELEMENT FOR EACH --------------------------------//

            //test if we are currently aiming at a target
            Camera cam = Camera.main;
            Vector3 wPos = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth * 0.5f, cam.pixelHeight * 0.5f, 0));
            RaycastHit result;
            Collider hitCollider = null;
            //TODO: could store last hit and pass it to the RopeTool somehow -> dont need to SphereCast twice
            if (Physics.SphereCast(origin: wPos, 0.1f, transform.forward, out result, Mathf.Infinity, LayerMask.GetMask("RopeTarget")))
            {
                hitCollider = result.collider;
            }

            var hits = Physics.OverlapSphere(transform.position, 100.0f, LayerMask.GetMask("RopeTarget"));
            Matrix4x4[] matrices = new Matrix4x4[hits.Length];
            int amount = 0;
            for (var i = 0; i < hits.Length; i++)
            {
                RopeToolState currentState = ropeTool.getState();
                if (currentState == RopeToolState.SELECTED_NONE ||
                    currentState == RopeToolState.SELECTED_STATIC ||
                    (currentState == RopeToolState.SELECTED_DYNAMIC && !hits[i].CompareTag("DynamicRopeTarget")))
                {
                    //unlike raycasthit.transform, sphereoverlap returns the colliders own. 
                    //Ie we have to get the parent containing the rigidbody and its transform ourselves
                    GameObject topmostGameObject = hits[i].attachedRigidbody.gameObject;

                    //constructing "rotation" Matrix from camera axis in world space
                    Matrix4x4 rotMat = Matrix4x4.identity;
                    rotMat.SetColumn(1, camYWS);
                    rotMat.SetColumn(2, camXWS);
                    if (hits[i] == hitCollider)
                    { 
                        rotMat *= Matrix4x4.Rotate(Quaternion.Euler(Time.time*360, 0, 0));
                    }
                    //the following can be avoided if we require the "TargetPosition" Empty to not also define the target collider
                    Transform targetTransform = topmostGameObject.transform.Find("TargetPosition");

                    matrices[amount] = Matrix4x4.Translate(targetTransform.position) * rotMat;
                    amount++;
                }
            }
            Graphics.DrawMeshInstanced(simpleQuad, 0, ropeTargetMat, matrices, amount,
                properties: null,
                castShadows: UnityEngine.Rendering.ShadowCastingMode.Off,
                receiveShadows: false,
                layer: LayerMask.NameToLayer("UI"), //may break stuff, will see when we have actual UI elements
                camera: null,
                lightProbeUsage: UnityEngine.Rendering.LightProbeUsage.Off);
        }
    }
}
