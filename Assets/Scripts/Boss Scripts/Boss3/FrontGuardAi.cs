using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class FrontGuardAI : MonoBehaviour
{
    public enum BossState
    {
        Chase,
        Attack,
        Stun,
        Punch
    }

    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public Animator animator;
    public AudioSource audioSource;

    private AttributesManager attributes;

    [Header("Detection")]
    public LayerMask whatIsPlayer;

    [Header("Ranges")]
    public float attackRange = 18f;
    public float punchRange = 3f;

    [Header("Machine Gun")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 40f;
    public float fireRate = 0.08f;

    [Header("Punch")]
    public float punchForce = 15f;
    public float punchCooldown = 2f;

    [Header("Stun")]
    public float minTimeBeforeStun = 8f;
    public float maxTimeBeforeStun = 18f;
    public float stunDuration = 4f;

    [Header("Sounds")]
    public AudioClip attackSound;
    public AudioClip stunSound;
    public AudioClip punchSound;

    private BossState currentState;

    private bool stunned = false;
    private bool playerInAttackRange;
    private bool playerInPunchRange;

    private float nextFire;
    private float nextPunch;

    void Start()
    {
        attributes = GetComponent<AttributesManager>();

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        currentState = BossState.Chase;

        StartCoroutine(RandomStunRoutine());
    }

    void Update()
    {
        if (attributes != null && attributes.IsDead())
            return;

        Collider[] attackHits = Physics.OverlapSphere(
            transform.position,
            attackRange,
            whatIsPlayer);

        Collider[] punchHits = Physics.OverlapSphere(
            transform.position,
            punchRange,
            whatIsPlayer);

        playerInAttackRange = attackHits.Length > 0;
        playerInPunchRange = punchHits.Length > 0;

        if (playerInAttackRange)
            player = attackHits[0].transform;
        else
            player = null;

        if (player == null)
        {
            if (agent != null)
                agent.isStopped = true;

            animator.SetBool("Chase", false);
            animator.SetBool("Attack", false);
            animator.SetBool("Stun", false);

            return;
        }

        Vector3 lookPos = player.position;
        lookPos.y = transform.position.y;
        transform.LookAt(lookPos);

        // Highest priority
        if (stunned)
        {
            ChangeState(BossState.Stun);
            return;
        }

        // Second priority
        if (playerInPunchRange && Time.time >= nextPunch)
        {
            ChangeState(BossState.Punch);
            return;
        }

        // Third priority
        if (playerInAttackRange)
        {
            ChangeState(BossState.Attack);
        }
        else
        {
            ChangeState(BossState.Chase);
        }

        switch (currentState)
        {
            case BossState.Chase:
                Chase();
                break;

            case BossState.Attack:
                Attack();
                break;

            case BossState.Stun:
                break;

            case BossState.Punch:
                break;
        }
    }

    void ChangeState(BossState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;

        animator.SetBool("Chase", false);
        animator.SetBool("Attack", false);
        animator.SetBool("Stun", false);

        switch (newState)
        {
            case BossState.Chase:
                animator.SetBool("Chase", true);
                break;

            case BossState.Attack:
                animator.SetBool("Attack", true);
                break;

            case BossState.Stun:
                animator.SetBool("Stun", true);
                break;

            case BossState.Punch:
                animator.SetTrigger("Punch");
                StartCoroutine(PunchRoutine());
                break;
        }
    }

    void Chase()
    {
        if (agent == null || player == null)
            return;

        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    void Attack()
    {
        if (agent == null || player == null)
            return;

        // Stop moving while firing
        agent.isStopped = true;

        // Keep facing the player
        Vector3 lookPos = player.position;
        lookPos.y = transform.position.y;
        transform.LookAt(lookPos);

        // Fire continuously
        if (Time.time >= nextFire)
        {
            Shoot();
            nextFire = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
            return;

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation
        );

        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // If your Unity version uses linearVelocity:
            rb.linearVelocity = firePoint.forward * bulletSpeed;

            // Otherwise use:
            // rb.velocity = firePoint.forward * bulletSpeed;
        }

        // Play attack sound
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    IEnumerator PunchRoutine()
    {
        if (agent != null)
            agent.isStopped = true;

        // Play punch sound
        if (audioSource != null && punchSound != null)
        {
            audioSource.PlayOneShot(punchSound);
        }

        // Wait until the punch animation reaches the hit frame
        yield return new WaitForSeconds(0.4f);

        if (player != null)
        {
            // Deal damage
            if (attributes != null)
            {
                attributes.DealDamage(player.gameObject);
            }

            // Apply knockback
            Rigidbody playerRb = player.GetComponent<Rigidbody>();

            if (playerRb != null)
            {
                Vector3 knockbackDirection =
                    (player.position - transform.position).normalized;

                playerRb.AddForce(
                    knockbackDirection * punchForce,
                    ForceMode.Impulse);
            }
        }

        nextPunch = Time.time + punchCooldown;

        // Wait for punch animation to finish
        yield return new WaitForSeconds(0.8f);

        // Resume normal behavior
        if (player != null)
        {
            if (playerInAttackRange)
                ChangeState(BossState.Attack);
            else
                ChangeState(BossState.Chase);
        }
    }

    IEnumerator RandomStunRoutine()
    {
        while (true)
        {
            // Wait a random amount of time before the next stun
            float waitTime = Random.Range(minTimeBeforeStun, maxTimeBeforeStun);
            yield return new WaitForSeconds(waitTime);

            // Stop if the boss has died
            if (attributes != null && attributes.IsDead())
                yield break;

            stunned = true;

            if (agent != null)
                agent.isStopped = true;

            // Play stun sound
            if (audioSource != null && stunSound != null)
            {
                audioSource.PlayOneShot(stunSound);
            }

            ChangeState(BossState.Stun);

            // Stay stunned
            yield return new WaitForSeconds(stunDuration);

            stunned = false;

            // Resume behavior if still alive
            if (attributes != null && !attributes.IsDead())
            {
                if (playerInAttackRange)
                    ChangeState(BossState.Attack);
                else
                    ChangeState(BossState.Chase);
            }
        }
    }

    public void PlayDeathAnimation()
    {
        StopAllCoroutines();

        if (agent != null)
        {
            agent.isStopped = true;

            if (agent.enabled)
                agent.enabled = false;
        }

        enabled = false;

        animator.SetBool("Chase", false);
        animator.SetBool("Attack", false);
        animator.SetBool("Stun", false);

        animator.SetTrigger("Death");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, punchRange);
    }
}