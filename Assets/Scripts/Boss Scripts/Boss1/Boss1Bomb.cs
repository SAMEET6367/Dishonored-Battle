using UnityEngine;

public class Boss1Bomb : MonoBehaviour
{
    [Header("Bomb Settings")]
    public int damage = 20;
    public float destroyAfter = 10f;

    [Header("Movement")]
    public float bombDropSpeed = 10f;
    public Vector3 bombRotationSpeed = new Vector3(180f, 180f, 180f);

    private Rigidbody rb;
    private bool exploded = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearVelocity = Vector3.down * bombDropSpeed;
        }

        Destroy(gameObject, destroyAfter);
    }

    private void Update()
    {
        transform.Rotate(bombRotationSpeed * Time.deltaTime, Space.Self);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (exploded)
            return;

        exploded = true;

        // Look for AttributesManager on the hit object or any parent
        AttributesManager attributes = collision.collider.GetComponentInParent<AttributesManager>();

        if (attributes != null)
        {
            attributes.TakeDamage(damage);
            Debug.Log("Bomb dealt " + damage + " damage to " + attributes.gameObject.name);
        }

        Destroy(gameObject);
    }
}