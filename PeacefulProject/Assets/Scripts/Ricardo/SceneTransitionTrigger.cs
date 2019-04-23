using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour
{

    public int targetScene;
    public KeyCode changeSceneKey;
    public Vector3 nextSceneTargetPos;

    public MasterSceneManager masterSceneManager;
    
    // Start is called before the first frame update
    void Start()
    {
        if(masterSceneManager == null)
            masterSceneManager = GameObject.FindWithTag("MasterSceneManager").GetComponent<MasterSceneManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (Input.GetKeyDown(changeSceneKey))
        {
            if (other.gameObject.CompareTag("Player"))
            {
                masterSceneManager.nextTeleportPosition = nextSceneTargetPos;
                masterSceneManager.TeleportRaccoon();
            }
        }
    }
}
