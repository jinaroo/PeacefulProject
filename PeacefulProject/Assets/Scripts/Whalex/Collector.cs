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
    
    private bool[] collectStatus;
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
                            EventManagerNew.Instance.Fire(new CollectEvent(collectTransforms[0]));
                        }
                        break;
                    case Dialogue.CharacterType.SNAKE:
                        if (other.CompareTag("SnakeBranch") && numItemsCollected < 3)
                        {
                            targetDialogue.AcceptItem(numItemsCollected);
                            EventManagerNew.Instance.Fire(new CollectEvent(collectTransforms[numItemsCollected]));
                            numItemsCollected++;
                        } else if (other.CompareTag("SnakeLighter") && !collectStatus[3])
                        {
                            collectStatus[3] = true;
                            targetDialogue.AcceptItem(3);
                            EventManagerNew.Instance.Fire(new CollectEvent(collectTransforms[3]));
                        }
                        break;
                    case Dialogue.CharacterType.BIRD:
                        if (other.CompareTag("BirdRope") && !collectStatus[0])
                        {
                            collectStatus[0] = true;
                            targetDialogue.AcceptItem(0);
                            EventManagerNew.Instance.Fire(new CollectEvent(collectTransforms[0]));
                        } else if (other.CompareTag("BirdSquare") && !collectStatus[1])
                        {
                            collectStatus[1] = true;
                            targetDialogue.AcceptItem(1);
                            EventManagerNew.Instance.Fire(new CollectEvent(collectTransforms[1]));
                        } else if (other.CompareTag("BirdCircle") && !collectStatus[2])
                        {
                            collectStatus[2] = true;
                            targetDialogue.AcceptItem(2);
                            EventManagerNew.Instance.Fire(new CollectEvent(collectTransforms[2]));
                        } else if (other.CompareTag("BirdTriangle") && !collectStatus[3])
                        {
                            collectStatus[3] = true;
                            targetDialogue.AcceptItem(3);
                            EventManagerNew.Instance.Fire(new CollectEvent(collectTransforms[3]));
                        }
                        break;
                }
            }
            
        }
    }
}