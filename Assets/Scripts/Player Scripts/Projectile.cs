using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int damage;
    public float lifetime = 3f;

    private void Start()
    {
        Destroy(gameObject, lifetime);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Collider playerCol = player.GetComponent<Collider>();
            Collider myCol = GetComponent<Collider>();

            if (playerCol != null && myCol != null)
                Physics.IgnoreCollision(myCol, playerCol);
        }

#if UNITY_2023_1_OR_NEWER
        Projectile[] existingProjectiles = GameObject.FindObjectsByType<Projectile>(FindObjectsSortMode.None);
#else
        Projectile[] existingProjectiles = FindObjectsOfType<Projectile>();
#endif

        Collider myCollider = GetComponent<Collider>();

        if (myCollider != null)
        {
            foreach (Projectile proj in existingProjectiles)
            {
                Collider otherCol = proj.GetComponent<Collider>();

                if (otherCol != null && otherCol != myCollider)
                    Physics.IgnoreCollision(myCollider, otherCol);
            }
        }
    }

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }

    private void OnCollisionEnter(Collision collision)
    {
        AttributesManager enemy =
            collision.collider.GetComponentInParent<AttributesManager>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}