using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdRescueEvent : MonoBehaviour
{
    public GameObject dirtClumpParentObject;

    public void TriggerDirtClumpEvent()
    {
        dirtClumpParentObject.SetActive(false);
    }
}
