using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObstacleController : MonoBehaviour
{
    private Rigidbody2D rBody;

    public LayerMask staticLayer;

    private bool isStatic = false;

    public LayerMask playerLayer;
    private ContactFilter2D conFilter;

    private float staticThreshold = 0.0001f;
    
    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        conFilter = new ContactFilter2D();
        conFilter.layerMask = playerLayer;
        conFilter.useLayerMask = true;
        conFilter.useTriggers = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (rBody.velocity.magnitude <= staticThreshold && rBody.angularVelocity <= staticThreshold && !rBody.isKinematic && !isStatic)
        {
            Collider2D[] results = new Collider2D[4];
            int numOverlapCols = GetComponent<Collider2D>().OverlapCollider(conFilter, results);
            if (numOverlapCols == 0)
            {   
                //rBody.isKinematic = true;
                gameObject.layer = LayerMask.NameToLayer("GrabbableObstacle");
                isStatic = true;
            }
        }
        else
        {
            isStatic = false;
        }
    }
}
