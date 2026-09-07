using UnityEngine;

public class jump : MonoBehaviour
{
    public Rigidbody rb;
    private Animator animator;

    [Header("Jump")]
    public float jumpForce = 7f;

    [Header("Ground Detection")]
    [Tooltip("How steep a surface can be and still count as ground (in degrees from straight up).")]
    public float maxGroundAngle = 45f;
    private int groundContactCount = 0;
    private bool isGrounded => groundContactCount > 0;

    [Header("Jump Feel")]
    public float coyoteTime = 0.15f;      // grace period after walking off a ledge
    public float jumpBufferTime = 0.15f;  // grace period if jump is pressed just before landing
    private float coyoteTimer;
    private float jumpBufferTimer;

    [Header("Fall")]
    public float fallMultiplier = 2.5f;      // extra gravity while falling
    public float lowJumpMultiplier = 2f;     // extra gravity if jump is released early (short hop)
    public float maxFallSpeed = 20f;         // terminal velocity cap (positive number, applied downward)

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // Coyote time: stay "jumpable" briefly after leaving the ground
        if (isGrounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;

        // Jump buffer: remember a jump press briefly so it fires the instant you land
        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferTimer = jumpBufferTime;
        else
            jumpBufferTimer -= Time.deltaTime;

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
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        if (animator != null)
            animator.SetTrigger("Jump");
    }

    void ControlFallSpeed()
    {
        if (rb.linearVelocity.y < 0f)
        {
            // Falling: apply extra gravity so the fall feels weighty
            rb.AddForce(Vector3.down * (fallMultiplier - 1f) * Physics.gravity.magnitude, ForceMode.Acceleration);

            // Cap terminal velocity so long falls don't look unnaturally fast
            if (rb.linearVelocity.y < -maxFallSpeed)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, -maxFallSpeed, rb.linearVelocity.z);
            }
        }
        else if (rb.linearVelocity.y > 0f && !Input.GetKey(KeyCode.Space))
        {
            // Rising but the button was released early: cut the jump short for a variable jump height
            rb.AddForce(Vector3.down * (lowJumpMultiplier - 1f) * Physics.gravity.magnitude, ForceMode.Acceleration);
        }
    }

    // --- Collision-based ground detection ---
    // Counts any collision whose contact normal points up steeply enough to count as "floor"
    // rather than a wall. This needs no extra GameObjects or layer setup — it just uses the
    // colliders that already exist on the player and the ground.

    void OnCollisionEnter(Collision collision)
    {
        CheckGroundContact(collision);
    }

    void OnCollisionStay(Collision collision)
    {
        CheckGroundContact(collision);
    }

    void CheckGroundContact(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            float angle = Vector3.Angle(contact.normal, Vector3.up);
            if (angle <= maxGroundAngle)
            {
                groundContactCount++;
                return;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (groundContactCount > 0)
            groundContactCount--;
    }
}