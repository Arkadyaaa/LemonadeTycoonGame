using UnityEngine;
using UnityEngine.UI;

public class NeonLightUI : MonoBehaviour
{
    private float hue;
    public float speed = 1f;
    private Image image;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        hue = (hue + Time.deltaTime * speed) % 1;
        Color rgb = Color.HSVToRGB(hue, 1f, 1f);
        image.color = rgb;
    }
}
