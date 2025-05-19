using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenController : MonoBehaviour
{
    public GameObject loadingPanel;
    public Slider progressSlider;

    public void Show()
    {
        loadingPanel?.SetActive(true);
    }

    public void Hide()
    {
        loadingPanel?.SetActive(false);
    }

    public void SetProgress(float value)
    {
        if (progressSlider != null)
        {
            progressSlider.value = Mathf.Clamp01(value);
            Debug.Log($"Progress set to: {value}");
        }
    }
}
