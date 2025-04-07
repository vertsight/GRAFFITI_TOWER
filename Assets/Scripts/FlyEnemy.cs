using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class FlyEnemy : MonoBehaviour
{
    public bool IsUsing;
    private float startX;
    private Rigidbody2D rb;
    private PlayerCamera playerCamera;

    private void OnEnable()
    {
        playerCamera = FindFirstObjectByType<PlayerCamera>();
        rb = GetComponent<Rigidbody2D>();
        startX = transform.position.x;
        Physics2D.IgnoreLayerCollision(0, 10);
        Physics2D.IgnoreLayerCollision(10, 8);
        Physics2D.IgnoreLayerCollision(10, 7);
    }
    public void Initialize()
    {
        IsUsing = true;
        rb.velocity = Vector3.zero;
        transform.position = new Vector3(startX, -11.5f + playerCamera.transform.position.y);
        transform.DOLocalMoveY(-6.36f, 1f);
        StartCoroutine(Attack());
    }
    public void SetUsed()
    {
        rb.velocity = Vector3.zero;
        transform.localPosition = new Vector3(startX, 13.7f);
        IsUsing = false;
    }
    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(4f);
        transform.DOLocalMoveY(13.7f, 1.5f).onComplete = () => 
        {
            SetUsed();
        };
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Destroy")
        {
            NotificationsManager.Instance.SpawnNotification(transform.position.x < 0 ? transform.position + Vector3.right : 
                transform.position + Vector3.left, 1);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            float direction = transform.position.x < collision.transform.position.x ? 1f : -1f;
            if(collision.gameObject.GetComponent<Player>().isDowned)
            {
                // StartCoroutine(Stunning());
                rb.AddForce(new Vector2(20 * -direction, 10), ForceMode2D.Impulse);
                transform.DORotate(new Vector3(0, 0, 90 * -direction), 0.7f);
                return;
            }
            
            float horizontalForce = 10f * 0.7f;
            float verticalForce = Mathf.Max(10f * (1 - 0.7f), 0.4f);
            
            
            Vector2 knockback = new Vector2(
                direction * horizontalForce,
                -verticalForce
            );
            
            
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            StartCoroutine(collision.gameObject.GetComponent<Player>().Stunning());
            playerRb.velocity = Vector2.zero; 
            playerRb.AddForce(knockback, ForceMode2D.Impulse);
            
            
            rb.AddForce(new Vector2(-direction * 2f, 3f), ForceMode2D.Impulse);
        }
    }
}