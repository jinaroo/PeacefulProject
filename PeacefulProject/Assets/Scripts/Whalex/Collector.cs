using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Grabable"))
        {
            if (other.gameObject.CompareTag(gameObject.tag))
            {
                //EventManager.Instance.TriggerEvent("ObjCollect");
                EventManagerNew.Instance.Fire(new CollectEvent(transform));
            }
        }
    }
}