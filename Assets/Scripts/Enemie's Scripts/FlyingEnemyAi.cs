using UnityEngine;

public class DroneEnemyAI : MonoBehaviour
{
    [Header("Detection")]
    public LayerMask whatIsPlayer;

    public Transform player;
    private AttributesManager attributes;

    [Header("Movement")]
    public float moveSpeed = 6f;
    public float rotationSpeed = 8f;
    public float hoverHeight = 5f;

    [Header("Ranges")]
    public float sightRange = 30f;
    public float attackRange = 12f;

    [Header("Weapon")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    public int projectileDamage = 10;
    public float projectileSpeed = 50f;

    [Header("Attack")]
    public float timeBetweenShots = 0.35f;

    private bool alreadyShot;

    private enum State
    {
        Idle,
        Chase,
        Attack
    }

    private State currentState;

    void Awake()
    {
        attributes = GetComponent<AttributesManager>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");

        if (p != null)
            player = p.transform;
    }

    void Update()
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
                Hover();
                break;

            case State.Chase:
                ChasePlayer();
                break;

            case State.Attack:
                AttackPlayer();
                break;
        }
    }

    //================ IDLE ================

    void Hover()
    {
        MaintainHeight();
    }

    //================ CHASE ================

    void ChasePlayer()
    {
        MaintainHeight();

        Vector3 targetPosition = player.position;
        targetPosition.y = hoverHeight;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        FacePlayer();
    }

    //================ ATTACK ================

    void AttackPlayer()
    {
        MaintainHeight();

        FacePlayer();

        if (!alreadyShot)
        {
            Shoot();

            alreadyShot = true;

            Invoke(nameof(ResetAttack), timeBetweenShots);
        }
    }

    //================ SHOOT ================

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
            return;

        GameObject bullet = Instantiate(
            projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );

        EnemyProjectile projectile =
            bullet.GetComponent<EnemyProjectile>();

        if (projectile != null)
        {
            projectile.SetDamage(projectileDamage);
            projectile.SetSpeed(projectileSpeed);
        }
    }

    //================ ROTATION ================

    void FacePlayer()
    {
        Vector3 direction =
            player.position - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    //================ HEIGHT ================

    void MaintainHeight()
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.Lerp(
            pos.y,
            hoverHeight,
            Time.deltaTime * 2f
        );

        transform.position = pos;
    }

    //================ RESET ================

    void ResetAttack()
    {
        alreadyShot = false;
    }

    //================ GIZMOS ================

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