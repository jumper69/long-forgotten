using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 5f;
    public int damage = 10;
    public KeyCode attackKey = KeyCode.Space;
    public LayerMask enemyLayer;

    void Update()
    {
        if (Input.GetKeyDown(attackKey))
        {
            AttackNearbyEnemies();
        }
    }

    void AttackNearbyEnemies()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            HealthSystem enemyHealth = hit.GetComponent<HealthSystem>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
