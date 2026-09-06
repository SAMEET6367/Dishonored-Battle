using UnityEngine;
using UnityEngine.InputSystem;

public class AttackPacerLongLegs : MonoBehaviour
{
    [Header("General")]
    public LayerMask enemyLayer;

    private AttributesManager attributes;
    private Animator animator;
    private Collider playerCollider;

    [Header("Gun (LMB) - Shotgun")]
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float projectileSpeed = 25f;
    public float shootDamageMultiplier = 2f;
    public float fireRate = 0.5f;

    [Header("Ammo")]
    public int maxShots = 6;
    public float reloadTime = 2f;

    private int currentShots;
    private bool isReloading = false;

    [Header("Shotgun Settings")]
    public int pelletCount = 7;
    public float spreadAngle = 5f;

    [Header("Recoil Settings")]
    public Camera firstPersonCamera;
    public Camera thirdPersonCamera;
    public float firstPersonVerticalRecoil = 4f;
    public float firstPersonHorizontalRecoil = 2f;
    public float thirdPersonRecoilAngle = 1.5f;
    public float recoilRecoverySpeed = 8f;

    private float nextShootTime;

    private Vector2 recoilAmount;

    private void Awake()
    {
        attributes = GetComponent<AttributesManager>();
        animator = GetComponentInChildren<Animator>();
        playerCollider = GetComponent<Collider>();

        currentShots = maxShots;

        if (attributes == null)
            Debug.LogError("Player has NO AttributesManager!");

        if (animator == null)
            Debug.LogError("No Animator found!");
    }

    private void Update()
    {
        // SHOOT
        if (Mouse.current.leftButton.isPressed)
            TryShoot();

        ApplyRecoilRecovery();
    }

    // =========================
    // SHOOT
    // =========================
    void TryShoot()
    {
        if (isReloading) return;

        if (Time.time < nextShootTime) return;

        if (currentShots <= 0)
        {
            StartReload();
            return;
        }

        if (projectilePrefab == null || shootPoint == null) return;

        nextShootTime = Time.time + fireRate;

        animator.SetTrigger("Shoot");

        currentShots--;

        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 spawnPos = shootPoint.position + shootPoint.forward * 0.1f * i;

            Quaternion spreadRotation = shootPoint.rotation *
                Quaternion.Euler(
                    Random.Range(-spreadAngle, spreadAngle),
                    Random.Range(-spreadAngle, spreadAngle),
                    0f
                );

            GameObject projectile = Instantiate(projectilePrefab, spawnPos, spreadRotation);

            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null)
                rb.linearVelocity = spreadRotation * Vector3.forward * projectileSpeed;

            // Ignore collision with player
            if (playerCollider != null)
            {
                Collider projCollider = projectile.GetComponent<Collider>();

                if (projCollider != null)
                    Physics.IgnoreCollision(projCollider, playerCollider);
            }

            // Ignore collision with other projectiles
#if UNITY_2023_1_OR_NEWER
            Projectile[] existingProjectiles = GameObject.FindObjectsByType<Projectile>(FindObjectsSortMode.None);
#else
            Projectile[] existingProjectiles = FindObjectsOfType<Projectile>();
#endif

            Collider projCol = projectile.GetComponent<Collider>();

            if (projCol != null)
            {
                foreach (Projectile p in existingProjectiles)
                {
                    Collider otherCol = p.GetComponent<Collider>();

                    if (otherCol != null && otherCol != projCol)
                        Physics.IgnoreCollision(projCol, otherCol);
                }
            }

            // Damage
            Projectile projScript = projectile.GetComponent<Projectile>();

            if (projScript != null)
            {
                int damage = Mathf.RoundToInt(attributes.attack * shootDamageMultiplier);
                projScript.SetDamage(damage);
            }
        }

        ApplyRecoil();

        if (currentShots <= 0)
        {
            StartReload();
        }
    }

    // =========================
    // RELOAD
    // =========================
    void StartReload()
    {
        if (isReloading) return;

        isReloading = true;

        animator.SetTrigger("Reload");

        Invoke(nameof(FinishReload), reloadTime);
    }

    void FinishReload()
    {
        currentShots = maxShots;
        isReloading = false;
    }

    // =========================
    // RECOIL SYSTEM
    // =========================
    private void ApplyRecoil()
    {
        if (firstPersonCamera != null)
        {
            float verticalKick = Random.Range(
                firstPersonVerticalRecoil * 0.8f,
                firstPersonVerticalRecoil
            );

            float horizontalKick = Random.Range(
                -firstPersonHorizontalRecoil,
                 firstPersonHorizontalRecoil
            );

            recoilAmount += new Vector2(verticalKick, horizontalKick);
        }

        if (thirdPersonCamera != null)
        {
            float wiggleX = Random.Range(
                -thirdPersonRecoilAngle,
                 thirdPersonRecoilAngle
            );

            float wiggleY = Random.Range(
                -thirdPersonRecoilAngle,
                 thirdPersonRecoilAngle
            );

            thirdPersonCamera.transform.localRotation *=
                Quaternion.Euler(wiggleX, wiggleY, 0f);
        }
    }

    private void ApplyRecoilRecovery()
    {
        if (firstPersonCamera != null)
        {
            recoilAmount = Vector2.Lerp(
                recoilAmount,
                Vector2.zero,
                Time.deltaTime * recoilRecoverySpeed
            );

            firstPersonCamera.transform.localRotation =
                Quaternion.Euler(-recoilAmount.x, recoilAmount.y, 0f);
        }
    }
}