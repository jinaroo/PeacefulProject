using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeRescueEvent : MonoBehaviour
{

    public GameObject tunnelGrateTriggerObject;
    public GameObject sewerGrateTriggerObject;
    public GameObject sewerTopBranch;
    
    public void TriggerSewerEvent()
    {
        tunnelGrateTriggerObject.SetActive(true);
        sewerGrateTriggerObject.SetActive(true);
        sewerTopBranch.SetActive(false);
    }
}
