using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour
{
    public KeyCode changeSceneKey;
    public Vector3 targetPos;

    public MasterSceneManager masterSceneManager;

    private bool playerinTrigger;

    // Start is called before the first frame update
    void Start()
    {
        if (masterSceneManager == null)
            masterSceneManager = GameObject.FindWithTag("MasterSceneManager").GetComponent<MasterSceneManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerinTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerinTrigger = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(changeSceneKey))
        {
            if (playerinTrigger)
            {
                masterSceneManager.nextTeleportPosition = targetPos;
                masterSceneManager.TeleportRaccoon();
            }
        }
    }

}
