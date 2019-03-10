﻿using System.Collections;
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
        if (controller.collisions.faceDir == 1)
        {
            transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
        }
        else
        {
            transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        }

        if (controller.isClimbing)
        {
            animator.SetBool("isClimbing", true);
        }
        else
        {
            animator.SetBool("isClimbing", false);
            if (controller.collisions.below)
            {
                animator.SetBool("isWalking", controller.collisions.isMovingX);
                animator.SetBool("isMidair", false);
            }
            else
            {
                animator.SetBool("isMidair", true);
            }
        }
    }
}