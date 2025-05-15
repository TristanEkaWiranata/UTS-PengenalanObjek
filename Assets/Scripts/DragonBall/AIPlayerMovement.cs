using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class AIPlayerMovement : MonoBehaviour
{
    public Transform target;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public string paddleTag = "Paddle";

    [Header("Movement Parameters")]
    public float moveSpeed = 5f;
    public float jumpPower = 10f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float jumpThreshold = 2f;
    public float groundCheckRadius = 0.3f;
    public float dashDistanceThreshold = 3f;
    public float retreatDistance = 2f;
    public float decisionInterval = 0.3f;
    public float advancedDashDistance = 5f; // Distance threshold for advanced dash
    public float longDashSpeed = 20f; // Faster dash for longer distances

    [Header("Advanced Movement")]
    public float retreatBeforeJumpDistance = 1.5f;
    public float advancedSequenceDelay = 0.2f;
    public float dashAfterJumpDelay = 0.3f;

    private bool isFacingRight = false;
    private bool isDashing = false;
    private bool isGrounded = true;

    private int maxJumps = 2;
    private int jumpCount = 0;

    private float dashTimer;
    private float decisionTimer;
    private float sequenceTimer;
    private bool shouldRetreat;

    private Rigidbody2D rb;
    private Animator animator;

    // FSM (Finite State Machine)
    private enum AIState
    {
        Idle,
        Retreating,
        Jumping,
        Dashing,
        Approaching,
        RetreatBeforeJump,    // New state for retreat-jump-dash sequence
        JumpAfterRetreat,     // New state for retreat-jump-dash sequence
        DashAfterJump,        // New state for retreat-jump-dash sequence
        LongRangeDash         // New state for dashing from far away
    }

    private AIState currentState = AIState.Idle;
    private AIState nextState = AIState.Idle;
    private float stateTimer = 0f;
    private bool isExecutingSequence = false;
    private Vector2 dashDirection;

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
        if (target == null) return;

        UpdateGroundedStatus();
        FlipSprite();

        Vector2 directionToBall = target.position - transform.position;
        float horizontalInput = Mathf.Clamp(directionToBall.x, -1f, 1f);
        float distanceX = Mathf.Abs(directionToBall.x);
        float distanceY = directionToBall.y;

        animator.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));
        animator.SetFloat("yVelocity", rb.linearVelocity.y);

        // Make decisions at intervals rather than every frame (unless we're in a sequence)
        if (!isExecutingSequence)
        {
            decisionTimer -= Time.deltaTime;
            if (decisionTimer <= 0f)
            {
                decisionTimer = decisionInterval;
                EvaluateSituation(directionToBall, distanceX, distanceY);
            }
        }
        else
        {
            // Handle sequence timing
            sequenceTimer -= Time.deltaTime;
            if (sequenceTimer <= 0f)
            {
                ExecuteNextSequenceState();
            }
        }

        // State machine execution
        switch (currentState)
        {
            case AIState.Idle:
                if (!isDashing)
                {
                    rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
                }
                break;

            case AIState.Retreating:
                // Move away from ball when it's above us
                float retreatDirection = -Mathf.Sign(directionToBall.x);
                rb.linearVelocity = new Vector2(retreatDirection * moveSpeed, rb.linearVelocity.y);
                
                if (distanceX > retreatDistance || distanceY < jumpThreshold)
                {
                    currentState = AIState.Idle;
                }
                break;

            case AIState.Jumping:
                if (IsGrounded() && jumpCount < maxJumps)
                {
                    Jump();
                }
                else if (!IsGrounded() && jumpCount < maxJumps)
                {
                    // Enable double jump if we're in the air and have jumps left
                    Jump(0.9f); // Slightly weaker double jump
                }
                break;

            case AIState.Dashing:
                if (!isDashing && IsGrounded())
                {
                    Dash(Mathf.Sign(directionToBall.x));
                }
                break;

            case AIState.Approaching:
                // Move toward ball but be ready to jump/dash
                rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
                break;

            // New states for advanced sequence
            case AIState.RetreatBeforeJump:
                // First step of sequence: Move away from ball
                float retreatDir = -Mathf.Sign(directionToBall.x);
                rb.linearVelocity = new Vector2(retreatDir * moveSpeed * 1.2f, rb.linearVelocity.y);
                break;

            case AIState.JumpAfterRetreat:
                // Second step of sequence: Jump higher
                if (IsGrounded() && jumpCount < maxJumps)
                {
                    // Force jump immediately with higher power
                    Jump(1.3f); // Jump with 30% extra power
                }
                else if (!IsGrounded() && jumpCount < maxJumps)
                {
                    // Double jump if we're already in the air
                    Jump(1.2f);
                }
                break;

            case AIState.DashAfterJump:
                // Third step of sequence: Dash toward the ball while in air
                if (!isDashing)
                {
                    isDashing = true;
                    dashTimer = dashDuration;
                    dashDirection = (target.position - transform.position).normalized;
                    rb.linearVelocity = new Vector2(dashDirection.x * dashSpeed * 1.2f, rb.linearVelocity.y);
                
                    if (dashDirection.x < 0f)
                        animator.SetTrigger("DashBackward");
                    else
                        animator.SetTrigger("DashForward");
                }
                break;

            case AIState.LongRangeDash:
                // Special dash for long distances
                if (!isDashing && IsGrounded())
                {
                    isDashing = true;
                    dashTimer = dashDuration * 1.5f;
                    rb.linearVelocity = new Vector2(Mathf.Sign(directionToBall.x) * longDashSpeed, rb.linearVelocity.y);
                
                    if (directionToBall.x < 0f)
                        animator.SetTrigger("DashBackward");
                    else
                        animator.SetTrigger("DashForward");
                }
                break;
        }

        // Handle dash timer
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
                
                // If we were in a dash state, transition to idle or next sequence state
                if (currentState == AIState.Dashing || currentState == AIState.LongRangeDash || 
                    currentState == AIState.DashAfterJump)
                {
                    if (!isExecutingSequence)
                        currentState = AIState.Idle;
                    else
                        ExecuteNextSequenceState();
                }
            }
        }
    }

    void EvaluateSituation(Vector2 directionToBall, float distanceX, float distanceY)
    {
        // Avoid evaluating if we're already executing a sequence
        if (isExecutingSequence) return;

        // Check if there are paddles nearby that might hit the ball
        bool paddlesNearby = CheckForNearbyPaddles();

        // Ball is far away horizontally - use long range dash
        if (distanceX > advancedDashDistance && IsGrounded())
        {
            currentState = AIState.LongRangeDash;
            return;
        }

        // Ball is significantly above us - start the retreat-jump-dash sequence
        if (distanceY > jumpThreshold && distanceX < retreatDistance * 2.5f)
        {
            StartAdvancedSequence();
            return;
        }
        
        // Ball is at jumpable height and far enough horizontally
        if (distanceY > jumpThreshold * 0.5f && distanceX > dashDistanceThreshold * 0.7f)
        {
            if (IsGrounded() || jumpCount < maxJumps)
            {
                currentState = AIState.Jumping;
                return;
            }
        }
        
        // Ball is far horizontally but not extremely far
        if (distanceX > dashDistanceThreshold && distanceX < advancedDashDistance && IsGrounded())
        {
            currentState = AIState.Dashing;
            return;
        }
        
        // Ball is at similar height but not too close
        if (distanceX > 1f && distanceY < 1f)
        {
            currentState = AIState.Approaching;
            return;
        }
        
        // Default state
        currentState = AIState.Idle;
    }

    void StartAdvancedSequence()
    {
        isExecutingSequence = true;
        
        // Set up the sequence: Retreat -> Jump -> Dash
        currentState = AIState.RetreatBeforeJump;
        sequenceTimer = advancedSequenceDelay;
        
        // Force reset jump count to ensure we can jump during the sequence
        if (IsGrounded())
        {
            jumpCount = 0;
        }
    }

    void ExecuteNextSequenceState()
    {
        switch (currentState)
        {
            case AIState.RetreatBeforeJump:
                currentState = AIState.JumpAfterRetreat;
                sequenceTimer = advancedSequenceDelay;
                break;
                
            case AIState.JumpAfterRetreat:
                currentState = AIState.DashAfterJump;
                sequenceTimer = dashAfterJumpDelay;
                break;
                
            case AIState.DashAfterJump:
                // End of sequence
                isExecutingSequence = false;
                currentState = AIState.Idle;
                break;
                
            default:
                // In case something goes wrong, reset
                isExecutingSequence = false;
                currentState = AIState.Idle;
                break;
        }
    }

    bool CheckForNearbyPaddles()
    {
        GameObject[] paddles = GameObject.FindGameObjectsWithTag(paddleTag);
        foreach (GameObject paddle in paddles)
        {
            if (paddle == gameObject) continue; // Skip self
            
            float distance = Vector2.Distance(transform.position, paddle.transform.position);
            if (distance < 5f) // Adjust this distance as needed
            {
                return true;
            }
        }
        return false;
    }

    void Jump(float powerMultiplier = 1.0f)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower * powerMultiplier);
        jumpCount++;
        isGrounded = false;
        animator.SetBool("isJumping", true);
        
        // After jumping, decide next action if not in a sequence
        if (!isExecutingSequence)
        {
            currentState = AIState.Approaching;
        }
    }

    void Dash(float direction)
    {
        isDashing = true;
        dashTimer = dashDuration;
        rb.linearVelocity = new Vector2(Mathf.Sign(direction) * dashSpeed, rb.linearVelocity.y);

        if (direction < 0f)
            animator.SetTrigger("DashBackward");
        else
            animator.SetTrigger("DashForward");
    }

    void UpdateGroundedStatus()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
        if (isGrounded && !wasGrounded)
        {
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