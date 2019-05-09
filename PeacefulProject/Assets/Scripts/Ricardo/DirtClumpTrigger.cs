using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtClumpTrigger : MonoBehaviour
{
    private bool eventHasTriggered;
    
    public GameObject oldDirtClumpParentObject;
    public GameObject newDirtClumpParentObject;


    public void TriggerSewerPipeEvent()
    {
        oldDirtClumpParentObject.SetActive(false);
        newDirtClumpParentObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!eventHasTriggered)
            {
                eventHasTriggered = true;
                TriggerSewerPipeEvent();
            }
        }
    }
}
