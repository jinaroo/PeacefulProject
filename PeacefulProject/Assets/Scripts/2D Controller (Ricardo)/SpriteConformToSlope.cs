﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteConformToSlope : MonoBehaviour
{
    public Controller2D controller;
    private SpriteRenderer sRend;

    public float heightAdjustment = 0f;

    private Vector3 tgtUp;
    public Vector3 tgtLocalPos;

    public float rotationSmoothing = 0.5f;
    public float rotationSpeed = 10f;
    
    int layerMask = 1 << 8;
    // Start is called before the first frame update
    void Start()
    {
        sRend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.collisions.below && (controller.collisions.climbingSlope || controller.collisions.descendingSlope))
        {
            RaycastHit2D hit = Physics2D.Raycast(controller.transform.position, -controller.collisions.slopeNormal, 3f, layerMask);
            if (hit)
            {
                heightAdjustment = hit.distance - (sRend.size.y / 2f);
                //Debug.Log(hit.distance);
                //Debug.Log("height " + sRend.size.y);
//                if (hit.collider == controller.collisions.curGroundCollider)
//                {
//                    heightAdjustment = hit.distance - (sRend.bounds.size.y / 2f);
//                    Debug.Log(hit.distance);
//                }
//                else
//                {
//                    Debug.Log("not hitting the right ground collider!");
//                    Debug.Log("we're hitting " + hit.collider);
//                    Debug.Log("we're looking for " + controller.collisions.curGroundCollider);
//                }
            }
            //Debug.Log("not hitting any ground collider!");

            tgtUp = controller.collisions.slopeNormal;
            tgtLocalPos = new Vector3(0f, -heightAdjustment, 0f);
            //tgtLocalPos = (controller.collisions.slopeNormal.normalized * (sRend.size.y / 2f));
            //Debug.Log(hit.point);
        }
        else
        {
            heightAdjustment = 0f;
            tgtUp = Vector3.up;
            tgtLocalPos = Vector3.zero;
        }
            



        transform.up += (tgtUp - transform.up) * rotationSmoothing * rotationSpeed * Time.deltaTime;
        transform.localPosition += (tgtLocalPos - transform.localPosition) * rotationSmoothing * rotationSpeed * Time.deltaTime;



    }
}