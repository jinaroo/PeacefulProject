using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndergroundSoundTrigger : MonoBehaviour
{
    public MasterSceneManager masterSceneManager;

    void Start()
    {
        if(masterSceneManager == null)
            masterSceneManager = GameObject.FindWithTag("MasterSceneManager").GetComponent<MasterSceneManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            masterSceneManager.EnterUnderground();
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            masterSceneManager.EnterUnderground();
        }
    }
}
