using UnityEngine;

public class jump : MonoBehaviour
{
    public Rigidbody rb;
    private Animator animator;

    [Header("Jump")]
    public float jumpForce = 7f;
    public float jumpInterval = 0.5f;
    private float nextJumpTime = 0f;

    [Header("Fall")]
    public float fallMultiplier = 2.5f; // Increase to fall faster

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextJumpTime)
        {
            Jump();
        }

        ControlFallSpeed();
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        animator.SetTrigger("Jump");

        nextJumpTime = Time.time + jumpInterval;
    }

    void ControlFallSpeed()
    {
        // Only apply while falling
        if (rb.linearVelocity.y < 0f)
        {
            rb.AddForce(Vector3.down * (fallMultiplier - 1f) * Physics.gravity.magnitude, ForceMode.Acceleration);
        }
    }
}