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
            //Debug.Log(m_currentMagnet);
            m_magnetStrengthUIRoot.gameObject.SetActive(true);
            m_magnetStrengthUI.fillAmount = m_currentMagnet.m_magnetStrength / m_currentMagnet.m_maxMagnetStrength ;
             if (Input.GetKeyDown(KeyCode.O))
            {
                m_currentMagnet.DecrementMagnetStrength();
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                m_currentMagnet.IncrementMagnetStrength();
            }
        }
        else
        {
            m_magnetStrengthUIRoot.gameObject.SetActive(false);
        }
       
    }
    private void selectMagnetByRay()
    {
        Ray ray = m_camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        Ray playerRay = new Ray(transform.position, ray.direction);
        Debug.DrawRay(playerRay.origin, ray.direction * m_maxRayDistance, Color.red);
        RaycastHit hitInfo;
        if (Physics.Raycast(playerRay, out hitInfo, layerMask))
        {
            var hitItem = hitInfo.collider.GetComponent<Magnet>();

            if (hitItem == null)
            {
                m_currentMagnet = null;
            }
            else if (hitItem != null)
            {
                if (Vector3.Distance(hitItem.transform.position, transform.position) <= m_maxRayDistance)
                {
                    m_currentMagnet = hitItem;
                }
            }
        }
        else
        {
            m_currentMagnet = null;
        }
    }
}
