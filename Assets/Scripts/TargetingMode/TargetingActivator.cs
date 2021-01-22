using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
//needed for the vignette, PP is pipeline specific
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class TargetingActivator : MonoBehaviour
{
    public KeyCode keyToActive = KeyCode.F;
    public int priorityBoost = 10;
    public Volume globalPostProcessVolume;

    Cinemachine.CinemachineVirtualCameraBase vcam;
    bool boosted = false;

    void Start()
    {
        vcam = GetComponent<Cinemachine.CinemachineVirtualCameraBase>();
        Assert.IsNotNull(vcam);
    }

    void Update()
    {
        if (Input.GetKeyDown(keyToActive))
        {
            if (!boosted)
            {
                globalPostProcessVolume.profile.TryGet<Vignette>(out Vignette vig);
                vig.active = true;
                vcam.Priority += priorityBoost;
                boosted = true;
            }
            else
            {
                globalPostProcessVolume.profile.TryGet<Vignette>(out Vignette vig);
                vig.active = false;
                vcam.Priority -= priorityBoost;
                boosted = false;
            }
        }
        else
        {
            if (boosted)
            {
                var hits = Physics.OverlapSphere(Camera.main.transform.position, 100.0f, ~LayerMask.NameToLayer("RopeTarget"));
                foreach (var hit in hits)
                {
                    Debug.DrawLine(hit.transform.position - 0.5f*Vector3.up, hit.transform.position + 0.5f*Vector3.up, Color.cyan);
                }
            }
        }
    }
}
