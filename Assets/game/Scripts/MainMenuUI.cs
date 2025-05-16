using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenuUI : MonoBehaviour
{
    public GameObject settingsPanel;
    public AudioMixer audioMixer;
    public GameObject startButton;
    public GameObject settingsButton;
    public GameObject exitButton;
    public GameObject controlsPanel;
    public GameObject controlsButton;

    public void StartGame()
    {
        SceneManager.LoadScene("HomeMap");
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        startButton.SetActive(false);
        settingsButton.SetActive(false);
        exitButton.SetActive(false);
        controlsButton.SetActive(false);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        startButton.SetActive(true);
        settingsButton.SetActive(true);
        exitButton.SetActive(true);
        controlsButton.SetActive(true);
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit pressed");
    }

    public void OpenControls()
    {
        controlsPanel.SetActive(true);
        settingsPanel.SetActive(false);
        startButton.SetActive(false);
        settingsButton.SetActive(false);
        exitButton.SetActive(false);
        controlsButton.SetActive(false);
    }

    public void CloseControls()
    {
        controlsPanel.SetActive(false);
        startButton.SetActive(true);
        settingsButton.SetActive(true);
        exitButton.SetActive(true);
        controlsButton.SetActive(true);
    }
}
