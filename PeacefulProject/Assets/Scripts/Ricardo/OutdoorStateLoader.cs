using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutdoorStateLoader : MonoBehaviour
{
    
    public StateManager stateManager;

    public GameObject dogParent;

    public Transform playerTransform;

    public Transform camTransform;
    // Start is called before the first frame update
    void Start()
    {
        stateManager = GameObject.FindWithTag("StateManager").GetComponent<StateManager>();

        if(stateManager.isDogActive)
            dogParent.transform.GetChild(0).gameObject.SetActive(true);

        if (stateManager.raccoonStartPos != Vector3.zero) 
        {
            playerTransform.position = stateManager.raccoonStartPos;
            camTransform.position = stateManager.raccoonStartPos + Vector3.back;
        }
    }

}
