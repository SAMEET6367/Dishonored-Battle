using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    [Header("References")]
    public LayerMask enemyLayer;
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;

    private AttributesManager attributes;
    private Animator animator;

    // =========================
    // SLASH
    // =========================
    [Header("Slash")]
    public float slashRange = 2f;
    public float slashCooldown = 0.8f;
    private float nextSlashTime;

    // =========================
    // THROW
    // =========================
    [Header("Throw")]
    public float throwForce = 20f;
    public float throwCooldown = 0.5f;
    private float nextThrowTime;

    // =========================
    // KICK
    // =========================
    [Header("Kick")]
    public float kickRange = 1.5f;
    public float kickCooldown = 1f;
    private float nextKickTime;

    private void Awake()
    {
        attributes = GetComponent<AttributesManager>();
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
            Debug.LogError("Animator missing!");
    }

    private void Update()
    {
        HandleInput();
    }

    // =========================
    // INPUT
    // =========================
    void HandleInput()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
            TryThrow();

        if (Mouse.current.rightButton.wasPressedThisFrame)
            TrySlash();

        if (Keyboard.current.fKey.wasPressedThisFrame)
            TryKick();
    }

    // =========================
    // ANIMATION CONTROL
    // =========================
    void SetAttacking(bool value)
    {
        if (animator != null)
            animator.SetBool("IsAttacking", value);
    }

    void StopAttacking()
    {
        SetAttacking(false);
    }

    // =========================
    // SLASH
    // =========================
    void TrySlash()
    {
        if (Time.time < nextSlashTime) return;

        nextSlashTime = Time.time + slashCooldown;

        SetAttacking(true);
        animator.SetTrigger("Slash");

        Invoke(nameof(StopAttacking), 0.5f);

        Collider[] hits = Physics.OverlapSphere(transform.position, slashRange, enemyLayer);

        foreach (Collider hit in hits)
        {
            AttributesManager enemy = hit.GetComponentInParent<AttributesManager>();

            if (enemy != null)
                enemy.TakeDamage(attributes.attack);
        }
    }

    // =========================
    // THROW
    // =========================
    void TryThrow()
    {
        if (Time.time < nextThrowTime) return;
        if (projectilePrefab == null || projectileSpawnPoint == null) return;

        nextThrowTime = Time.time + throwCooldown;

        SetAttacking(true);
        animator.SetTrigger("Throw");

        Invoke(nameof(StopAttacking), 0.5f);

        GameObject proj = Instantiate(
            projectilePrefab,
            projectileSpawnPoint.position,
            projectileSpawnPoint.rotation
        );

        Rigidbody rb = proj.GetComponent<Rigidbody>();

        if (rb != null)
            rb.linearVelocity = projectileSpawnPoint.forward * throwForce;

        Projectile p = proj.GetComponent<Projectile>();

        if (p != null)
            p.SetDamage(attributes.attack);
    }

    // =========================
    // KICK
    // =========================
    void TryKick()
    {
        if (Time.time < nextKickTime) return;

        nextKickTime = Time.time + kickCooldown;

        SetAttacking(true);
        animator.SetTrigger("Kick");

        Invoke(nameof(StopAttacking), 0.5f);

        Collider[] hits = Physics.OverlapSphere(transform.position, kickRange, enemyLayer);

        foreach (Collider hit in hits)
        {
            AttributesManager enemy = hit.GetComponentInParent<AttributesManager>();

            if (enemy != null)
                enemy.TakeDamage(Mathf.RoundToInt(attributes.attack * 1.5f));
        }
    }

    // =========================
    // DEBUG VISUALS
    // =========================
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, slashRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, kickRange);
    }
}