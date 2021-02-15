using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{

    void Update()
    {
        transform.Rotate(90 * Time.deltaTime, 0, 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player")
        {
            other.GetComponent<ThirdPersonMovement>().points++;
            Destroy(gameObject);
        }
    }
}
