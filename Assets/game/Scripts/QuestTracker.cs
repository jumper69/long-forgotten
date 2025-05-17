using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestTracker : MonoBehaviour
{
    public TextMeshProUGUI questText;

    public int skeletonsToKill = 10;
    public int orcsToKill = 10;
    public int bossesToKill = 1;

    private int skeletonsKilled = 0;
    private int orcsKilled = 0;
    private int bossesKilled = 0;

    void Start()
    {
        UpdateUI();
    }

    public void EnemyKilled(string type)
    {
        switch (type)
        {
            case "Skeleton":
                skeletonsKilled++;
                break;
            case "Orc":
                orcsKilled++;
                break;
            case "Boss":
                bossesKilled++;
                break;
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        questText.text = $"Pokonaj wrogów:\n" +
                         $"Skeleton: {skeletonsKilled}/{skeletonsToKill}\n" +
                         $"Orc: {orcsKilled}/{orcsToKill}\n" +
                         $"Horde Boss: {bossesKilled}/{bossesToKill}";
    }
}
