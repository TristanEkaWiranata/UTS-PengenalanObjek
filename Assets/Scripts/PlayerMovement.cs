using UnityEngine;

public class PLayerMovement : MonoBehaviour
{
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode jumpKey = KeyCode.W;

    bool isFacingRight = true;
    bool isGrounded = true;
    bool isDashing = false;

    float horizontalInput;
    float moveSpeed = 5f;
    float jumpPower = 10f;

    float dashSpeed = 15f;
    float dashDuration = 0.2f;
    float dashTimer;

    float tapTimer = 0f;
    float tapThreshold = 0.25f;
    int lastTapDirection = 0;
    bool waitingForSecondTap = false;

    Rigidbody2D rb;
    Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDashing) return;

        // Reset input
        horizontalInput = 0f;

        // Gerakan biasa
        if (Input.GetKey(leftKey)) horizontalInput = -1f;
        if (Input.GetKey(rightKey)) horizontalInput = 1f;

        // Handle tap untuk dash
        if (Input.GetKeyDown(leftKey)) HandleTap(-1);
        if (Input.GetKeyDown(rightKey)) HandleTap(1);

        // Timer tap
        if (waitingForSecondTap)
        {
            tapTimer += Time.deltaTime;
            if (tapTimer > tapThreshold)
            {
                waitingForSecondTap = false;
            }
        }

        // Flip
        FlipSprite();

        // Lompat
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            isGrounded = false;
            animator.SetBool("isJumping", true);
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
            return;
        }

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        animator.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    void FlipSprite()
    {
        if ((isFacingRight && horizontalInput < 0f) || (!isFacingRight && horizontalInput > 0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }
    }

    void HandleTap(int direction)
    {
        horizontalInput = direction;
        
        if (waitingForSecondTap && lastTapDirection == direction)
        {
            Dash(direction);
            waitingForSecondTap = false;
        }
        else
        {
            lastTapDirection = direction;
            tapTimer = 0f;
            waitingForSecondTap = true;
        }
    }

    void Dash(float direction)
    {
        isDashing = true;
        rb.linearVelocity = new Vector2(direction * dashSpeed, 0f);
        dashTimer = dashDuration;

        if (direction < 0f) animator.SetTrigger("DashBackward");
        else animator.SetTrigger("DashForward");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("isJumping", false);
        }
    }
}
