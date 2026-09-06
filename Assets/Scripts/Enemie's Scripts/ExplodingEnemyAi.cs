using UnityEngine;
using System.Collections;

public class ExplodingEnemyAI : MonoBehaviour
{
    [Header("Detection")]
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;
    public LayerMask damageLayers;

    [Header("References")]
    public Transform player;
    private AttributesManager attributes;

    [Header("Movement")]
    public float moveSpeed = 4f;
    public float sightRange = 15f;

    [Header("Patrol")]
    public float walkPointRange = 10f;

    private Vector3 walkPoint;
    private bool walkPointSet;

    [Header("Explosion")]
    public float explodeDistance = 2.5f;
    public float explosionRadius = 4f;

    public int explosionDamage = 50;
    public int deathExplosionDamage = 25;

    public GameObject explosionEffect;
    public AudioSource audioSource;
    public AudioClip explosionSound;

    [Header("Explosion Visual")]
    public float explosionScale = 6f;
    public float explosionGrowTime = 0.2f;
    public float explosionLifetime = 2f;

    private bool exploding = false;

    private enum State
    {
        Patrol,
        Chase
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
        if (exploding)
            return;

        if (attributes != null && attributes.IsDead())
            return;

        if (player == null)
            return;

        bool playerInSight = Physics.CheckSphere(
            transform.position,
            sightRange,
            whatIsPlayer
        );

        currentState = playerInSight ? State.Chase : State.Patrol;

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;

            case State.Chase:
                ChasePlayer();
                break;
        }
    }

    //================ PATROL ================

    void Patrol()
    {
        if (!walkPointSet)
            SearchWalkPoint();

        if (walkPointSet)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                walkPoint,
                moveSpeed * Time.deltaTime
            );

            FaceTarget(walkPoint);

            if (Vector3.Distance(transform.position, walkPoint) < 0.5f)
                walkPointSet = false;
        }
    }

    void SearchWalkPoint()
    {
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        float randomZ = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(
            transform.position.x + randomX,
            transform.position.y,
            transform.position.z + randomZ
        );

        walkPointSet = true;
    }

    //================ CHASE ================

    void ChasePlayer()
    {
        walkPointSet = false;

        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime
        );

        FaceTarget(player.position);

        if (Vector3.Distance(transform.position, player.position) <= explodeDistance)
        {
            Explode(true);
        }
    }

    //================ EXPLOSION ================

    public void Explode(bool reachedPlayer)
    {
        if (exploding)
            return;

        StartCoroutine(ExplosionRoutine(reachedPlayer));
    }

    IEnumerator ExplosionRoutine(bool reachedPlayer)
    {
        exploding = true;

        int damage = reachedPlayer ?
            explosionDamage :
            deathExplosionDamage;

        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            explosionRadius,
            damageLayers
        );

        foreach (Collider hit in hits)
        {
            AttributesManager target =
                hit.GetComponentInParent<AttributesManager>();

            if (target != null)
                target.TakeDamage(damage);
        }

        GameObject effect = null;

        if (explosionEffect != null)
        {
            effect = Instantiate(
                explosionEffect,
                transform.position,
                Quaternion.identity
            );

            StartCoroutine(ScaleExplosion(effect));

            // Destroy explosion after its lifetime
            Destroy(effect, explosionLifetime);
        }

        if (audioSource != null && explosionSound != null)
            audioSource.PlayOneShot(explosionSound);

        // Hide enemy mesh
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
            r.enabled = false;

        // Disable all colliders
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
            c.enabled = false;

        // Stop this script
        enabled = false;

        // Destroy enemy after explosion finishes
        Destroy(gameObject, explosionLifetime);

        yield break;
    }

    IEnumerator ScaleExplosion(GameObject effect)
    {
        if (effect == null)
            yield break;

        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one * explosionScale;

        effect.transform.localScale = startScale;

        float timer = 0f;

        while (timer < explosionGrowTime)
        {
            timer += Time.deltaTime;

            effect.transform.localScale = Vector3.Lerp(
                startScale,
                endScale,
                timer / explosionGrowTime
            );

            yield return null;
        }

        effect.transform.localScale = endScale;
    }

    //================ FACE TARGET ================

    void FaceTarget(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * 8f
        );
    }

    //================ GIZMOS ================

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position,
            sightRange
        );

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position,
            explodeDistance
        );

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(
            transform.position,
            explosionRadius
        );
    }
}