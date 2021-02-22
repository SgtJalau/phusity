using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BrickWallSpawner : MonoBehaviour
{
    public List<GameObject> brickPrefabs = new List<GameObject>();
    public Vector3 size = new Vector3(0.6f, 0.2f, 0.2f);
    public Vector2Int dimensions = new Vector2Int(6, 5);

    public void createObjects()
    {
        for (int y = 0; y < dimensions.y; y++)
        {
            int unevenRow = y % 2;
            int endIndex = dimensions.x - unevenRow;
            for (int x = 0; x < endIndex; x++)
            {
                Vector3 position = new Vector3(x*size.x, y*size.y, 0);
                if (unevenRow == 1)
                {
                    position.x += size.x * 0.5f;
                }
                Quaternion rotation = Quaternion.identity;
                var res = Instantiate(brickPrefabs[Random.Range(0, brickPrefabs.Count)], transform);
                res.GetComponent<Rigidbody>().mass = 0.1f;
                if (y == 0 || x == 0 || x == endIndex - 1)
                {
                    DestroyImmediate(res.GetComponent<Rigidbody>());
                }
                res.transform.localScale = size;
                res.transform.localPosition = position;
                Transform childTransform = res.transform.GetChild(0);
                childTransform.localRotation = Quaternion.Euler(
                    Random.Range(-2, 3) * 90f,
                    Random.Range(-2, 3) * 90f,
                    Random.Range(-2, 3) * 90f);
            }
        }
    }

    public void clearObjects()
    {
        for (int i = transform.childCount; i > 0; i--)
        { 
            DestroyImmediate(this.transform.GetChild(0).gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody.name == "SteinAnSeil")
        {
            foreach (Transform child in transform)
            {
                var rb = child.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                }
            }
        }
        GetComponent<BoxCollider>().enabled = false;
    }
}

[CustomEditor(typeof(BrickWallSpawner))]
public class BrickWallSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        BrickWallSpawner bwTarget = (BrickWallSpawner)target;
        if (GUILayout.Button("Create"))
        { 
            bwTarget.createObjects();
        }
        if (GUILayout.Button("Clear"))
        {
            bwTarget.clearObjects();
        }
    }
}
