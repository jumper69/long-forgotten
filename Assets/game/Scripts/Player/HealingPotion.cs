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

    public AudioClip drinkSound;
    private AudioSource audioSource;

    private bool unlocked = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void UnlockPotion()
    {
        unlocked = true;
    }

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

                    if (drinkSound != null && audioSource != null)
                    {
                        audioSource.PlayOneShot(drinkSound);
                    }
                }
            }
        }
    }
}
