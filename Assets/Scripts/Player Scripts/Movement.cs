using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public Rigidbody rb;

    private Animator animator;
    private Vector3 moveDir;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
            Debug.LogError("No Animator found on Player or its children!");

        // Auto-find the Rigidbody if it wasn't dragged in manually. This means
        // the script works correctly in every scene the player prefab/object
        // is placed in, without needing the field set by hand each time.
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (rb == null)
            Debug.LogError("No Rigidbody found on this GameObject! Add a Rigidbody component to the player.");
    }

    void Update()
    {
        // Read input every frame in Update so no key press is ever missed,
        // even though the actual movement is applied on the physics tick.
        moveDir = Vector3.zero;

        if (Keyboard.current.wKey.isPressed)
            moveDir += transform.forward;

        if (Keyboard.current.sKey.isPressed)
            moveDir -= transform.forward;

        if (Keyboard.current.aKey.isPressed)
            moveDir -= transform.right;

        if (Keyboard.current.dKey.isPressed)
            moveDir += transform.right;

        moveDir = moveDir.normalized;

        // Animation is purely visual, so it's fine to update every frame.
        animator.SetFloat("Speed", moveDir.magnitude);
    }

    void FixedUpdate()
    {
        // Drive the Rigidbody directly instead of moving transform.position.
        // This is the key fix: jump.cs also controls this Rigidbody (for jumping
        // and gravity), so movement has to go through the same Rigidbody instead
        // of fighting it with direct transform edits. Only the horizontal (x/z)
        // velocity is set here — the vertical (y) velocity is left untouched so
        // jumping and falling, handled in jump.cs, keep working correctly.
        Vector3 horizontalVelocity = moveDir * speed;
        rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
    }
}