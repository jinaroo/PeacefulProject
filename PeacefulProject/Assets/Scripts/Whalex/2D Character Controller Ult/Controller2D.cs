using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.UIElements;

namespace Whalex
{
    [Serializable]
    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public bool descendingSlope;
        public bool slidingDownMaxSlope;

        public float slopeAngle, slopeAngleOld;
        public Vector2 slopeNormal;
        public Vector2 moveAmountOld;
        public int faceDir;
        public bool fallingThroughPlatform;
        
        public Collider2D curGroundCollider;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;
            slidingDownMaxSlope = false;
            slopeNormal = Vector2.zero;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
    
    public class Controller2D : RaycastController
    {
        public float maxSlopeAngle = 80;

        public CollisionInfo collisions;
        [HideInInspector] public Vector2 playerInput;

        public override void Start()
        {
            base.Start();
            collisions.faceDir = 1;
        }

        public void Move(Vector2 moveAmount, bool standingOnPlatform)
        {
            Move(moveAmount, Vector2.zero, standingOnPlatform);
        }

        public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false)
        {
            UpdateRaycastOrigins();

            collisions.Reset();
            collisions.moveAmountOld = moveAmount;
            playerInput = input;

            if (moveAmount.y < 0)
            {
                DescendSlope(ref moveAmount);
            }

            if (moveAmount.x != 0)
            {
                collisions.faceDir = (int) Mathf.Sign(moveAmount.x);
            }

            // it doesn't require movement on x axis to detect collision for wall sliding
            //if (moveAmount.x != 0)
            {
                HorizontalCollisions(ref moveAmount);
            }

            //if (moveAmount.y != 0)
            {
                VerticalCollisions(ref moveAmount);
            }

            transform.Translate(moveAmount);

            if (standingOnPlatform)
            {
                collisions.below = true;
            }
        }

