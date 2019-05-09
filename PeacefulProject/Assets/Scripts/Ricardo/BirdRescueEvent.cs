using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdRescueEvent : MonoBehaviour
{
    public SceneTransitionTrigger tunnelPipeTriggerObject;
    public SceneTransitionTrigger sewerPipeTriggerObject;

    public Prompt prompt1;
    public Prompt prompt2;
    
    public void TriggerSewerPipeEvent()
    {
        tunnelPipeTriggerObject.enabled = true;
        sewerPipeTriggerObject.enabled = true;

        prompt1.enabled = true;
        prompt2.enabled = true;
    }
}
