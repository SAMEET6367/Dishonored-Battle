using UnityEngine;

public class Dash : MonoBehaviour
{
    public Rigidbody rb;
    private Animator animator;        // reference to child mesh animator

    public float dashForce = 10f;
    public float dashCooldown = 1f;

    private float nextDashTime = 0f;

    void Start()
    {
        // Automatically find animator on child mesh
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
            Debug.LogError("No Animator found on child mesh!");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= nextDashTime)
        {
            DashForward();
        }
    }

    void DashForward()
    {
        // Optional: lock Y velocity if needed
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, rb.linearVelocity.z);

        // Apply dash force
        rb.AddForce(transform.forward * dashForce, ForceMode.Impulse);

        // Play dash animation directly
        animator.Play("Dash", 0, 0f); // Make sure the state name is exactly "Dash"

        // Set cooldown
        nextDashTime = Time.time + dashCooldown;
    }
}
