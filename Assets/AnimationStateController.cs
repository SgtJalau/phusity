using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    // Animator Referenz deklarieren
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        //getComponent sucht das GameObject<Animator> im Animator nach Bedingungen ab
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        //Vorwärtsbewegung abfragen
        if (Input.GetKey("w"))
        {
            // isWalking ist die Bedingung --> Walk wird ausgeführt, wenn isWalking == true
            animator.SetBool("isWalking", true);
        }

        // isWalking ist die Bedingung --> Idle wird ausgeführt, wenn isWalking == false
        if (!Input.GetKey("w"))
        {
            animator.SetBool("isWalking", false);
        }
    }
}
