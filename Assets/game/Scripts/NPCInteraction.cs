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
    public GameObject questTextObject;

    public string[] initialDialogLines;
    public string[] completedDialogLines;
    private QuestTracker questTracker;

    void Start()
    {
        questTracker = FindObjectOfType<QuestTracker>();
    }
    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            dialogPanel.SetActive(true);

            string[] currentDialog = questTracker.IsQuestComplete() ? completedDialogLines : initialDialogLines;

            dialogText.text = currentDialog[dialogIndex];
            dialogIndex++;

            if (dialogIndex >= currentDialog.Length)
            {
                dialogPanel.SetActive(false);
                dialogIndex = 0;

                if (!questTracker.IsQuestComplete())
                {
                    questTextObject.SetActive(true);
                }
            }
        }
    }

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
