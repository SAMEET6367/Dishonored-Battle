using UnityEngine;
using System.Collections;

public class BrainBot : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform firePoint;
    public GameObject projectilePrefab;

    public AudioSource audioSource;
    public AudioClip attackSound;

    private AttributesManager attributes;

    [Header("Attack Settings")]
    public float attackRange = 25f;
    public float attackCooldown = 4f;
    public float projectileSpeed = 18f;

    [Header("Projectile Growth")]
    public float growTime = 1.2f;
    public float maxScale = 2f;

    [Header("Rotation")]
    public float rotationSpeed = 8f;

    private bool alreadyAttacking;

    private void Start()
    {
        attributes = GetComponent<AttributesManager>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        if (attributes != null && attributes.IsDead())
            return;

        RotateTowardsPlayer();

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange && !alreadyAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private IEnumerator Attack()
    {
        alreadyAttacking = true;

        // Spawn projectile
        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );

        projectile.transform.localScale = Vector3.zero;

        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
            rb.isKinematic = true;

        // Hold projectile while charging
        projectile.transform.SetParent(firePoint);

        float timer = 0f;

        while (timer < growTime)
        {
            timer += Time.deltaTime;

            float t = timer / growTime;

            projectile.transform.localScale = Vector3.Lerp(
                Vector3.zero,
                Vector3.one * maxScale,
                t
            );

            yield return null;
        }

        // Release projectile
        projectile.transform.SetParent(null);

        // Aim at player
        Vector3 direction = (player.position - projectile.transform.position).normalized;
        projectile.transform.forward = direction;

        // Play attack sound
        if (audioSource != null && attackSound != null)
            audioSource.PlayOneShot(attackSound);

        // Enable Rigidbody movement
        if (rb != null)
            rb.isKinematic = false;

        // Use your existing EnemyProjectile script
        EnemyProjectile enemyProjectile = projectile.GetComponent<EnemyProjectile>();

        if (enemyProjectile != null && attributes != null)
        {
            enemyProjectile.SetDamage(attributes.attack);
            enemyProjectile.SetSpeed(projectileSpeed);
        }

        yield return new WaitForSeconds(attackCooldown);

        alreadyAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}