        void HorizontalCollisions(ref Vector2 moveAmount)
        {
            float directionX = collisions.faceDir;
            float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
            // for wall climbing since the x movement may be zero at that situation
            if (Mathf.Abs(moveAmount.x) < skinWidth)
            {
                rayLength = 2 * skinWidth;
            }

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
                Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

                if (hit)
                {
                    if (hit.distance == 0)
                    {
                        continue;
                    }

                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                    if (i == 0 && slopeAngle <= maxSlopeAngle)
                    {
                        // if it happens to be descending and climbing at the same time, there is a lag 
                        // since descending function will decrease x movement but player will not descend rather climbing at the moment
                        // that's why we restore its previews movement for climbing to work (EP 5 12:39)
                        if (collisions.descendingSlope)
                        {
                            collisions.descendingSlope = false;
                            moveAmount = collisions.moveAmountOld;
                        }

                        float distanceToSlopeStart = 0;
                        bool longEnough = false;
                        if (slopeAngle != collisions.slopeAngleOld)
                        {
                            distanceToSlopeStart = hit.distance - skinWidth;
                            // bug fixed: when you facing up slope and jump on a slope, you will keep moving forwards after landing
                            if (moveAmount.x > skinWidth)
                            {
                                longEnough = true;
                                moveAmount.x -= distanceToSlopeStart * directionX;
                            }
                        }

                        ClimbSlope(ref moveAmount, slopeAngle, hit.normal);
                        if (moveAmount.x == 0)
                            print(longEnough);
                        if (longEnough)
                        {
                            moveAmount.x += distanceToSlopeStart * directionX;
                        }
                    }

                    // you should be able to walk through a one-way platform
                    bool ignoreHorizontalCollider = false;
                    if (hit.collider.CompareTag("Through") && hit.collider != collisions.curGroundCollider)
                        ignoreHorizontalCollider = true;
                    
                    if ((!collisions.climbingSlope || slopeAngle > maxSlopeAngle) && !ignoreHorizontalCollider)
                    {
                        // when player starts jumping on a slope, the x movement should not be changed
                        if (moveAmount.x != 0)
                            moveAmount.x = (hit.distance - skinWidth) * directionX;
                        rayLength = hit.distance;
                        
                        // when you are climbing a slope and run into a wall, you need this chunk to adjust y movement to avoid jagging
                        if (collisions.climbingSlope)
                        {
                            moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                        }

                        collisions.left = directionX == -1;
                        collisions.right = directionX == 1;
                    }
                }
            }
        }

        void VerticalCollisions(ref Vector2 moveAmount)
        {
            float directionY = Mathf.Sign(moveAmount.y);
            float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

                Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

                if (hit)
                {
                    if (hit.collider.tag == "Through")
                    {
                        if (directionY == 1 || hit.distance == 0)
                        {
                            continue;
                        }
                        // adding a amount of time for controller to accelerate in case the platform moves faster than it
                        if (collisions.fallingThroughPlatform)
                        {
                            continue;
                        }
                        if (playerInput.y == -1)
                        {
                            collisions.fallingThroughPlatform = true;
                            Invoke("ResetFallingThroughPlatform", .5f);
                            continue;
                        }
                    }

                    moveAmount.y = (hit.distance - skinWidth) * directionY;
                    rayLength = hit.distance;

                    if (collisions.climbingSlope)
                    {
                        moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) *
                                       Mathf.Sign(moveAmount.x);
                    }
                    
                    collisions.below = directionY == -1;
                    collisions.above = directionY == 1;
                    
                    collisions.curGroundCollider = hit.collider;
                }
            }
            
            if (collisions.climbingSlope)
            {
                float directionX = Mathf.Sign(moveAmount.x);
                rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
                Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) +
                                    Vector2.up * moveAmount.y;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

                if (hit)
                {
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    // avoid lagging when passing by the intersection of two slopes
                    if (slopeAngle != collisions.slopeAngle)
                    {
                        moveAmount.x = (hit.distance - skinWidth) * directionX;
                        collisions.slopeAngle = slopeAngle;
                        collisions.slopeNormal = hit.normal;
                    }
                }
                
                // you can't fall off the platform when you are facing up slope (no downward ray casting)
                for (int i = 0; i < verticalRayCount; i++)
                {
                    rayOrigin = raycastOrigins.bottomLeft + Vector2.right * (verticalRaySpacing * i + moveAmount.x);
                    hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Abs(collisions.moveAmountOld.y) + skinWidth, collisionMask);
                    Debug.DrawRay(rayOrigin, -Vector2.up, Color.red);
                    if (hit)
                    {
                        if (hit.collider.tag == "Through")
                        {
                            // adding a amount of time for controller to accelerate in case the platform moves faster than it
/*                            if (hit.distance == 0)
                            {
                                moveAmount.y = collisions.moveAmountOld.y;
                            }*/
                            if (collisions.fallingThroughPlatform)
                            {
                                moveAmount.y = collisions.moveAmountOld.y;
                            }
                            if (playerInput.y == -1)
                            {
                                collisions.fallingThroughPlatform = true;
                                Invoke("ResetFallingThroughPlatform", .5f);
                                moveAmount.y = collisions.moveAmountOld.y;
                            }
                        }
                    }
                }
            }
        }

        void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal)
        {
            float moveDistance = Mathf.Abs(moveAmount.x);
            float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

            if (moveAmount.y <= climbmoveAmountY)
            {
                moveAmount.y = climbmoveAmountY;
                moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                collisions.below = true;
                collisions.climbingSlope = true;
                collisions.slopeAngle = slopeAngle;
                collisions.slopeNormal = slopeNormal;
            }
            else
            {
                print(moveAmount.x);
            }
        }

        void DescendSlope(ref Vector2 moveAmount)
        {
            RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down,
                Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);
            RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down,
                Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);
            // XOR, to avoid player slides when most of its body is still on flat ground
            if (maxSlopeHitLeft ^ maxSlopeHitRight)
            {
                SlideDownMaxSlope(maxSlopeHitLeft, ref moveAmount);
                SlideDownMaxSlope(maxSlopeHitRight, ref moveAmount);
            }

            if (!collisions.slidingDownMaxSlope)
            {
                float directionX = Mathf.Sign(moveAmount.x);
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

                if (hit)
                {
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
                    {
                        if (Mathf.Sign(hit.normal.x) == directionX)
                        {
                            if (hit.distance - skinWidth <=
                                Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                            {
                                float moveDistance = Mathf.Abs(moveAmount.x);
                                float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                                moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance *
                                               Mathf.Sign(moveAmount.x);
                                moveAmount.y -= descendmoveAmountY;

                                collisions.slopeAngle = slopeAngle;
                                collisions.descendingSlope = true;
                                collisions.below = true;
                                collisions.slopeNormal = hit.normal;
                            }
                        }
                    }
                }
            }
        }

        void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount)
        {
            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle > maxSlopeAngle)
                {
                    moveAmount.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs(moveAmount.y) - hit.distance) /
                                   Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

                    collisions.slopeAngle = slopeAngle;
                    collisions.slidingDownMaxSlope = true;
                    collisions.slopeNormal = hit.normal;
                }
            }
        }

        void ResetFallingThroughPlatform()
        {
            collisions.fallingThroughPlatform = false;
        }
    }
}