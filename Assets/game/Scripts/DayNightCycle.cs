using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    public Light2D globalLight;

    public float dawnDuration = 60f;
    public float dayDuration = 300f;
    public float duskDuration = 60f;
    public float nightDuration = 180f;

    private float timer;
    private Phase currentPhase = Phase.Dawn;

    private enum Phase { Dawn, Day, Dusk, Night }

    void Update()
    {
        if (globalLight == null) return;

        timer += Time.deltaTime;

        switch (currentPhase)
        {
            case Phase.Dawn:
                UpdateLight(0.2f, 1f, new Color(0.2f, 0.3f, 0.5f), new Color(1f, 0.85f, 0.7f), dawnDuration);
                if (timer >= dawnDuration)
                {
                    timer = 0f;
                    currentPhase = Phase.Day;
                }
                break;

            case Phase.Day:
                globalLight.intensity = 1f;
                globalLight.color = Color.white;
                if (timer >= dayDuration)
                {
                    timer = 0f;
                    currentPhase = Phase.Dusk;
                }
                break;

            case Phase.Dusk:
                UpdateLight(1f, 0.2f, Color.white, new Color(0.2f, 0.3f, 0.5f), duskDuration);
                if (timer >= duskDuration)
                {
                    timer = 0f;
                    currentPhase = Phase.Night;
                }
                break;

            case Phase.Night:
                globalLight.intensity = 0.2f;
                globalLight.color = new Color(0.2f, 0.3f, 0.5f);
                if (timer >= nightDuration)
                {
                    timer = 0f;
                    currentPhase = Phase.Dawn;
                }
                break;
        }
    }

    void UpdateLight(float fromIntensity, float toIntensity, Color fromColor, Color toColor, float duration)
    {
        float t = timer / duration;
        globalLight.intensity = Mathf.Lerp(fromIntensity, toIntensity, t);
        globalLight.color = Color.Lerp(fromColor, toColor, t);
    }
}
