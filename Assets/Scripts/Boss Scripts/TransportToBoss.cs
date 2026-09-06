using UnityEngine;

public class PlayerTeleporter : MonoBehaviour
{
    [Header("Teleport Destination")]
    public Transform teleportDestination;

    private int playerLayer;

    private void Awake()
    {
        playerLayer = LayerMask.NameToLayer("whatIsPlayer");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            CharacterController controller = other.GetComponent<CharacterController>();
            if (controller != null)
                controller.enabled = false;

            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            other.transform.position = teleportDestination.position;
            other.transform.rotation = teleportDestination.rotation;

            if (controller != null)
                controller.enabled = true;
        }
    }
}