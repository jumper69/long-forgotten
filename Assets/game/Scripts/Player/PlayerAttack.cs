using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 5f;
    public int damage = 10;
    public KeyCode attackKey = KeyCode.Space;

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
            Debug.Log("Hit: " + hit.name);

            HealthSystem enemyHealth = hit.GetComponent<HealthSystem>();
            if (enemyHealth != null)
            {
                Debug.Log("Enemy with health found!");
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
