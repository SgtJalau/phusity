using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagnetInteraction : MonoBehaviour
{
    private Magnet m_currentMagnet;
    private LayerMask layerMask;

    public Image m_magnetStrengthUI;
    public RectTransform m_magnetStrengthUIRoot;
    public Camera m_camera;

    public float m_maxRayDistance = 2f;
    private void Awake() {
        layerMask = LayerMask.NameToLayer("Magnet");
    }
    private void Update() {
        selectMagnetByRay();

        if (m_currentMagnet != null)
        {
            m_magnetStrengthUIRoot.gameObject.SetActive(true);
            m_magnetStrengthUI.fillAmount = m_currentMagnet.m_magnetStrength / m_currentMagnet.m_maxMagnetStrength ;
        }
        else
        {
            //m_magnetStrengthUIRoot.gameObject.SetActive(false);
        }
    }
    private void selectMagnetByRay()
    {
        Ray ray = m_camera.ViewportPointToRay(Vector3.one / 2f);
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * m_maxRayDistance, Color.red);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, m_maxRayDistance, layerMask))
        {
            var hitItem = hitInfo.collider.GetComponent<Magnet>();
            Debug.Log(hitItem);
            if (hitItem == null)
            {
                m_currentMagnet = null;
            }
            else if (hitItem != null)
            {
                m_currentMagnet = hitItem;
            }
        }
        else
        {
            m_currentMagnet = null;
        }
    }

}
