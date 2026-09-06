using UnityEngine;

public class FollowParent : MonoBehaviour
{
    [Header("Parent Settings")]
    public Transform parent;               // The parent to follow
    public bool followRotation = true;     // Should the child rotate with the parent?

    [Header("Axis Lock (Only used if followRotation = false)")]
    public bool lockX = true;
    public bool lockY = true;
    public bool lockZ = true;

    private Quaternion initialLocalRotation;

    void Start()
    {
        if (parent == null)
        {
            Debug.LogWarning("FollowParent: Parent is not assigned!");
            return;
        }

        // Save initial local rotation relative to parent
        initialLocalRotation = transform.localRotation;
    }

    void LateUpdate()
    {
        if (parent == null) return;

        // Always follow position
        transform.position = parent.position;

        if (followRotation)
        {
            // Rotate exactly like parent
            transform.rotation = parent.rotation;
        }
        else
        {
            // Keep initial rotation, optionally locking axes
            Vector3 currentEuler = transform.localEulerAngles;
            Vector3 initialEuler = initialLocalRotation.eulerAngles;

            if (lockX) currentEuler.x = initialEuler.x;
            if (lockY) currentEuler.y = initialEuler.y;
            if (lockZ) currentEuler.z = initialEuler.z;

            transform.localEulerAngles = currentEuler;
        }
    }
}
