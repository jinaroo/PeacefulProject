using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnFire : MonoBehaviour
{
    public GameObject fire;

    public void TurnFireOn()
    {
        fire.SetActive(true);
    }
}
