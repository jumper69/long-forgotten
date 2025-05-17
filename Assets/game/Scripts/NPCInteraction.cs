using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class NPCInteraction : MonoBehaviour
{
    public GameObject dialogPanel;
    private bool isPlayerNear = false;
    public string[] dialogLines;
    private int dialogIndex = 0;
    public TextMeshProUGUI dialogText;

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            dialogPanel.SetActive(true);
            dialogText.text = dialogLines[dialogIndex];

            dialogIndex++;
            if (dialogIndex >= dialogLines.Length)
            {
                dialogIndex = 0;
                dialogPanel.SetActive(false);
            }
        }
    }
    //void Update()
    //{
    //    if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
    //    {
    //        dialogPanel.SetActive(!dialogPanel.activeSelf);
    //    }
    //}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            dialogPanel.SetActive(false);
        }
    }
}
