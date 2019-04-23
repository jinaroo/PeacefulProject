using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour
{
    public KeyCode changeSceneKey;
    public Vector3 targetPos;

    public MasterSceneManager masterSceneManager;
    
    // Start is called before the first frame update
    void Start()
    {
        if(masterSceneManager == null)
            masterSceneManager = GameObject.FindWithTag("MasterSceneManager").GetComponent<MasterSceneManager>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (Input.GetKeyDown(changeSceneKey))
        {
            if (other.gameObject.CompareTag("Player"))
            {
                masterSceneManager.nextTeleportPosition = targetPos;
                masterSceneManager.TeleportRaccoon();
            }
        }
    }
}
