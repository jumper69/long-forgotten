using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public GameObject dialogPanel;
    private bool isPlayerNear = false;

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            dialogPanel.SetActive(!dialogPanel.activeSelf);
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
