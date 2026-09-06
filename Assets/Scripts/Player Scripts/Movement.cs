using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;

    private Animator animator;

    void Start()
    {
        // Get Animator from child mesh
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
            Debug.LogError("No Animator found on Player or its children!");
    }

    void Update()
    {
        Vector3 moveDir = Vector3.zero;

        // Input
        if (Keyboard.current.wKey.isPressed)
            moveDir += transform.forward;

        if (Keyboard.current.sKey.isPressed)
            moveDir -= transform.forward;

        if (Keyboard.current.aKey.isPressed)
            moveDir -= transform.right;

        if (Keyboard.current.dKey.isPressed)
            moveDir += transform.right;

        // Normalize movement
        moveDir = moveDir.normalized;

        // Move player
        transform.position += moveDir * speed * Time.deltaTime;

        // Animation
        animator.SetFloat("Speed", moveDir.magnitude);
    }
}