using UnityEngine;
using UnityEngine.UI;

public class PlayerXP : MonoBehaviour
{
    public int currentXP = 0;
    public int level = 1;
    public int xpToNextLevel = 100;

    public Slider xpBar;

    public GameObject healingPotionUI;
    public GameObject newAttackUI;

    public AudioClip levelUpSound;
    private AudioSource audioSource;

    void Start()
    {
        UpdateXPBar();
        audioSource = GetComponent<AudioSource>();
    }

    public void GainXP(int amount)
    {
        currentXP += amount;
        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }

        UpdateXPBar();
    }

    void LevelUp()
    {
        currentXP -= xpToNextLevel;
        level++;
        xpToNextLevel += 50;

        Debug.Log($"LEVEL UP! {level}");

        if (level == 2)
        {
            if (healingPotionUI != null)
                healingPotionUI.SetActive(true);

            HealingPotion potion = FindObjectOfType<HealingPotion>();
            if (potion != null)
                potion.UnlockPotion();

            if (levelUpSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(levelUpSound);
            }
        }

        if (level == 3)
        {
            if (newAttackUI != null)
                newAttackUI.SetActive(true);

            FindObjectOfType<PlayerAttack>().strongAttackUnlocked = true;

            if (levelUpSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(levelUpSound);
            }
        }
    }
    void UpdateXPBar()
    {
        if (xpBar != null)
        {
            xpBar.maxValue = xpToNextLevel;
            xpBar.value = currentXP;
        }
    }
}
