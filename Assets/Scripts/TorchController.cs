using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchController : MonoBehaviour
{
    public CapsuleCollider coll;
    public Transform player;

    public float pickUpRange;
    public float dropForwardForce, dropUpwardForce;

    private bool equipped;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 distanceToPlayer = player.position - transform.position;
        if(!equipped && distanceToPlayer.magnitude <= pickUpRange && Input.GetKeyDown(KeyCode.E))
            PickUpTorch();
        if(equipped && Input.GetKeyDown(KeyCode.Q))
            DropTorch();
    }

    private void PickUpTorch()
    {
        equipped = true;

        Destroy (transform.gameObject.GetComponent<Rigidbody>());

        transform.SetParent(player);
        transform.localPosition = new Vector3(-0.27f, -0.32f, 0.0f);
        transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    private void DropTorch()
    {
        equipped = false;


        transform.SetParent(null);
        transform.gameObject.AddComponent<Rigidbody>();
        Rigidbody rb = transform.gameObject.GetComponent<Rigidbody>();
        rb.velocity = player.gameObject.GetComponent<Rigidbody>().velocity;
        rb.AddForce(transform.TransformDirection(Vector3.forward * dropForwardForce + Vector3.up * dropUpwardForce), ForceMode.Impulse);
    }


}
