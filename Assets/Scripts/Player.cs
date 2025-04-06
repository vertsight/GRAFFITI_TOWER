using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour
{
    public event Action OnBreak;

    public PlayerCamera mainCamera;
    public float moveSpeed = 10f;
    public float jumpForce = 7f;
    public float fastFallForce = 15f;
    public float groundCheckDistance = 0.1f;
    public float downImpactForce = 10f;
    public float platformCooldown = 0.5f;
    public float minJumpHeightForDown = 3f;
    public LayerMask groundLayer;
    public LayerMask breakLayer;
    public GroundImpact groundImpact;
    public Animator animator;
    private float _lastXDirection = 1f;
    public float groundedBufferTime = 0.15f;
    public float minFallSpeed = -2f;

    private float _lastGroundedTime;
    private bool _isReallyGrounded;
    
    private Rigidbody2D rb;
    public bool isGrounded;
    public bool isCanMove = true;
    public bool isDowned;
    private bool canHitPlatform = true;
    private float jumpStartY;
    private bool jumpRecorded;
    private bool isCanPrimaryFall;
    private bool isStunning;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        HandlePlatformHit();
        CheckGrounded();
        HandleMovement();
        HandleJump();
        HandleFastFall();
        UpdateAnimation();

        if(!isGrounded && !jumpRecorded && rb.velocity.y > 0)
        {
            jumpStartY = transform.position.y;
            jumpRecorded = true;
        }
    }
    public IEnumerator Stunning()
    {
        isCanMove = false;
        isDowned = false;
        isStunning = true;
        animator.SetBool("IsStunning", true);
        yield return new WaitForSeconds(0.5f);
        isCanPrimaryFall = true;
        isCanMove = true;
        yield return new WaitForSeconds(0.3f);
        isCanPrimaryFall = false;
        animator.SetBool("IsStunning", false);
        isStunning = false;
    }
    private void CheckGrounded()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer) || 
                    Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, breakLayer);

        if(isGrounded)
        {
            _lastGroundedTime = Time.time;
            _isReallyGrounded = true;
            
            if(Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, breakLayer) && rb.velocity.y < -5)
            {
                jumpRecorded = false;
                return;
            }
            
            if(isDowned) 
            {
                groundImpact.PerformImpact();
                mainCamera.transform.DOShakePosition(0.2f);
                SoundManager.Instance.Explosion.Play();
            }
            isDowned = false;
            jumpRecorded = false;
        }
        else
        {
            _isReallyGrounded = (Time.time - _lastGroundedTime) < groundedBufferTime;
        }
    }
    
    private void HandleMovement()
    {
        if(isCanMove)
        {
            float moveInput = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
            
            if(Input.GetAxisRaw("Horizontal") == 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
            }
        }
    }
    private void UpdateAnimation()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        if (moveInput != 0 && isCanMove)
        {
            _lastXDirection = moveInput * 2.3f;
            transform.localScale = new Vector3(_lastXDirection, 2.3f, 1);
        }
        bool showFallAnimation = !_isReallyGrounded && rb.velocity.y < minFallSpeed;
            
        animator.SetBool("IsGrounded", _isReallyGrounded);
        animator.SetFloat("MoveSpeed", Mathf.Abs(rb.velocity.x));
        animator.SetBool("IsFalling", showFallAnimation);
    }
    
    private void HandleJump()
    {
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetTrigger("Jump");
        }
    }
    
    private void HandleFastFall()
    {
        bool reachedMinHeight = jumpRecorded && 
                              (transform.position.y - jumpStartY) >= minJumpHeightForDown;
        if((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && (!isGrounded || isCanPrimaryFall) && reachedMinHeight)
        {
            isCanPrimaryFall = false;
            rb.velocity = new Vector2(rb.velocity.x, -fastFallForce);
            isDowned = true;
        }
    }
    
    private void HandlePlatformHit()
    {
        if(!canHitPlatform) return;
        
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position, 
            Vector2.down, 
            groundCheckDistance, 
            breakLayer);
            
        if(hit.collider != null && isDowned)
        {
            groundImpact.PerformImpact();
            hit.collider.GetComponent<BreakingObstacle>()?.Break();
            
            OnBreak?.Invoke();

            Invoke(nameof(ForceToDown), 0.01f);
            
            canHitPlatform = false;
            
            Invoke(nameof(ResetPlatformHit), platformCooldown);
        }
    }
    
    private void ResetPlatformHit()
    {
        canHitPlatform = true;
    }
    private void ForceToDown()
    {
        rb.AddForce(Vector2.down * downImpactForce, ForceMode2D.Impulse);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isStunning && collision.gameObject.layer == 0)
        {
            float direction = transform.position.x < collision.transform.position.x ? 1f : -1f;
            
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(-direction, 0.7f) * 15, ForceMode2D.Impulse);
            SoundManager.Instance.Rebound.Play();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Destroy")
        {
            GameManager.Instance.RestartGame();
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
        if(!isGrounded && jumpRecorded)
        {
            Gizmos.color = Color.yellow;
            float currentHeight = transform.position.y - jumpStartY;
            Gizmos.DrawWireSphere(
                new Vector3(transform.position.x, jumpStartY + minJumpHeightForDown, 0), 
                0.5f);
                
            Gizmos.DrawLine(
                new Vector3(transform.position.x - 1, jumpStartY + minJumpHeightForDown, 0),
                new Vector3(transform.position.x + 1, jumpStartY + minJumpHeightForDown, 0));
        }
    }
}
