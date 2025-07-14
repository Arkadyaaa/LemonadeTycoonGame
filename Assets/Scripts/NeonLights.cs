using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NeonLights : MonoBehaviour
{
    public Material targetMaterial;
    public float scrollSpeed = 1f;
    public float intensity = 5f;

    private float offset;
    public bool toggleCycle = true;
    public Toggle toggleUI;

    public TMP_InputField intensityField;
    public TMP_InputField speedField;

    void OnValidate()
    {
        targetMaterial.SetFloat("_intensity", intensity);
    }

    void Update()
    {
        if (!toggleCycle) return;

        offset += Time.unscaledDeltaTime * scrollSpeed;
        offset %= 1f;

        targetMaterial.SetFloat("_offset", offset);
    }

    public void ToggleCycle()
    {
        toggleCycle = toggleUI.isOn;
    }

    public void ChangeSpeed()
    {
        if (string.IsNullOrEmpty(speedField.text)) return;

        string userInput = speedField.text;
        int speedValue = int.Parse(userInput);
        speedValue = Mathf.Clamp(speedValue, 0, 99);
        float normalizedValue = (float)speedValue / 100;

        scrollSpeed = normalizedValue;
    }

    public void ChangeIntensity()
    {
        if (string.IsNullOrEmpty(intensityField.text)) return;

        string userInput = intensityField.text;
        int intensityValue = int.Parse(userInput);
        intensityValue = Mathf.Clamp(intensityValue, 0, 99);

        intensity = intensityValue;
        targetMaterial.SetFloat("_intensity", intensity);
    }

    public void LoadData(bool toggleCycle, float scrollSpeed, float intensity)
    {
        toggleUI.isOn = toggleCycle;
        this.toggleCycle = toggleCycle;

        this.scrollSpeed = Mathf.Clamp(scrollSpeed, 0f, 1f);
        speedField.text = $"{this.scrollSpeed * 100f:F0}";

        this.intensity = Mathf.Clamp(intensity, 0f, 100f);
        intensityField.text = $"{this.intensity:F0}";
        targetMaterial.SetFloat("_intensity", this.intensity);
    }
}
