using UnityEngine;
using UnityEngine.AI;

public class ShotgunEnemyAI : MonoBehaviour
{
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;

    public NavMeshAgent agent;
    public Transform player;

    private AttributesManager attributes;

    [Header("Animation")]
    public Animator animator;
    private bool wasInSightRange = false;

    [Header("Detection")]
    public float sightRange = 15f;
    public float attackRange = 7f;

    [Header("Patrol")]
    public float walkPointRange = 10f;

    private Vector3 walkPoint;
    private bool walkPointSet;

    [Header("Shotgun")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    public int pelletCount = 8;
    public float pelletSpread = 0.08f;

    [Header("Projectile Stats")]
    public int pelletDamage = 10;
    public float projectileSpeed = 30f;

    [Header("Attack")]
    public float timeBetweenAttacks = 1.2f;

    private bool alreadyAttacked;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip walkSound;
    public AudioClip shootSound;

    private enum State
    {
        Idle,
        Chase,
        Attack
    }

    private State currentState;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        attributes = GetComponent<AttributesManager>();

        if (animator == null)
            animator = GetComponent<Animator>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");

        if (p != null)
            player = p.transform;
    }

    private void Update()
    {
        if (attributes != null && attributes.IsDead())
            return;

        if (player == null)
            return;

        bool playerInSight = Physics.CheckSphere(
            transform.position,
            sightRange,
            whatIsPlayer
        );

        if (playerInSight && !wasInSightRange)
        {
            if (animator != null)
                animator.SetTrigger("Alert");
        }

        wasInSightRange = playerInSight;

        if (!playerInSight)
        {
            currentState = State.Idle;
        }
        else
        {
            float distance = Vector3.Distance(
                transform.position,
                player.position
            );

            if (distance > attackRange)
                currentState = State.Chase;
            else
                currentState = State.Attack;
        }

        switch (currentState)
        {
            case State.Idle:
                Patroling();
                break;

            case State.Chase:
                ChasePlayer();
                break;

            case State.Attack:
                AttackPlayer();
                break;
        }

        HandleWalkingSound();
    }

    //================ PATROL ================

    private void Patroling()
    {
        if (agent == null) return;

        agent.isStopped = false;

        if (!walkPointSet)
            SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        if (animator != null)
        {
            bool isMoving =
                !agent.isStopped &&
                agent.velocity.sqrMagnitude > 0.01f;

            animator.SetBool("IsMoving", isMoving);
            animator.SetBool("IsAttacking", false);
        }

        if (!agent.pathPending &&
            agent.remainingDistance <= 1f)
        {
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        float randomZ = Random.Range(-walkPointRange, walkPointRange);

        Vector3 randomPoint = new Vector3(
            transform.position.x + randomX,
            transform.position.y,
            transform.position.z + randomZ
        );

        NavMeshHit hit;

        if (NavMesh.SamplePosition(
            randomPoint,
            out hit,
            5f,
            NavMesh.AllAreas))
        {
            walkPoint = hit.position;
            walkPointSet = true;
        }
    }

    //================ CHASE ================

    private void ChasePlayer()
    {
        if (agent == null) return;

        if (animator != null)
        {
            bool isMoving =
                !agent.isStopped &&
                agent.velocity.sqrMagnitude > 0.01f;

            animator.SetBool("IsMoving", isMoving);
            animator.SetBool("IsAttacking", false);
        }

        walkPointSet = false;

        agent.isStopped = false;
        agent.updateRotation = true;
        agent.stoppingDistance = attackRange - 1f;

        agent.SetDestination(player.position);
    }

    //================ ATTACK ================

    private void AttackPlayer()
    {
        if (agent == null) return;

        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
            animator.SetBool("IsAttacking", true);
        }

        agent.isStopped = true;
        agent.updateRotation = false;

        FaceTarget(player.position);

        if (!alreadyAttacked)
        {
            ShootShotgun();

            if (audioSource != null && shootSound != null)
                audioSource.PlayOneShot(shootSound);

            alreadyAttacked = true;

            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ShootShotgun()
    {
        if (projectilePrefab == null || firePoint == null)
            return;

        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 direction = firePoint.forward;

            direction += firePoint.right *
                Random.Range(-pelletSpread, pelletSpread);

            direction += firePoint.up *
                Random.Range(-pelletSpread, pelletSpread);

            direction.Normalize();

            GameObject bullet = Instantiate(
                projectilePrefab,
                firePoint.position,
                Quaternion.LookRotation(direction)
            );

            EnemyProjectile projectile =
                bullet.GetComponent<EnemyProjectile>();

            if (projectile != null)
            {
                projectile.SetDamage(pelletDamage);
                projectile.SetSpeed(projectileSpeed);
            }
        }
    }

    private void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction =
            targetPosition - transform.position;

        direction.y = 0;

        if (direction.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * 50f
        );
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;

        if (animator != null)
            animator.SetBool("IsAttacking", false);
    }

    private void HandleWalkingSound()
    {
        if (audioSource == null || walkSound == null)
            return;

        bool walking =
            currentState == State.Chase &&
            !agent.isStopped &&
            agent.velocity.sqrMagnitude > 0.01f;

        if (walking)
        {
            if (!audioSource.isPlaying || audioSource.clip != walkSound)
            {
                audioSource.clip = walkSound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying && audioSource.clip == walkSound)
            {
                audioSource.Stop();
                audioSource.clip = null;
                audioSource.loop = false;
            }
        }
    }

    public void PlayRandomDeathAnimation()
    {
        if (animator == null)
            return;

        int randomDeath = Random.Range(1, 4);

        switch (randomDeath)
        {
            case 1:
                animator.SetTrigger("Death1");
                break;

            case 2:
                animator.SetTrigger("Death2");
                break;

            case 3:
                animator.SetTrigger("Death3");
                break;
        }

        // Stop walking sound immediately
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.clip = null;
            audioSource.loop = false;
        }

        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position,
            sightRange
        );

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );
    }
}