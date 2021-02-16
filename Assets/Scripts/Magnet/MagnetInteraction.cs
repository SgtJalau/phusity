using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class MagnetInteraction : MonoBehaviour
{
    private Magnet m_currentMagnet;
    private LayerMask layerMask;

    public Image m_magnetStrengthUI;
    public RectTransform m_magnetStrengthUIRoot;
    public Camera m_camera;

    private InputMaster _input;

    public GameObject m_strengthIndicator;
    public GameObject m_targetingCam;

    private void Awake() {
        layerMask = LayerMask.NameToLayer("Magnet");
        m_magnetStrengthUIRoot.gameObject.SetActive(false);
        m_targetingCam = GameObject.Find("CM_TargetingCam");
        _input = new InputMaster();
        _input.Player.MagnetPower.performed += ChangeMagnetStrength;
    }
    private void OnEnable()
    {
        _input.Player.Enable();
    }

    private void OnDisable()
    {
        _input.Player.Disable();
    }

    private void ChangeMagnetStrength(InputAction.CallbackContext context)
    {
        if (m_currentMagnet != null)
        {
            float direction = context.ReadValue<float>();
            if (direction == 1)
            {
                m_currentMagnet.IncrementMagnetStrength();
            }
            else{
                m_currentMagnet.DecrementMagnetStrength();
            }
            
        }
    }

    private void Update() {
        if (m_targetingCam.GetComponent<TargetingActivator>().focusedObject != null)
        {
            m_currentMagnet = m_targetingCam.GetComponent<TargetingActivator>().focusedObject.GetComponent<Magnet>();
        }
        else
        {
            m_currentMagnet = null;
        }
        if (m_currentMagnet != null)
        {
            m_strengthIndicator.transform.position = m_currentMagnet.transform.position;
            m_strengthIndicator.transform.localScale = new Vector3(m_currentMagnet.m_magnetRange,m_currentMagnet.m_magnetRange,m_currentMagnet.m_magnetRange);
            m_strengthIndicator.SetActive(true);
            m_magnetStrengthUIRoot.gameObject.SetActive(true);
            m_magnetStrengthUI.fillAmount = m_currentMagnet.m_magnetStrength / m_currentMagnet.m_maxMagnetStrength ;
        }
        else
        {
            m_magnetStrengthUIRoot.gameObject.SetActive(false);
            m_strengthIndicator.SetActive(false);
        }
       
    }
}
