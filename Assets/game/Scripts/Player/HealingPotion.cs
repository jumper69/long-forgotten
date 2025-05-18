using UnityEngine;
using TMPro;

public class HealingPotion : MonoBehaviour
{
    public TextMeshProUGUI cooldownText;
    public int healingAmount = 50;
    public KeyCode useKey = KeyCode.H;
    public HealthSystem healthSystem;
    public float cooldown = 5f;
    private float lastUseTime = -Mathf.Infinity;

    private bool unlocked = false;

    public void UnlockPotion()
    {
        unlocked = true;
    }

    //void Update()
    //{
    //    if (unlocked && Input.GetKeyDown(useKey))
    //    {
    //        if (Time.time - lastUseTime >= cooldown)
    //        {
    //            if (healthSystem != null)
    //            {
    //                healthSystem.Heal(healingAmount);
    //                lastUseTime = Time.time;
    //            }
    //        }
    //        else
    //        {
    //            Debug.Log("Cooldown!");
    //        }
    //    }
    //}
    void Update()
    {
        float timeSinceUse = Time.time - lastUseTime;
        float timeLeft = cooldown - timeSinceUse;

        if (cooldownText != null)
        {
            if (timeLeft > 0)
                cooldownText.text = $"{timeLeft:F1}s";
            else
                cooldownText.text = "Ready";
        }

        if (unlocked && Input.GetKeyDown(useKey))
        {
            if (timeLeft <= 0f)
            {
                if (healthSystem != null)
                {
                    healthSystem.Heal(healingAmount);
                    lastUseTime = Time.time;
                }
            }
        }
    }
}
