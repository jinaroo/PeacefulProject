using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whalex;

public class CollectEvent : MyEvent
{
    public readonly Transform collectorTf;
    public CollectEvent(Transform tf)
    {
        collectorTf = tf;
    }
}

public class Collector : MonoBehaviour
{
    public Transform[] collectTransforms;
    public Dialogue targetDialogue;
    
    public bool[] collectStatus;
    private int numItemsCollected;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Grabbable"))
        {
            if (targetDialogue.hasBeenRescued && !targetDialogue.questComplete)
            {
                switch (targetDialogue.charType)
                {
                    case Dialogue.CharacterType.DOG:
                        if (other.CompareTag("DogBow") && !collectStatus[0])
                        {
                            collectStatus[0] = true;
                            targetDialogue.AcceptItem(0);
                            
                            SnapItem(other.gameObject, collectTransforms[0]);
                        }
                        break;
                    case Dialogue.CharacterType.SNAKE:
                        if (other.CompareTag("SnakeBranch") && numItemsCollected < 3)
                        {
                            targetDialogue.AcceptItem(numItemsCollected);
                            SnapItem(other.gameObject, collectTransforms[numItemsCollected]);
                            numItemsCollected++;
                            if (numItemsCollected == 3)
                            {
                                targetDialogue.numPhasesComplete++;
                                targetDialogue.SwitchToNextIconSequence();
                            }
                        } else if (other.CompareTag("SnakeLighter") && !collectStatus[3])
                        {
                            if (targetDialogue.numPhasesComplete > 0)
                            {
                                collectStatus[3] = true;
                                targetDialogue.AcceptItem(3);
                                SnapItem(other.gameObject, collectTransforms[3]);
                            }
                        }
                        break;
                    case Dialogue.CharacterType.BIRD:
                        if (other.CompareTag("BirdRope") && !collectStatus[0])
                        {
                            collectStatus[0] = true;
                            targetDialogue.AcceptItem(0);
                            SnapItem(other.gameObject, collectTransforms[0]);
                            targetDialogue.numPhasesComplete++;
                            targetDialogue.SwitchToNextIconSequence();
                        } else if (other.CompareTag("BirdSquare") && !collectStatus[1])
                        {
                            if (targetDialogue.numPhasesComplete > 0)
                            {
                                collectStatus[1] = true;
                                targetDialogue.AcceptItem(1);
                                SnapItem(other.gameObject, collectTransforms[1]);
                            }
                        } else if (other.CompareTag("BirdCircle") && !collectStatus[2])
                        {
                            if (targetDialogue.numPhasesComplete > 0)
                            {
                                collectStatus[2] = true;
                                targetDialogue.AcceptItem(2);
                                SnapItem(other.gameObject, collectTransforms[2]);
                            }
                        } else if (other.CompareTag("BirdTriangle") && !collectStatus[3])
                        {
                            if (targetDialogue.numPhasesComplete > 0)
                            {
                                collectStatus[3] = true;
                                targetDialogue.AcceptItem(3);
                                SnapItem(other.gameObject, collectTransforms[3]);
                            }
                        }
                        break;
                }
            }
            
        }
    }

    void SnapItem(GameObject holdingObject, Transform collectorTf)
    {
        if (holdingObject.GetComponent<Collider2D>().isTrigger)
        {
            EventManagerNew.Instance.Fire(new CollectEvent(collectorTf));  
        }
        else
        {
            holdingObject.transform.SetParent(collectorTf);
            holdingObject.transform.localPosition = Vector3.zero;
            holdingObject.transform.localRotation = Quaternion.identity;
            holdingObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            holdingObject.GetComponent<Rigidbody2D>().angularVelocity = 0f;
            holdingObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            holdingObject.GetComponent<Collider2D>().isTrigger = true;

            // change its layer so it won't be picked up by player again
            holdingObject.layer = LayerMask.NameToLayer("Placed");
            
            ObjectSpriteSwapper spriteSwapper = holdingObject.GetComponent<ObjectSpriteSwapper>();
            if(spriteSwapper)
                spriteSwapper.SwitchToPlacedSprite();

        }
    }
}