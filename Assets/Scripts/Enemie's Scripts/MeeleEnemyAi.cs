using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MeleeEnemyAI : MonoBehaviour
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
    public float attackRange = 2.5f;

    [Header("Patrol")]
    public float walkPointRange = 10f;

    private Vector3 walkPoint;
    private bool walkPointSet;

    [Header("Melee")]
    public int damage = 20;
    public float attackCooldown = 1.2f;

    private bool alreadyAttacked;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip walkSound;
    public AudioClip attackSound;

    private bool attackSoundPlaying = false;

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
            whatIsPlayer);

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
                player.position);

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

    //================= PATROL =================

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
            bool moving =
                !agent.isStopped &&
                agent.velocity.sqrMagnitude > 0.01f;

            animator.SetBool("IsMoving", moving);
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
        float randomX =
            Random.Range(-walkPointRange, walkPointRange);

        float randomZ =
            Random.Range(-walkPointRange, walkPointRange);

        Vector3 randomPoint = new Vector3(
            transform.position.x + randomX,
            transform.position.y,
            transform.position.z + randomZ);

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

    //================= CHASE =================

    private void ChasePlayer()
    {
        if (agent == null) return;

        if (animator != null)
        {
            bool moving =
                !agent.isStopped &&
                agent.velocity.sqrMagnitude > 0.01f;

            animator.SetBool("IsMoving", moving);
            animator.SetBool("IsAttacking", false);
        }

        walkPointSet = false;

        agent.isStopped = false;
        agent.updateRotation = true;
        agent.stoppingDistance = attackRange - 0.3f;

        agent.SetDestination(player.position);
    }

    //================= ATTACK =================

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
            // Play the attack sound only if the previous one has finished
            if (!attackSoundPlaying &&
                audioSource != null &&
                attackSound != null)
            {
                StartCoroutine(PlayAttackSound());
            }

            AttributesManager playerAttributes =
                player.GetComponent<AttributesManager>();

            if (playerAttributes != null)
            {
                playerAttributes.TakeDamage(damage);
            }

            alreadyAttacked = true;

            Invoke(nameof(ResetAttack), attackCooldown);
        }
    }

    private IEnumerator PlayAttackSound()
    {
        attackSoundPlaying = true;

        // Stop footsteps if they are playing
        if (audioSource.isPlaying &&
            audioSource.clip == walkSound)
        {
            audioSource.Stop();
            audioSource.clip = null;
            audioSource.loop = false;
        }

        audioSource.PlayOneShot(attackSound);

        // Wait until the ENTIRE clip has finished
        yield return new WaitForSeconds(attackSound.length);

        attackSoundPlaying = false;
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
            Time.deltaTime * 20f);
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
            if (audioSource.isPlaying &&
                audioSource.clip == walkSound)
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

        // Stop all sounds immediately
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = null;
            audioSource.loop = false;
        }

        // Stop attack sound coroutine if it's running
        StopAllCoroutines();
        attackSoundPlaying = false;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        animator.SetBool("IsMoving", false);
        animator.SetBool("IsAttacking", false);

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

        enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position,
            sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position,
            attackRange);
    }
}