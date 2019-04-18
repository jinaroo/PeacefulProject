using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteConformToSlope : MonoBehaviour
{
    public Controller2D controller;
    private SpriteRenderer sRend;

    public float heightAdjustment = 0f;

    private Vector3 tgtUp;
    private Vector3 oldTgtUp;
    private Vector3 modifiedTgtUp;
    
    private Vector3 tgtLocalPos;
    private Vector3 modifiedTgtLocalPos;
    

    public float rotationSmoothing = 0.5f;
    public float rotationSpeed = 10f;

    public Transform spriteTransform;

    public float ignoreBumpTime = 0.05f;
    private float timeToStopIgnoringSlope;

    public float climbAngleAdjustment = 90f;
    public float climbHeightAdjustment = 0.35f;
    
    public LayerMask layerMask;

    private int climbingDir = -10;
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
            RaycastHit2D[] hits = Physics2D.RaycastAll(controller.transform.position, -controller.collisions.slopeNormal, 1.5f, layerMask);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider == controller.collisions.curGroundCollider && hit.distance > 0f)
                {
                    heightAdjustment = hit.distance - (sRend.size.y / 2f);
                    tgtUp = controller.collisions.slopeNormal;
                    tgtLocalPos = new Vector3(0f, -heightAdjustment, 0f);
                    break;
                }
            }
        }
        else
        {
            heightAdjustment = 0f;
            tgtUp = Vector3.up;
            tgtLocalPos = Vector3.zero;
        }
        
        if (oldTgtUp != tgtUp)
        {
            timeToStopIgnoringSlope = Time.timeSinceLevelLoad + ignoreBumpTime;
            oldTgtUp = tgtUp;
        }
        
        if (controller.isClimbing)
        {
            if (climbingDir == -10)
            {
                climbingDir = controller.collisions.faceDir;
            }
            modifiedTgtUp = Quaternion.AngleAxis(climbAngleAdjustment * -climbingDir, Vector3.forward) * tgtUp;
            //modifiedTgtUp = tgtUp;
            modifiedTgtLocalPos = tgtLocalPos + Vector3.right * climbHeightAdjustment * climbingDir;
        }
        else
        {
            climbingDir = -10;
            modifiedTgtUp = tgtUp;
            modifiedTgtLocalPos = tgtLocalPos;
        }


        
        if (Time.timeSinceLevelLoad >= timeToStopIgnoringSlope)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.LookRotation(Vector3.forward, modifiedTgtUp), rotationSmoothing * rotationSpeed * Time.deltaTime);
            spriteTransform.localPosition += (modifiedTgtLocalPos - spriteTransform.localPosition) * rotationSmoothing * rotationSpeed * Time.deltaTime;
        }
    }
}
