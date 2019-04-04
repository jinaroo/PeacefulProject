using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour
{

    public int targetScene;
    public KeyCode changeSceneKey;
    public Vector3 nextSceneTargetPos;

    public StateManager stateManager;
    
    // Start is called before the first frame update
    void Start()
    {
        stateManager = GameObject.FindWithTag("StateManager").GetComponent<StateManager>();
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
                stateManager.raccoonStartPos = nextSceneTargetPos;
                stateManager.isDogActive = true;
                SceneManager.LoadScene(targetScene);
            }
        }
    }
}
