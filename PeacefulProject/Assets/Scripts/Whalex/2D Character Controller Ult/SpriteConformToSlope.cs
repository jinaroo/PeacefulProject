using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Whalex
{
    public class SpriteConformToSlope : MonoBehaviour
    {
        public Controller2D controller;
        public float rotationSmoothing = 0.5f;
        public float rotationSpeed = 10f;

        [SerializeField] private float heightAdjustment = 0f;
        [SerializeField] private Vector3 tgtUp;
        [SerializeField] private Vector3 tgtLocalPos;
        [SerializeField] private float angle;
        
        private SpriteRenderer sRend;
        private LayerMask layerMask;

        void Start()
        {
            layerMask = controller.collisionMask;
            sRend = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            if (controller.collisions.below &&
                (controller.collisions.climbingSlope || controller.collisions.descendingSlope))
            {
                RaycastHit2D hit = Physics2D.Raycast(controller.transform.position, -controller.collisions.slopeNormal,
                    3f, layerMask);
                if (hit)
                {
                    heightAdjustment = hit.distance - (sRend.size.y / 2f);
                }
                tgtUp = controller.collisions.slopeNormal;
                tgtLocalPos = new Vector3(0f, -heightAdjustment, 0f);
            }
            else
            {
                heightAdjustment = 0f;
                tgtUp = Vector3.up;
                tgtLocalPos = Vector3.zero;
            }
            
            //angle = Vector2.Angle(transform.up, tgtUp);
            //transform.Rotate(0,0,angle); 
            transform.up += (tgtUp - transform.up) * rotationSmoothing * rotationSpeed * Time.deltaTime;
            transform.localPosition += (tgtLocalPos - transform.localPosition) * rotationSmoothing * rotationSpeed *
                                       Time.deltaTime;
        }
    }
}