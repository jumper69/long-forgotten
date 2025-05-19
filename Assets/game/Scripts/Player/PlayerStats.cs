using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public PlayerAttack playerAttack;
    public HealthSystem playerHealthSystem;

    public int maxUpgrades = 5;
    public int healthUpgrades = 0;
    public int damageUpgrades = 0;

    public int upgradePoints = 0;

    public void AddUpgradePoints(int amount)
    {
        upgradePoints += amount;
    }

    public bool UpgradeHealth()
    {
        if (playerHealthSystem != null && upgradePoints > 0 && healthUpgrades < maxUpgrades)
        {
            playerHealthSystem.IncreaseMaxHealth(20);
            healthUpgrades++;
            upgradePoints--;
            Debug.Log("Health upgraded.");
            return true;
        }
        return false;
    }

    public bool UpgradeDamage()
    {
        if (playerAttack != null && upgradePoints > 0 && damageUpgrades < maxUpgrades)
        {
            playerAttack.damage += 5;
            damageUpgrades++;
            upgradePoints--;
            Debug.Log("Damage upgraded to: " + playerAttack.damage);
            return true;
        }
        return false;
    }
}
