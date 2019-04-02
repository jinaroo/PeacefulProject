using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour
{

    public int targetScene;
    public KeyCode changeSceneKey;
    
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
        if (Input.GetKeyDown(changeSceneKey))
        {
            if (other.gameObject.CompareTag("Player"))
            {
                SceneManager.LoadScene(targetScene);
            }
        }
    }
}
