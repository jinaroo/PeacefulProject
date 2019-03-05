using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour {

    public float minGroundNormalY = .65f;
    public float gravityModifier = 1f;

    [SerializeField] protected Vector2 targetVelocity;
    [SerializeField] protected bool grounded;
    protected Vector2 groundNormal;
    protected Rigidbody2D rb2d;
    [SerializeField] protected Vector2 velocity;
    [SerializeField] private float projection;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D> (16);

    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;

    void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D> ();
    }

    void Start () 
    {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask (Physics2D.GetLayerCollisionMask (gameObject.layer));
        contactFilter.useLayerMask = true;
    }
    
    void Update () 
    {
        targetVelocity = Vector2.zero;
        ComputeVelocity (); 
    }

    protected virtual void ComputeVelocity()
    {
    
    }

    void FixedUpdate()
    {
        velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
        velocity.x = targetVelocity.x;

        grounded = false;

        Vector2 deltaPosition = velocity * Time.deltaTime;

        Vector2 moveAlongGround = new Vector2 (groundNormal.y, -groundNormal.x);

        // slope won't affect your "horizontal" speed
        // the problem is if player is not on the ground, it will still walk along previous ground which may cause unwanted results
        Vector2 move = moveAlongGround * deltaPosition.x;
        Movement (move, false);

        move = Vector2.up * deltaPosition.y;
        Movement (move, true);
    }

    void Movement(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;

        //prevents object to check collision constantly when still
        if (distance > minMoveDistance) 
        {
            int count = rb2d.Cast (move, contactFilter, hitBuffer, distance + shellRadius);
            hitBufferList.Clear ();
            for (int i = 0; i < count; i++) {
                hitBufferList.Add (hitBuffer [i]);
            }

            for (int i = 0; i < hitBufferList.Count; i++) 
            {
                Vector2 currentNormal = hitBufferList [i].normal;
                //if the surface is flat enough to stand upon
                if (currentNormal.y > minGroundNormalY) 
                {
                    grounded = true;
                    //if moving down the slope, it will not set current normal x to zero
                    //so that if you run into an up slope it will affect your horizontal speed
                    //though the speed will be reset next frame which means it won't affect the movement essentially
                    if (yMovement) 
                    {
                        groundNormal = currentNormal;
                        //x value is set to zero so that the ground will only affect velocity on y axis
                        currentNormal.x = 0;
                    }
                }

                projection = Vector2.Dot (velocity, currentNormal);
                if (projection < 0) 
                {
                    velocity = velocity - projection * currentNormal;
                }

                float modifiedDistance = hitBufferList [i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }
        rb2d.position = rb2d.position + move.normalized * distance;
    }

}