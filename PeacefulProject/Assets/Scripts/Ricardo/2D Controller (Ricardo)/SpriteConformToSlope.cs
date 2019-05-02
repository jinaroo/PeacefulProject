using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D;

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

    public float climbAngleAdjustment = 90f;
    public float climbHeightAdjustment = 0.35f;
    public float climbHeightSlopeModifier = 3f;
    
    public LayerMask layerMask;

    private int climbingDir = -10;

    private Collider2D oldClimbingCollider;

    public SpriteRenderer headSprite;
    public SpriteRenderer bodySprite;
    public SpriteRenderer tailSprite;
    public SpriteRenderer arm1Sprite;
    public SpriteRenderer arm2Sprite;
    public SpriteRenderer leg1Sprite;
    public SpriteRenderer leg2Sprite;
    
    private int headWalkingOrder;
    private int bodyWalkingOrder;
    private int tailWalkingOrder;
    private int arm1WalkingOrder;
    private int arm2WalkingOrder;
    private int leg1WalkingOrder;
    private int leg2WalkingOrder;
    
    public int headClimbingOrderOffset;
    public int bodyClimbingOrderOffset;
    public int tailClimbingOrderOffset;
    public int arm1ClimbingOrderOffset;
    public int arm2ClimbingOrderOffset;
    public int leg1ClimbingOrderOffset;
    public int leg2ClimbingOrderOffset;
    
    // Start is called before the first frame update
    void Start()
    {
        sRend = GetComponent<SpriteRenderer>();

        headWalkingOrder = headSprite.sortingOrder;
        bodyWalkingOrder = bodySprite.sortingOrder;
        tailWalkingOrder = tailSprite.sortingOrder;
        arm1WalkingOrder = arm1Sprite.sortingOrder;
        arm2WalkingOrder = arm2Sprite.sortingOrder;
        leg1WalkingOrder = leg1Sprite.sortingOrder;
        leg2WalkingOrder = leg2Sprite.sortingOrder;
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


        
        
        if (controller.isClimbing)
        {
            if (controller.collisions.curGroundCollider != oldClimbingCollider)
            {
                //Debug.Log("hitting a new collider");
                oldClimbingCollider = controller.collisions.curGroundCollider;
                SpriteShapeRenderer spriteShapeRend = oldClimbingCollider.GetComponent<SpriteShapeRenderer>();
                SpriteRenderer spriteRend = oldClimbingCollider.GetComponent<SpriteRenderer>();

                int curObstacleSortOrder = 45;
                if (spriteShapeRend)
                {
                    curObstacleSortOrder = spriteShapeRend.sortingOrder;
                } else if (spriteRend)
                {
                    curObstacleSortOrder = spriteRend.sortingOrder;
                }
                
                SwitchToClimbingOrders(curObstacleSortOrder);
            }
            if (climbingDir == -10)
            {
                climbingDir = controller.collisions.faceDir;
            }
            modifiedTgtUp = Quaternion.AngleAxis(climbAngleAdjustment * -climbingDir, Vector3.forward) * tgtUp;
            //modifiedTgtUp = tgtUp;
            modifiedTgtLocalPos = tgtLocalPos + Vector3.right * (climbHeightAdjustment + tgtLocalPos.magnitude * climbHeightSlopeModifier * climbHeightAdjustment) * climbingDir;
        }
        else
        {
            if (climbingDir != -10)
            {
                SwitchToWalkingOrders();
                oldClimbingCollider = null;
            }
            
            climbingDir = -10;
            modifiedTgtUp = tgtUp;
            modifiedTgtLocalPos = tgtLocalPos;
        }


        

        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.LookRotation(Vector3.forward, modifiedTgtUp), rotationSmoothing * rotationSpeed * Time.deltaTime);
        spriteTransform.localPosition += (modifiedTgtLocalPos - spriteTransform.localPosition) * rotationSmoothing * rotationSpeed * Time.deltaTime;
    }

    void SwitchToClimbingOrders(int obstacleLayer)
    {
        Debug.Log("switching to climbing orders");
        bodySprite.sortingOrder = obstacleLayer + bodyClimbingOrderOffset;
        headSprite.sortingOrder = obstacleLayer + headClimbingOrderOffset;
        tailSprite.sortingOrder = obstacleLayer + tailClimbingOrderOffset;
        arm1Sprite.sortingOrder = obstacleLayer + arm1ClimbingOrderOffset;
        arm2Sprite.sortingOrder = obstacleLayer + arm2ClimbingOrderOffset;
        leg1Sprite.sortingOrder = obstacleLayer + leg1ClimbingOrderOffset;
        leg2Sprite.sortingOrder = obstacleLayer + leg2ClimbingOrderOffset;
    }

    void SwitchToWalkingOrders()
    {
        bodySprite.sortingOrder = bodyWalkingOrder;
        headSprite.sortingOrder = headWalkingOrder;
        tailSprite.sortingOrder = tailWalkingOrder;
        arm1Sprite.sortingOrder = arm1WalkingOrder;
        arm2Sprite.sortingOrder = arm2WalkingOrder;
        leg1Sprite.sortingOrder = leg1WalkingOrder;
        leg2Sprite.sortingOrder = leg2WalkingOrder;
    }
}
