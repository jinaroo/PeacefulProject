﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class GrabObjectsTemporary : MonoBehaviour
    {
        public float grabDst;
        public LayerMask grabMask;

        private Controller2D controller;
        private Rigidbody2D rg2d;
        public Transform grabRightPoint, grabLeftPoint;

        private bool isHolding;
        private GameObject holdingObject;
        private RigidbodyType2D previewsType;
        private float currentDir, previewDir;

        public float releaseForce = 10f;
        
        private void OnEnable()
        {
            //EventManager.Instance.StartListening("ObjCollect",PutDownObj);
            EventManagerNew.Instance.Register<CollectEvent>(PutDownObj);
        }

        private void OnDisable()
        {
            //EventManager.Instance.StopListening("ObjCollect",PutDownObj);
            EventManagerNew.Instance.Unregister<CollectEvent>(PutDownObj);
        }

        private void PutDownObj(CollectEvent myEvent)
        {
            holdingObject.transform.SetParent(myEvent.collectorTf);
            holdingObject.transform.localPosition = Vector3.zero;
            holdingObject.transform.localRotation = Quaternion.identity;
            holdingObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            holdingObject.GetComponent<Rigidbody2D>().angularVelocity = 0f;
            holdingObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            holdingObject.GetComponent<Collider2D>().isTrigger = true;

            
            // change its layer so it won't be picked up by player again
            holdingObject.layer = LayerMask.NameToLayer("Placed");
            holdingObject = null;
            isHolding = false;
        }

        private void Start()
        {
            controller = GetComponent<Controller2D>();
            rg2d = GetComponent<Rigidbody2D>();
            // assigning these in the inspector so we can change where they are in the hierarchy 
            // they are now children of the sprite, not the root player object
            //grabRightPoint = transform.Find("GrabRightPoint");
            //grabLeftPoint = transform.Find("GrabLeftPoint");
            currentDir = controller.collisions.faceDir;
            previewDir = controller.collisions.faceDir;
        }

        private void Update()
        {
            previewDir = currentDir;
            currentDir = controller.collisions.faceDir;

            if (Input.GetKeyDown(KeyCode.J) && isHolding == false)
            {
                for (int i = 0; i < controller.horizontalRayCount; i++)
                {
                    Vector2 rayOrigin = (currentDir == -1)
                        ? controller.raycastOrigins.bottomLeft
                        : controller.raycastOrigins.bottomRight;
                    rayOrigin += Vector2.up * (controller.horizontalRaySpacing * i);
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * currentDir, grabDst, grabMask);
                    Debug.DrawRay(rayOrigin, Vector2.right * currentDir * grabDst, Color.yellow);

                    if (hit)
                    {
                        holdingObject = hit.collider.gameObject;
                        
                        holdingObject.transform.SetParent((currentDir == -1) ? grabLeftPoint : grabRightPoint);
                        holdingObject.transform.localPosition = Vector3.zero;
                        holdingObject.transform.localRotation = Quaternion.identity;
                        holdingObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                        holdingObject.GetComponent<Rigidbody2D>().angularVelocity = 0f;
                        holdingObject.GetComponent<Collider2D>().isTrigger = true;
                        previewsType = holdingObject.GetComponent<Rigidbody2D>().bodyType;
                        holdingObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                        isHolding = true;
                        break;
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.J) && isHolding)
            {
                holdingObject.transform.SetParent(null);
                holdingObject.GetComponent<Collider2D>().isTrigger = false;
                holdingObject.GetComponent<Rigidbody2D>().bodyType = previewsType;
                holdingObject.GetComponent<Rigidbody2D>().velocity = controller.collisions.moveAmountOld * releaseForce;
                holdingObject = null;
                isHolding = false;
            }

            DetectFlip();
        }

        private void DetectFlip()
        {
            if (!isHolding) return;

            if (currentDir != previewDir)
            {
                holdingObject.transform.SetParent((currentDir == -1) ? grabLeftPoint : grabRightPoint);
                holdingObject.transform.localPosition = Vector3.zero;
                holdingObject.transform.localRotation = Quaternion.identity;
            }
        }
    }