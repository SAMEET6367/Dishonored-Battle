using UnityEngine;

public class jump : MonoBehaviour
{
    public Rigidbody rb;
    private Animator animator;

    [Header("Jump")]
    [Tooltip("This is the upward speed (m/s) applied on jump — NOT affected by the player's mass, so it stays consistent no matter how heavy the Rigidbody is.")]
    public float jumpForce = 7f;

    [Header("Ground Detection")]
    [Tooltip("How steep a surface can be and still count as ground (in degrees from straight up).")]
    public float maxGroundAngle = 45f;
    private readonly System.Collections.Generic.HashSet<Collider> groundColliders = new System.Collections.Generic.HashSet<Collider>();
    private bool isGrounded => groundColliders.Count > 0;
    public bool IsGrounded => isGrounded;

    [Header("Debug")]
    [Tooltip("Read-only — watch this in Play mode to see if the physics thinks the player is grounded.")]
    [SerializeField] private bool isGroundedDebugView;

    [Header("Jump Feel")]
    public float coyoteTime = 0.15f;      // grace period after walking off a ledge
    public float jumpBufferTime = 0.15f;  // grace period if jump is pressed just before landing
    private float coyoteTimer;
    private float jumpBufferTimer;

    [Header("Fall")]
    public float fallMultiplier = 3.5f;      // extra gravity while falling (raised from 2.5 — was hanging too long)
    public float lowJumpMultiplier = 2f;     // extra gravity if jump is released early (short hop)
    public float maxFallSpeed = 20f;         // terminal velocity cap (positive number, applied downward)

    [Header("Apex")]
    [Tooltip("Speed (up or down) below which the player is considered 'at the apex' of the jump.")]
    public float apexThreshold = 2f;
    [Tooltip("Extra gravity applied right at the top of the arc, so the player doesn't hang in the air.")]
    public float apexMultiplier = 2f;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        // Auto-find the Rigidbody if it wasn't dragged in manually, so this
        // works in every scene without needing the field set by hand each time.
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (rb == null)
            Debug.LogError("No Rigidbody found on this GameObject! Add a Rigidbody component to the player.");
    }

    void Update()
    {
        // Only read input and update timers here — no Rigidbody changes in Update.
        // That way input is never missed between physics ticks, but the actual
        // physics response always happens on a consistent FixedUpdate step,
        // in sync with Movement.cs.

        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferTimer = jumpBufferTime;
        else
            jumpBufferTimer -= Time.deltaTime;

        if (isGrounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;

        // Debug view — watch this checkbox in the Inspector during Play mode.
        // If it's ticking true/false correctly right when the character visibly
        // touches the ground, the physics is fine and this is purely an
        // animation/Animator Controller issue. If it stays false for a while
        // after the character clearly looks landed, it's a real physics/ground
        // detection issue instead.
        isGroundedDebugView = isGrounded;

        // Keep the Animator's grounded state in sync every frame, so any
        // Animator Controller transition that depends on landing actually fires.
        if (animator != null)
            animator.SetBool("Grounded", isGrounded);
    }

    void FixedUpdate()
    {
        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            Jump();
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }

        ControlFallSpeed();
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);

        if (animator != null)
            animator.SetTrigger("Jump");
    }

    void ControlFallSpeed()
    {
        float verticalSpeed = rb.linearVelocity.y;

        if (Mathf.Abs(verticalSpeed) < apexThreshold)
        {
            // Near the top of the arc, velocity is close to zero and gravity alone
            // barely does anything for a moment — this is what causes the "hang in
            // the air" floaty feeling. Push extra gravity here so the peak is a
            // sharp point instead of a long flat hover.
            rb.AddForce(Vector3.down * (apexMultiplier - 1f) * Physics.gravity.magnitude, ForceMode.Acceleration);
        }
        else if (verticalSpeed < 0f)
        {
            // Falling: apply extra gravity so the fall feels weighty
            rb.AddForce(Vector3.down * (fallMultiplier - 1f) * Physics.gravity.magnitude, ForceMode.Acceleration);

            // Cap terminal velocity so long falls don't look unnaturally fast
            if (rb.linearVelocity.y < -maxFallSpeed)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, -maxFallSpeed, rb.linearVelocity.z);
            }
        }
        else if (verticalSpeed > 0f && !Input.GetKey(KeyCode.Space))
        {
            // Rising but the button was released early: cut the jump short for a variable jump height
            rb.AddForce(Vector3.down * (lowJumpMultiplier - 1f) * Physics.gravity.magnitude, ForceMode.Acceleration);
        }
    }

    // --- Collision-based ground detection ---
    // Tracks the specific colliders currently touching the player at a "floor"
    // angle, rather than counting collision events. This avoids the counter
    // growing unbounded while standing still (OnCollisionStay fires every
    // physics step for as long as contact persists) and correctly clears when
    // that specific collider stops touching.

    void OnCollisionEnter(Collision collision)
    {
        UpdateGroundContact(collision);
    }

    void OnCollisionStay(Collision collision)
    {
        UpdateGroundContact(collision);
    }

    void UpdateGroundContact(Collision collision)
    {
        bool touchingGround = false;

        foreach (ContactPoint contact in collision.contacts)
        {
            float angle = Vector3.Angle(contact.normal, Vector3.up);
            if (angle <= maxGroundAngle)
            {
                touchingGround = true;
                break;
            }
        }

        if (touchingGround)
            groundColliders.Add(collision.collider);
        else
            groundColliders.Remove(collision.collider); // e.g. a wall — don't count it as ground
    }

    void OnCollisionExit(Collision collision)
    {
        groundColliders.Remove(collision.collider);
    }
}