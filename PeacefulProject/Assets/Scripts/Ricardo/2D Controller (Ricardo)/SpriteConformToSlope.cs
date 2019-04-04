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
    public Vector3 tgtLocalPos;

    public float rotationSmoothing = 0.5f;
    public float rotationSpeed = 10f;

    public Transform spriteTransform;

    public float ignoreBumpTime = 0.05f;
    private float timeToStopIgnoringSlope;

    public float climbAngleAdjustment = 90f;
    public float climbHeightAdjustment = 0.35f;
    
    int layerMask = 1 << 8;

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
            tgtUp = Quaternion.AngleAxis(climbAngleAdjustment * -climbingDir, Vector3.forward) * tgtUp;
            tgtLocalPos += Vector3.right * climbHeightAdjustment * climbingDir;
        }
        else
        {
            climbingDir = -10;
        }
        
        if (Time.timeSinceLevelLoad >= timeToStopIgnoringSlope)
        {
            transform.up += (tgtUp - transform.up) * rotationSmoothing * rotationSpeed * Time.deltaTime;
            spriteTransform.localPosition += (tgtLocalPos - spriteTransform.localPosition) * rotationSmoothing * rotationSpeed * Time.deltaTime;
        }
    }
}
