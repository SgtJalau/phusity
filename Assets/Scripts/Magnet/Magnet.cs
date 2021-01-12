using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    public enum MagnetColor {Red, Blue};
    public MagnetColor m_magnetColor;

    public Material m_redMaterial;
    public Material m_blueMaterial;

   
    public float m_magnetStrength;
    public float m_maxMagnetStrength = 15;
    protected List<GameObject> m_otherMagnets;

    void Awake() {
        gameObject.tag = "Magnet";
        gameObject.layer = LayerMask.NameToLayer("Magnet");
    }
    
    void Start() {
        m_otherMagnets =  new List<GameObject>();
        if (m_magnetColor == MagnetColor.Red)
        {
            GetComponent<Renderer>().material = m_redMaterial;
        }
        else
        {
            GetComponent<Renderer>().material = m_blueMaterial;
        }
        foreach (GameObject magnet in GameObject.FindGameObjectsWithTag("Magnet")) {
           m_otherMagnets.Add(magnet);
        }

    }
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Magnet"))
        {
            m_otherMagnets.Add(other.gameObject);
        }
    }
    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Magnet"))
        {
           m_otherMagnets.Remove(other.gameObject);
        }
    }

    void FixedUpdate()
    {
        foreach (GameObject other in m_otherMagnets)
        {
            if (Vector3.Distance(other.transform.position, transform.position) <= m_magnetStrength )
            {
                if (other.GetComponent<Magnet>().m_magnetColor == m_magnetColor)
                {
                    other.GetComponent<Rigidbody>().AddForce(Vector3.Normalize( transform.position - other.transform.position) * (Vector3.Distance(transform.position , other.transform.position)) * - Mathf.Abs(m_magnetStrength));
                }
                else
                {
                    other.GetComponent<Rigidbody>().AddForce(Vector3.Normalize( transform.position - other.transform.position)  * (Vector3.Distance(transform.position , other.transform.position)) * Mathf.Abs(m_magnetStrength));
                }
            }
            
           
        }
    }
    
    void Update() {
        foreach (GameObject other in m_otherMagnets)
        {
            if (Vector3.Distance(other.transform.position, transform.position) <= m_magnetStrength )
            {
                Debug.DrawLine(transform.position ,other.transform.position, Color.white); 
            }   
        }
    }

}
