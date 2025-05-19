using UnityEngine;
using TMPro;

public class UpgradePanelUI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI healthLevelText;
    public TextMeshProUGUI damageLevelText;

    private PlayerStats playerStats;

    void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();

        if (panel != null)
            panel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePanel();
        }

        if (panel.activeSelf)
        {
            UpdateUI();
        }
    }

    void TogglePanel()
    {
        bool isActive = panel.activeSelf;
        panel.SetActive(!isActive);
        Time.timeScale = isActive ? 1f : 0f;
    }

    public void UpgradeHealth()
    {
        if (playerStats != null && playerStats.UpgradeHealth())
        {
            UpdateUI();
        }
    }

    public void UpgradeDamage()
    {
        if (playerStats != null && playerStats.UpgradeDamage())
        {
            UpdateUI();
        }
    }

    public void ClosePanel()
    {
        if (panel != null)
            panel.SetActive(false);

        Time.timeScale = 1f;
    }

    void UpdateUI()
    {
        if (playerStats != null)
        {
            pointsText.text = $"Punkty do rozdania: {playerStats.upgradePoints}";
            healthLevelText.text = $"{playerStats.healthUpgrades}";
            damageLevelText.text = $"{playerStats.damageUpgrades}";
        }
    }
}
