using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogRescueEvent : MonoBehaviour
{
    public GameObject oldBow;

    public GameObject featherParent;

    public void TriggerBowEvent()
    {
        oldBow.SetActive(false);
        featherParent.SetActive(true);
    }
}
