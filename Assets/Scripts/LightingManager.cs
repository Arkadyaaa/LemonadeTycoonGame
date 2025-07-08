using UnityEngine;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    public Light DirectionalLight;
    public LightingPreset Preset;
    public bool reverseSunDirection = true;
    [Range(0, 24)] public float TimeOfDay = 9;
    [Range(0, 360)] public float sunYRotation = 170f;

    public bool isTimeRunning = false;

    void Update()
    {
        if (Preset == null) return;

        float timePercent = TimeOfDay / 24f;

        if (Application.isPlaying)
        {
            if (isTimeRunning)
            {
                TimeOfDay += (10f / 300f) * Time.deltaTime;
                timePercent = TimeOfDay / 24f;
            }
        }

        UpdateLighting(timePercent);

    }

    void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);

        DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);
        DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((GetTimePercent(timePercent) * 360f) - 90f, sunYRotation, 0f));
    }

    float GetTimePercent(float t) => reverseSunDirection ? -t : t;
}
