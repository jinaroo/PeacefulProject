using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D;

public class GrabObjects : MonoBehaviour
    {
        public float grabDst;
        public LayerMask grabMask;

        private Controller2D controller;
        private Rigidbody2D rg2d;
        public Transform grabRightPoint, grabLeftPoint;
        public AudioManager audioManager;

        private bool isHolding;
        private GameObject holdingObject;
        private RigidbodyType2D previousType;
        private float currentDir, previousDir;

        public float releaseForce = 10f;
        public int holdingSortOrder = 90;
        private int previousSortOrder;

        public float pickupAndGrabVolume = 0.75f;
        
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
            
            if(audioManager)
                audioManager.PlaySoundEffect(audioManager.Clips.dropItem, pickupAndGrabVolume);

            
            // change its layer so it won't be picked up by player again
            holdingObject.layer = LayerMask.NameToLayer("Placed");
            holdingObject = null;
            isHolding = false;
            if (holdingObject.GetComponent<SpriteRenderer>())
            {
                holdingObject.GetComponent<SpriteRenderer>().sortingOrder = previousSortOrder;
            } else if (holdingObject.GetComponent<SpriteShapeRenderer>())
            {
                holdingObject.GetComponent<SpriteShapeRenderer>().sortingOrder = previousSortOrder;
            }
            
            controller.isHolding = false;
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
            previousDir = controller.collisions.faceDir;

            if (audioManager == null)
            {
                GameObject audioManagerObj = GameObject.FindWithTag("AudioManager");
                if (audioManagerObj)
                {
                    audioManager = audioManagerObj.GetComponent<AudioManager>();
                }
            }
        }

        private void Update()
        {
            previousDir = currentDir;
            currentDir = controller.collisions.faceDir;

            if (Input.GetKeyDown(KeyCode.E) && isHolding == false)
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
                        Rigidbody2D holdingObjRigidbody = holdingObject.GetComponent<Rigidbody2D>();
                        holdingObjRigidbody.velocity = Vector2.zero;
                        holdingObjRigidbody.angularVelocity = 0f;
                        holdingObject.GetComponent<Collider2D>().isTrigger = true;
                        previousType = holdingObject.GetComponent<Rigidbody2D>().bodyType;
                        holdingObjRigidbody.bodyType = RigidbodyType2D.Kinematic;
                        holdingObject.layer = LayerMask.NameToLayer("Grabbable");
                        isHolding = true;
                        controller.isHolding = true;
                        if (holdingObject.GetComponent<SpriteRenderer>())
                        {
                            previousSortOrder = holdingObject.GetComponent<SpriteRenderer>().sortingOrder;
                            holdingObject.GetComponent<SpriteRenderer>().sortingOrder = holdingSortOrder;
                        } else if (holdingObject.GetComponent<SpriteShapeRenderer>())
                        {
                            previousSortOrder = holdingObject.GetComponent<SpriteShapeRenderer>().sortingOrder;
                            holdingObject.GetComponent<SpriteShapeRenderer>().sortingOrder = holdingSortOrder;
                        }
                        
                        if(audioManager)
                            audioManager.PlaySoundEffect(audioManager.Clips.pickUpItem, pickupAndGrabVolume);
                        break;
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.E) && isHolding)
            {
                holdingObject.transform.SetParent(null);
                holdingObject.GetComponent<Collider2D>().isTrigger = false;
                Rigidbody2D holdingObjRigidbody = holdingObject.GetComponent<Rigidbody2D>();
                holdingObjRigidbody.velocity = controller.collisions.moveAmountOld * releaseForce;
                //holdingObjRigidbody.bodyType = previousType;
                holdingObjRigidbody.bodyType = RigidbodyType2D.Dynamic;
                if (holdingObject.GetComponent<SpriteRenderer>())
                {
                    holdingObject.GetComponent<SpriteRenderer>().sortingOrder = previousSortOrder;
                } else if (holdingObject.GetComponent<SpriteShapeRenderer>())
                {
                    holdingObject.GetComponent<SpriteShapeRenderer>().sortingOrder = previousSortOrder;
                }
                holdingObject = null;
                isHolding = false;
                controller.isHolding = false;
                
                if(audioManager)
                    audioManager.PlaySoundEffect(audioManager.Clips.dropItem, pickupAndGrabVolume);
            }

            DetectFlip();
        }

        private void DetectFlip()
        {
            if (!isHolding) return;

            if (currentDir != previousDir)
            {
                holdingObject.transform.SetParent((currentDir == -1) ? grabLeftPoint : grabRightPoint);
                holdingObject.transform.localPosition = Vector3.zero;
                holdingObject.transform.localRotation = Quaternion.identity;
            }
        }
    }