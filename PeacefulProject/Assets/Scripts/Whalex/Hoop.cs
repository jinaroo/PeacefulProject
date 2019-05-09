using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoop : MonoBehaviour
{
    private ParticleSystem particle;

    private void Start()
    {
        particle = GetComponentInChildren<ParticleSystem>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            EventManagerNew.Instance.Fire(new DunkEvent(other.transform));
            particle.Play();
        }
    }
}

public class DunkEvent : MyEvent
{
    public Transform ball;
    
    public DunkEvent(Transform obj)
    {
        ball = obj;
    }
}