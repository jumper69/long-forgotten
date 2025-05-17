using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathUI : MonoBehaviour
{
    public GameObject deathOverlay;
    public Button returnToMenuButton;

    private void Start()
    {
        if (deathOverlay != null)
            deathOverlay.SetActive(false);

        if (returnToMenuButton != null)
            returnToMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    public void ShowDeathScreen()
    {
        if (deathOverlay != null)
            deathOverlay.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

