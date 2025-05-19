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

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"{gameObject.name} healed for {amount}. HP: {currentHealth}");

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }
    }
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
            FindObjectOfType<DeathSound>()?.PlayDeathSound();
            FindObjectOfType<DeathUI>().ShowDeathScreen();
        }
        if (CompareTag("Enemy"))
        {
            PlayerXP xp = FindObjectOfType<PlayerXP>();
            if (xp != null)
                xp.GainXP(25);
        }

    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            TakeDamage(15);
        }
    }
}
