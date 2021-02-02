using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotRender : MonoBehaviour
{

    private Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer.enabled = false;
    }

    

    void Awake()
    {
        renderer = GetComponent<Renderer>();
        
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
