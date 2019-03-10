using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUp : MonoBehaviour
{
    public GameObject popUp;

    // Start is called before the first frame update
    void Start()
    {
        popUp.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        popUp.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        popUp.SetActive(false);

    }
}
