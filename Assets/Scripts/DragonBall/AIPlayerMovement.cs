using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class AIPlayerMovement : MonoBehaviour 
{
    public Transform target;
    public Transform groundCheck;
    public LayerMask groundLayer;

    public float moveSpeed = 5f;
    public float jumpPower = 10f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float jumpThreshold = 2f;
    public float groundCheckRadius = 0.3f;

    private bool isFacingRight = true;
    private bool isDashing = false;
    private bool isGrounded = true;

    private int maxJumps = 2;
    private int jumpCount = 0;

    private float dashTimer;
    private float lastGroundedTime;
    private float coyoteTime = 0.1f;

    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (target == null)
        {
            GameObject ball = GameObject.FindGameObjectWithTag("Ball");
            if (ball != null) target = ball.transform;
        }
    }

    void Update()
    {
        if (target == null || isDashing) return;

        UpdateGroundedStatus();
        FlipSprite();

        float horizontalInput = Mathf.Clamp(target.position.x - transform.position.x, -1f, 1f);
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        animator.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));
        animator.SetFloat("yVelocity", rb.linearVelocity.y);

        if ((target.position.y > transform.position.y + jumpThreshold) && jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            jumpCount++;
            isGrounded = false;
            animator.SetBool("isJumping", true);
        }

        if (Mathf.Abs(horizontalInput) > 0.9f && IsGrounded())
        {
            if (dashTimer <= 0f)
            {
                Dash(horizontalInput);
            }
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
            }
        }
    }

    void UpdateGroundedStatus()
    {
        if (Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer))
        {
            isGrounded = true;
            jumpCount = 0;
            animator.SetBool("isJumping", false);
        }
    }

    void FlipSprite()
    {
        float direction = target.position.x - transform.position.x;
        if ((isFacingRight && direction < 0f) || (!isFacingRight && direction > 0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }
    }

    void Dash(float direction)
    {
        isDashing = true;
        dashTimer = dashDuration;
        rb.linearVelocity = new Vector2(Mathf.Sign(direction) * dashSpeed, 0f);

        if (direction < 0f) animator.SetTrigger("DashBackward");
        else animator.SetTrigger("DashForward");
    }

    bool IsGrounded()
    {
        return isGrounded;
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
