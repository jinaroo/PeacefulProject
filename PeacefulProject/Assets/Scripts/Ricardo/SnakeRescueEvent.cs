using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeRescueEvent : MonoBehaviour
{

    public SceneTransitionTrigger tunnelGrateTriggerObject;
    public SceneTransitionTrigger sewerGrateTriggerObject;
    //public GameObject sewerTopBranch;
    
    public Prompt prompt1;
    public Prompt prompt2;

    public GameObject branchCoverObj;

    public GameObject treebranchParent;
    
    public void TriggerSewerEvent()
    {
        tunnelGrateTriggerObject.enabled = true;
        sewerGrateTriggerObject.enabled = true;
        //sewerTopBranch.SetActive(false);

        prompt1.gameObject.SetActive(true);
        prompt2.gameObject.SetActive(true);

        branchCoverObj.SetActive(false);
        treebranchParent.SetActive(true);
    }
}
