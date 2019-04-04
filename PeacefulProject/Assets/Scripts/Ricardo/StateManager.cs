using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateManager : MonoBehaviour
{
    public Vector3 raccoonStartPos;

    public bool isDogActive = false;
    
    
    void Awake()
    {
        DontDestroyOnLoad(gameObject);    
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

}
