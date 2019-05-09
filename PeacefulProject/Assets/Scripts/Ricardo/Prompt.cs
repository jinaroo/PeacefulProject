using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Prompt : MonoBehaviour
{
    
    public float growTime = 1f;
    public Ease growEase;
    public Ease shrinkEase;

    private bool isDiplaying;

    public Transform cloudTransform;

    private void Start()
    {
        cloudTransform.localScale *= 0f;
        cloudTransform.GetComponent<SpriteRenderer>().DOFade(0f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isDiplaying = true;
            cloudTransform.DOScale(1f, growTime).SetEase(growEase);
            cloudTransform.GetComponent<SpriteRenderer>().DOFade(1f, growTime);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isDiplaying = false;
            cloudTransform.DOScale(0f, growTime).SetEase(shrinkEase);
            cloudTransform.GetComponent<SpriteRenderer>().DOFade(0f, growTime / 2f);
        }
    }
}
