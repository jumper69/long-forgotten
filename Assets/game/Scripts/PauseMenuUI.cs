using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PauseMenuUI : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject controlsPanel;
    public AudioMixer audioMixer;

    private bool isPaused = false;
    private GameObject currentPanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                OpenPanel(pausePanel);
                Time.timeScale = 0f;
                isPaused = true;
            }
            else
            {
                if (currentPanel == controlsPanel)
                {
                    OpenPanel(pausePanel);
                }
                else
                {
                    ClosePauseMenu();
                }
            }
        }
    }

    public void OpenPanel(GameObject panel)
    {
        if (currentPanel != null)
            currentPanel.SetActive(false);

        panel.SetActive(true);
        currentPanel = panel;
    }

    public void ClosePauseMenu()
    {
        if (currentPanel != null)
            currentPanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
        currentPanel = null;
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20f);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenControls()
    {
        OpenPanel(controlsPanel);
    }

    public void CloseControls()
    {
        OpenPanel(pausePanel);
    }
}
