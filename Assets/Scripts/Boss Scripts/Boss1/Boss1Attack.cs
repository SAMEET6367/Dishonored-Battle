using UnityEngine;

public class BossAttackLoop : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public AttributesManager attributes;

    [Header("Attack Settings")]
    public float timeBetweenAttacks = 2f;

    private int currentAttack = 1;
    private bool isDead = false;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (attributes == null)
            attributes = GetComponent<AttributesManager>();

        Invoke(nameof(StartAttackLoop), 0.5f);
    }

    void Update()
    {
        if (isDead)
            return;

        // Stop attacking if dead
        if (attributes != null && attributes.health <= 0)
        {
            isDead = true;
            CancelInvoke(nameof(PerformAttack));

            animator.ResetTrigger("Attack1");
            animator.ResetTrigger("Attack2");
            animator.ResetTrigger("Attack3");

            animator.SetTrigger("Death");
        }
    }

    void StartAttackLoop()
    {
        InvokeRepeating(nameof(PerformAttack), 0f, timeBetweenAttacks);
    }

    void PerformAttack()
    {
        if (isDead)
            return;

        switch (currentAttack)
        {
            case 1:
                animator.SetTrigger("Attack1");
                currentAttack = 2;
                break;

            case 2:
                animator.SetTrigger("Attack2");
                currentAttack = 3;
                break;

            case 3:
                animator.SetTrigger("Attack3");
                currentAttack = 1;
                break;
        }
    }
}