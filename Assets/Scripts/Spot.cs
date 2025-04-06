using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spot : MonoBehaviour
{
    public Color[] possibleColors;
    public Sprite spot;
    public Sprite missile;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private void OnEnable()
    {
        Initialize();
    }
    public void Initialize()
    {
        if(spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if(rb == null) rb = GetComponent<Rigidbody2D>();
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = missile;
        spriteRenderer.color = possibleColors[Random.Range(0, possibleColors.Length)];
        spriteRenderer.maskInteraction = SpriteMaskInteraction.None;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }
    public void Remove()
    {
        transform.parent = null;
        transform.localScale = Vector3.one;
        SpotManager.Instance.usedSpots.Add(this);
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 0)
        {
            transform.SetParent(collision.transform);
            spriteRenderer.sprite = spot;
            spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            rb.bodyType = RigidbodyType2D.Static;
        }
    }
}
