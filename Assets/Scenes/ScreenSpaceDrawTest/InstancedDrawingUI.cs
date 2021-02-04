using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstancedDrawingUI : MonoBehaviour
{
    public GameObject crosshairPrefab;

    private GameObject canvas;
    private GameObject[] lastFrameObjects = new GameObject[0];

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Canvas");
        UnityEngine.Assertions.Assert.IsNotNull(canvas);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.G))
        {
            var hits = Physics.OverlapSphere(transform.position, 100.0f, LayerMask.GetMask("RopeTarget"));
            GameObject[] thisFrameObjects = new GameObject[hits.Length];
            for (var i= 0; i < hits.Length; i++)
            {
                var hit = hits[i];
                if (!hit.gameObject.GetComponent<Renderer>().isVisible)
                    continue;
                var createdObj = Object.Instantiate(crosshairPrefab);
                createdObj.transform.SetParent(canvas.transform, false);
                createdObj.transform.position = Camera.main.WorldToScreenPoint(hit.transform.position);
                thisFrameObjects[i] = createdObj;
            }
            foreach (var obj in lastFrameObjects)
                GameObject.Destroy(obj);
            lastFrameObjects = thisFrameObjects;
        }
    }
}
