﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Whalex
{
    public class GrabObjects : MonoBehaviour
    {
        public float grabDst;
        public LayerMask grabMask;

        private Controller2D controller;
        private Rigidbody2D rg2d;
        private Transform grabRightPoint, grabLeftPoint;

        private bool isHolding;
        private GameObject holdingObject;
        private RigidbodyType2D previewsType;
        private float currentDir, previewDir;

        private void Start()
        {
            controller = GetComponent<Controller2D>();
            rg2d = GetComponent<Rigidbody2D>();
            grabRightPoint = transform.Find("GrabRightPoint");
            grabLeftPoint = transform.Find("GrabLeftPoint");
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
                        holdingObject.GetComponent<Collider2D>().enabled = false;
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
                holdingObject.GetComponent<Collider2D>().enabled = true;
                holdingObject.GetComponent<Rigidbody2D>().bodyType = previewsType;
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
}