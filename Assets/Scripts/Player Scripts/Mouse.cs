using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    [Header("Settings")]
    public float sensitivity = 2f;
    public float minVerticalAngle = -80f;
    public float maxVerticalAngle = 80f;

    [Header("Lean Settings")]
    public float leanAngle = 45f;
    public float leanSpeed = 8f;

    private float xRotation; // vertical
    private float yRotation; // horizontal
    private float targetZRotation = 0f; // for lean

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 current = transform.eulerAngles;
        xRotation = current.x;
        yRotation = current.y;
    }

    void Update()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // Horizontal (left/right)
        yRotation += mouseDelta.x * sensitivity;

        // Vertical (up/down)
        xRotation -= mouseDelta.y * sensitivity;
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);

        // --- Lean hold ---
        if (Keyboard.current.qKey.isPressed)
            targetZRotation = leanAngle;
        else if (Keyboard.current.eKey.isPressed)
            targetZRotation = -leanAngle;
        else
            targetZRotation = 0f;

        // Smoothly lerp Z rotation
        float currentZ = Mathf.LerpAngle(transform.eulerAngles.z, targetZRotation, Time.deltaTime * leanSpeed);

        // Apply rotation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, currentZ);
    }
}