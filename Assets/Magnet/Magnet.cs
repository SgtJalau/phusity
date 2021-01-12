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
    protected List<Collider> m_otherMagnets;
    protected SphereCollider m_trigger;

    void Awake() {
        gameObject.tag = "Magnet";
        gameObject.layer = LayerMask.NameToLayer("Magnet");
    }
    
    void Start() {
        m_otherMagnets =  new List<Collider>();
        m_trigger = gameObject.AddComponent<SphereCollider>();
        if (m_magnetColor == MagnetColor.Red)
        {
            GetComponent<Renderer>().material = m_redMaterial;
        }
        else
        {
            GetComponent<Renderer>().material = m_blueMaterial;
        }
         
        m_trigger.isTrigger = true;
        m_trigger.radius = m_magnetStrength;

    }
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Magnet"))
        {
            m_otherMagnets.Add(other);
        }
    }
    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Magnet"))
        {
           m_otherMagnets.Remove(other);
        }
    }
    private void OnCollisionEnter(Collision other) {
        if (other.collider.CompareTag("Magnet"))
        {
           m_otherMagnets.Remove(other.collider);
        }
    }
    private void OnCollisionExit(Collision other) {
         if (other.collider.CompareTag("Magnet"))
        {
           m_otherMagnets.Add(other.collider);
        }
    }
    void FixedUpdate()
    {
        foreach (Collider other in m_otherMagnets)
        {
            if (other.gameObject.GetComponent<Magnet>().m_magnetColor == m_magnetColor)
            {
                other.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.Normalize( transform.position - other.transform.position) * (1.0f / Vector3.Distance(transform.position , other.transform.position)) * - Mathf.Abs(m_magnetStrength));
            }
            else
            {
                other.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.Normalize( transform.position - other.transform.position)  * (1.0f / Vector3.Distance(transform.position , other.transform.position)) * Mathf.Abs(m_magnetStrength));
            }
           
        }
    }
    
    void Update() {
        foreach (Collider other in m_otherMagnets)
        {
            Debug.DrawLine(transform.position ,other.transform.position, Color.white);    
        }
    }
}
