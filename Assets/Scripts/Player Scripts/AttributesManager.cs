using UnityEngine;

public class AttributesManager : MonoBehaviour
{
    [Header("Attributes")]
    public int health = 100;
    public int attack = 15;

    public HealthBar healthBar;

    private bool isDead = false;

    [Header("Fall Death")]
    public float deathYLevel = -20f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip deathSound;

    [Header("Death Icon")]
    public GameObject deathIconPrefab;
    public float deathIconHeight = 2f;
    public float deathIconLifetime = 3f;

    private void Start()
    {
        if (healthBar != null)
            healthBar.SetMaxHealth(health);
    }

    private void Update()
    {
        if (!isDead && transform.position.y <= deathYLevel)
        {
            health = 0;
            Die();
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead)
            return;

        health -= amount;

        if (health < 0)
            health = 0;

        Debug.Log(gameObject.name + " Health: " + health);

        if (healthBar != null)
            healthBar.SetHealth(health);

        if (health <= 0)
            Die();
    }

    public void DealDamage(GameObject target)
    {
        DealDamage(target, attack);
    }

    public void DealDamage(GameObject target, int damageAmount)
    {
        if (isDead || target == null)
            return;

        AttributesManager targetAttributes = target.GetComponent<AttributesManager>();

        if (targetAttributes != null)
        {
            targetAttributes.TakeDamage(damageAmount);
        }
    }

    public void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log(gameObject.name + " died");

        // Spawn death icon
        if (deathIconPrefab != null)
        {
            GameObject icon = Instantiate(
                deathIconPrefab,
                transform.position + Vector3.up * deathIconHeight,
                Quaternion.identity
            );

            Destroy(icon, deathIconLifetime);
        }

        // Play death sound
        if (audioSource != null && deathSound != null)
            audioSource.PlayOneShot(deathSound);

        // Shotgun Enemy
        ShotgunEnemyAI shotgun = GetComponent<ShotgunEnemyAI>();
        if (shotgun != null)
        {
            shotgun.PlayRandomDeathAnimation();
            Destroy(gameObject, 3f);
            return;
        }

        // Sniper Enemy
        SniperEnemyAI sniper = GetComponent<SniperEnemyAI>();
        if (sniper != null)
        {
            sniper.PlayRandomDeathAnimation();
            Destroy(gameObject, 3f);
            return;
        }

        // Machine Gun Enemy
        MachineGunEnemyAI machineGun = GetComponent<MachineGunEnemyAI>();
        if (machineGun != null)
        {
            machineGun.PlayRandomDeathAnimation();
            Destroy(gameObject, 3f);
            return;
        }

        // Melee Enemy
        MeleeEnemyAI melee = GetComponent<MeleeEnemyAI>();
        if (melee != null)
        {
            melee.PlayRandomDeathAnimation();
            Destroy(gameObject, 3f);
            return;
        }

        // Default destruction
        Destroy(gameObject, 0.2f);
    }

    public bool IsDead()
    {
        return isDead;
    }
}