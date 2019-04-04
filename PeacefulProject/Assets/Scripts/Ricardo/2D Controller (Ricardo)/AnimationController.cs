using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Controller2D controller;
    public Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!controller.isClimbing)
        {
            if (controller.collisions.faceDir == 1)
            {
                transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
            }
            else
            {
                transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            }
        }


        
        animator.SetBool("isClimbing", controller.isClimbing);
        animator.SetBool("isRising", controller.isRising);
        animator.SetBool("isFalling", controller.isFalling);
        animator.SetBool("isWalking", controller.isWalking);
    }
}
