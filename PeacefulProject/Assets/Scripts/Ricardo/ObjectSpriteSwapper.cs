using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpriteSwapper : MonoBehaviour
{
    public Sprite defaultSprite;
    public Sprite heldSprite;
    public Sprite placedSprite;

    public SpriteRenderer targetSpriteRenderer;
    public Transform targetTransform;

    public Vector3 defaultSpriteScale = Vector3.one;
    public Vector3 heldSpriteScale = Vector3.one;
    public Vector3 placedSpriteScale = Vector3.one;

    void Start()
    {
        if (!targetSpriteRenderer)
            targetSpriteRenderer = GetComponent<SpriteRenderer>();

        if (!targetTransform)
            targetTransform = transform;
    }
    
    public void SwitchToDefaultSprite()
    {
        targetSpriteRenderer.sprite = defaultSprite;
        targetTransform.localScale = defaultSpriteScale;
    }
    
    public void SwitchToHeldSprite()
    {
        targetSpriteRenderer.sprite = heldSprite;
        targetTransform.localScale = heldSpriteScale;
    }
    
    public void SwitchToPlacedSprite()
    {
        targetSpriteRenderer.sprite = placedSprite;
        targetTransform.localScale = placedSpriteScale;
    }
}
