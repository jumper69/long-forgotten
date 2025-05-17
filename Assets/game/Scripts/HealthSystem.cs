using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public bool destroyOnDeath = true;

    public Slider healthBar;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = maxHealth;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }
    }

    //void Die()
    //{
    //    Debug.Log($"{gameObject.name} died.");

    //    // dth anim

    //    if (destroyOnDeath)
    //    {
    //        Destroy(gameObject);
    //    }
    //    else
    //    {
    //        gameObject.SetActive(false);
    //    }

    //    if (CompareTag("Player"))
    //    {
    //        FindObjectOfType<DeathUI>().ShowDeathScreen();
    //    }

    //    if (CompareTag("Enemy"))
    //    {
    //        var tracker = FindObjectOfType<QuestTracker>();
    //        if (tracker != null)
    //        {
    //            string enemyType = gameObject.name;

    //            if (enemyType.Contains("Skeleton"))
    //                tracker.EnemyKilled("Skeleton");
    //            else if (enemyType.Contains("OrcRider") || enemyType.Contains("Boss"))
    //                tracker.EnemyKilled("Boss");
    //            else if (enemyType.Contains("Orc"))
    //                tracker.EnemyKilled("Orc");
    //        }
    //    }
    //}

    void Die()
    {
        Debug.Log($"{gameObject.name} died.");

        if (!CompareTag("Player"))
        {
            QuestTracker tracker = FindObjectOfType<QuestTracker>();
            if (tracker != null)
            {
                if (gameObject.name.Contains("Skeleton"))
                    tracker.EnemyKilled("Skeleton");
                else if (gameObject.name.Contains("OrcRider"))
                    tracker.EnemyKilled("Boss");
                else if (gameObject.name.Contains("Orc"))
                    tracker.EnemyKilled("Orc");
            }
        }

        if (destroyOnDeath)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);

        if (CompareTag("Player"))
        {
            FindObjectOfType<DeathUI>().ShowDeathScreen();
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10);
        }
    }
}
