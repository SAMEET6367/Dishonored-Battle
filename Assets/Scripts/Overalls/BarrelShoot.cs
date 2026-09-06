using UnityEngine;

public class HazardShooter : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject hazardPrefab;
    public float projectileSpeed = 10f;
    public float projectileLifetime = 5f;

    [Header("Fire Settings")]
    public float fireRate = 2f;
    private float fireCooldown = 0f;

    [Header("Direction")]
    public Vector3 shootDirection = Vector3.forward;

    [Header("Spread")]
    public float spreadAngle = 10f;

    [Header("Audio")]
    public AudioSource audioSource;     // assign AudioSource
    public AudioClip spawnSound;        // sound when spawning
    public float soundInterval = 0.2f;  // optional: prevents spam
    private float soundTimer = 0f;

    void Update()
    {
        fireCooldown -= Time.deltaTime;
        soundTimer += Time.deltaTime;

        if (fireCooldown <= 0f)
        {
            FireHazard();
            fireCooldown = fireRate;
        }
    }

    void FireHazard()
    {
        if (hazardPrefab == null) return;

        // Spawn projectile
        GameObject hazard = Instantiate(hazardPrefab, transform.position, transform.rotation);

        // Play spawn sound with interval check
        if (audioSource != null && spawnSound != null && soundTimer >= soundInterval)
        {
            audioSource.PlayOneShot(spawnSound);
            soundTimer = 0f;
        }

        // Apply spread
        Vector3 direction = transform.TransformDirection(shootDirection.normalized);

        float randomX = Random.Range(-spreadAngle, spreadAngle);
        float randomY = Random.Range(-spreadAngle, spreadAngle);

        Quaternion spreadRotation = Quaternion.Euler(randomX, randomY, 0f);
        direction = spreadRotation * direction;

        // Apply velocity
        Rigidbody rb = hazard.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
        }

        // Destroy after lifetime
        Destroy(hazard, projectileLifetime);
    }
}