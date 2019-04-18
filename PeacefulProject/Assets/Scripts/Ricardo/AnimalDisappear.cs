using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalDisappear : MonoBehaviour
{
    private bool isDisappearing = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                if (!isDisappearing)
                {
                    Invoke("Disappear", 4f);
                }
            }
        }
    }

    void Disappear()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
