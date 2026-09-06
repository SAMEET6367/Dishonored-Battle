using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour
{
    [Header("Layers")]
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;

    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    private AttributesManager attributes;

    [Header("Animation")]
    public Animator animator;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Ranges")]
    public float sightRange = 35f;
    public float attackRange = 18f;

    private bool playerInSightRange;
    private bool playerInAttackRange;

    [Header("Machine Gun")]
    public int machineGunShots = 15;
    public float machineGunFireRate = 0.12f;
    public float machineGunProjectileSpeed = 35f;

    [Header("Big Gun")]
    public float bigGunProjectileSpeed = 20f;
    public float bigProjectileScale = 3f;
    public float bigGunChargeTime = 0.8f;

    [Header("Cooldown")]
    public float attackCooldown = 2f;

    [Header("Rotation")]
    public float rotationSpeed = 8f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip chaseSound;
    public AudioClip machineGunSound;
    public AudioClip bigGunSound;

    private bool alreadyAttacking;
    private bool isDead;

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");

            if (p != null)
                player = p.transform;
        }

        attributes = GetComponent<AttributesManager>();
    }

    void Update()
    {
        if (isDead)
            return;

        if (player == null)
            return;

        playerInSightRange = Physics.CheckSphere(
            transform.position,
            sightRange,
            whatIsPlayer);

        playerInAttackRange = Physics.CheckSphere(
            transform.position,
            attackRange,
            whatIsPlayer);

        if (!playerInSightRange)
        {
            agent.isStopped = true;

            animator.SetBool("Chasing", false);
            animator.SetBool("MachineGun", false);
            animator.SetBool("BigGun", false);

            return;
        }

        if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
        }

        if (playerInAttackRange)
        {
            AttackPlayer();
        }
    }

    void ChasePlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);

        animator.SetBool("Chasing", true);
        animator.SetBool("MachineGun", false);
        animator.SetBool("BigGun", false);

        FacePlayer();

        if (audioSource != null &&
            chaseSound != null &&
            !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(chaseSound);
        }
    }

    void AttackPlayer()
    {
        agent.isStopped = true;

        FacePlayer();

        if (alreadyAttacking)
            return;

        StartCoroutine(RandomAttack());
    }

    IEnumerator RandomAttack()
    {
        alreadyAttacking = true;

        int attack = Random.Range(0, 2);

        if (attack == 0)
        {
            yield return StartCoroutine(MachineGunAttack());
        }
        else
        {
            yield return StartCoroutine(BigGunAttack());
        }

        yield return new WaitForSeconds(attackCooldown);

        alreadyAttacking = false;
    }

    void FacePlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;

        if (direction == Vector3.zero)
            return;

        Quaternion rotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rotation,
            rotationSpeed * Time.deltaTime);
    }

    IEnumerator MachineGunAttack()
    {
        animator.SetBool("Chasing", false);
        animator.SetBool("MachineGun", true);
        animator.SetBool("BigGun", false);

        if (audioSource != null && machineGunSound != null)
            audioSource.PlayOneShot(machineGunSound);

        for (int i = 0; i < machineGunShots; i++)
        {
            if (player == null)
                break;

            FacePlayer();

            FireProjectile(
                machineGunProjectileSpeed,
                1f
            );

            yield return new WaitForSeconds(machineGunFireRate);
        }

        animator.SetBool("MachineGun", false);
    }

    void FireProjectile(float speed, float scale)
    {
        if (projectilePrefab == null || firePoint == null || player == null)
            return;

        Vector3 targetPosition = player.position + Vector3.up * 1.2f;
        Vector3 direction = (targetPosition - firePoint.position).normalized;

        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(direction)
        );

        projectile.transform.localScale = Vector3.one * scale;

        EnemyProjectile enemyProjectile =
            projectile.GetComponent<EnemyProjectile>();

        if (enemyProjectile != null)
        {
            if (attributes != null)
                enemyProjectile.SetDamage(attributes.attack);

            enemyProjectile.SetSpeed(speed);
        }

        Physics.IgnoreCollision(
            projectile.GetComponent<Collider>(),
            GetComponent<Collider>()
        );
    }

    IEnumerator BigGunAttack()
    {
        animator.SetBool("Chasing", false);
        animator.SetBool("MachineGun", false);
        animator.SetBool("BigGun", true);

        if (audioSource != null && bigGunSound != null)
            audioSource.PlayOneShot(bigGunSound);

        // Charge up before firing
        yield return new WaitForSeconds(bigGunChargeTime);

        if (player != null)
        {
            FacePlayer();

            FireProjectile(
                bigGunProjectileSpeed,
                bigProjectileScale
            );
        }

        // Wait for the animation to finish
        yield return new WaitForSeconds(0.8f);

        animator.SetBool("BigGun", false);
    }

    // Called by AttributesManager when the boss dies
    public void PlayDeathAnimation()
    {
        if (isDead)
            return;

        isDead = true;

        agent.isStopped = true;
        agent.enabled = false;

        animator.SetBool("Chasing", false);
        animator.SetBool("MachineGun", false);
        animator.SetBool("BigGun", false);

        // Trigger your death animation
        animator.SetTrigger("Death");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}