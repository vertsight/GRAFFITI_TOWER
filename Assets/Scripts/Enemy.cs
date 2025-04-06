using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Animations")]
    public MultiSpriteAnimation spriteAnimations;

    [Header("Enemy Settings")]
    public float jumpForce = 8f; 
    public float jumpCooldown = 2f; 
    public float detectionRange = 5f; 
    public float KnockbackForce = 10f;
    public float groundKnockbackForce = 3f;
    public float enemyKnockbackForce = 5f; 
    public float horizontalRatio = 0.7f; 
    public float minVerticalForce = 0.4f; 
    
    [Header("Ground Check")]
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;
    
    public Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spr;
    public bool isGrounded;
    private float lastJumpTime;
    public bool canJump = true;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    
    void Update()
    {
        CheckGrounded();
        TryJumpAtPlayer();
    }

    public IEnumerator Stunning()
    {
        canJump = false;
        GetComponent<CapsuleCollider2D>().isTrigger = true;
        spr.color = Color.blue;
        spriteAnimations.PlayAnimation("Stunning");
        yield return new WaitForSeconds(0.9f);
        spriteAnimations.PlayAnimation("Jump");
        canJump = true;
        spr.color = Color.white;
        GetComponent<CapsuleCollider2D>().isTrigger = false;
    }
    void CheckGrounded()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
    }
    
   void TryJumpAtPlayer()
    {
        if(!canJump || !isGrounded || Time.time - lastJumpTime < jumpCooldown) 
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        if(distanceToPlayer <= detectionRange)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(player.position.x - transform.position.x) * 1.4f;
            transform.localScale = scale;

            Vector2 jumpDirection = (player.position - transform.position).normalized;
            jumpDirection.y = 0.7f;
            
            if(spriteAnimations != null)
                spriteAnimations.PlayAnimation("Jump");
                
            rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
            
            lastJumpTime = Time.time;
            canJump = false;
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    
    void ResetJump()
    {
        canJump = true;
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
            if(collision.gameObject.GetComponent<Player>().isDowned) //collision.transform.position.y > transform.position.y+1 && 
            {
                StartCoroutine(Stunning());
                rb.AddForce(new Vector2(20 * -direction, 10), ForceMode2D.Impulse);
                return;
            }
            
            float horizontalForce = KnockbackForce * horizontalRatio;
            float verticalForce = Mathf.Max(KnockbackForce * (1 - horizontalRatio), minVerticalForce);
            
            
            Vector2 knockback = new Vector2(
                direction * horizontalForce,
                verticalForce
            );
            
            
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            StartCoroutine(collision.gameObject.GetComponent<Player>().Stunning());
            playerRb.velocity = Vector2.zero; 
            playerRb.AddForce(knockback, ForceMode2D.Impulse);
            
            
            rb.AddForce(new Vector2(-direction * 2f, 3f), ForceMode2D.Impulse);
        }
    }
    private void OnDrawGizmosSelected()
    {
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}