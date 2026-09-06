using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private int damage;
    private Rigidbody rb;

    public float lifetime = 999f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        Destroy(gameObject, lifetime);
    }

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    public void SetSpeed(float speed)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (rb != null)
            rb.linearVelocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        AttributesManager target =
            other.GetComponentInParent<AttributesManager>();

        if (target != null && other.CompareTag("Player"))
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}