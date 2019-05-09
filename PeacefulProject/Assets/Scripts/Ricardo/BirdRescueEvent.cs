using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdRescueEvent : MonoBehaviour
{
    public GameObject tunnelPipeTriggerObject;
    public GameObject sewerPipeTriggerObject;

    public void TriggerSewerPipeEvent()
    {
        tunnelPipeTriggerObject.SetActive(true);
        sewerPipeTriggerObject.SetActive(true);
    }
}
