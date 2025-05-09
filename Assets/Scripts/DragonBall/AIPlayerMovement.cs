using UnityEngine;

public class AIPlayerMovement : MonoBehaviour
{
    public Transform ball;
    public float moveSpeed = 5f;
    public float jumpPower = 10f;
    public float followDistance = 1.5f;
    public float jumpThreshold = 1.5f;

    public float dashSpeed = 15f;
    public float dashCooldown = 1f;
    private float lastDashTime = -10f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isFacingRight = true;

    void Start()
    {
        // Cek apakah groundCheck dan ball sudah di-assign
        if (groundCheck == null)
        {
            Debug.LogError("GroundCheck belum di-assign!");
        }

        if (ball == null)
        {
            Debug.LogError("Ball belum di-assign!");
        }

        // Cek apakah Rigidbody2D dan Animator ada
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D tidak ditemukan pada objek!");
        }

        if (animator == null)
        {
            Debug.LogError("Animator tidak ditemukan pada objek!");
        }
    }

    void Update()
    {
        if (ball == null) return; // Jika bola null, stop eksekusi

        float direction = ball.position.x - transform.position.x;

        // DASH jika bola cukup jauh & cooldown selesai & grounded
        if (Mathf.Abs(direction) > 3f && Time.time - lastDashTime > dashCooldown && IsGrounded())
        {
            Dash(Mathf.Sign(direction));
            lastDashTime = Time.time;
        }

        // Gerakan horizontal jika terlalu jauh dari bola
        if (Mathf.Abs(direction) > followDistance)
        {
            rb.linearVelocity = new Vector2(Mathf.Sign(direction) * moveSpeed, rb.linearVelocity.y);
            FlipSprite(direction);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        // LOMPAT jika bola ada cukup tinggi di atas & grounded
        if (ball.position.y > transform.position.y + jumpThreshold && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            animator.SetBool("isJumping", true);
        }

        animator.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    void Dash(float dir)
    {
        rb.linearVelocity = new Vector2(dir * dashSpeed, 0);
        if (dir < 0)
            animator.SetTrigger("DashBackward");
        else
            animator.SetTrigger("DashForward");
    }

    void FlipSprite(float dir)
    {
        if ((isFacingRight && dir < 0) || (!isFacingRight && dir > 0))
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Pastikan tidak terjadi null reference pada collision
        if (collision != null && collision.CompareTag("Ground"))
        {
            animator.SetBool("isJumping", false);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
