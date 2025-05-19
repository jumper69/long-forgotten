using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 5f;
    public int damage = 10;
    public KeyCode attackKey = KeyCode.Space;
    public LayerMask enemyLayer;
    public AudioClip hitSound;
    private AudioSource audioSource;

    public bool strongAttackUnlocked = false;
    public int strongAttackDamage = 35;
    public float strongAttackRange = 2f;
    public float strongAttackCooldown = 1f;
    public KeyCode strongAttackKey = KeyCode.R;
    private bool canStrongAttack = true;

    public Animator animator;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        strongAttackUnlocked = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(attackKey))
        {
            AttackNearbyEnemies(damage, attackRange);
            animator.SetTrigger("isAttacking");
        }

        if (strongAttackUnlocked && Input.GetKeyDown(strongAttackKey) && canStrongAttack)
        {
            StartCoroutine(PerformStrongAttack());
        }
    }

    void AttackNearbyEnemies(int dmg, float range)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                HealthSystem enemyHealth = hit.GetComponent<HealthSystem>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(dmg);
                    if (hitSound != null && audioSource != null)
                    audioSource.PlayOneShot(hitSound);
                    Debug.Log("Enemy hit with " + dmg);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    IEnumerator PerformStrongAttack()
    {
        canStrongAttack = false;
        animator.SetBool("StrongAttack", true);

        AttackNearbyEnemies(strongAttackDamage, strongAttackRange);

        yield return new WaitForSeconds(0.5f);
        animator.SetBool("StrongAttack", false);

        yield return new WaitForSeconds(strongAttackCooldown);
        canStrongAttack = true;
    }
}
