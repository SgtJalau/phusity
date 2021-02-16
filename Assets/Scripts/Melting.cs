using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melting : MonoBehaviour
{
    // Start is called before the first frame update
    public float meltingSpeed = 0.001f;
    public GameObject iceBlock;
    private bool melting = false;
    private bool isMolten = false;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Torch" && !isMolten){
            melting = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Torch" && !isMolten){
            melting = false;
        }
    }

    void Update(){
        if (melting && !isMolten)
        {
            Debug.Log("melting");
            iceBlock.transform.localScale *= 1 - meltingSpeed * Time.deltaTime;
            iceBlock.GetComponent<Rigidbody>().mass *= 1 - meltingSpeed * Time.deltaTime;
            if(iceBlock.transform.localScale.x <= 0.1 || iceBlock.transform.localScale.y <= 0.1 || iceBlock.transform.localScale.z <= 0.1)
                isMolten = true;
        }
    }
}
