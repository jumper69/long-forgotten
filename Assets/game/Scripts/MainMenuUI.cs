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
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        startButton.SetActive(true);
        settingsButton.SetActive(true);
        exitButton.SetActive(true);
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
}
