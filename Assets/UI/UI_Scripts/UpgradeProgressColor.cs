using UnityEngine;
using UnityEngine.UI;

public class UpgradeProgressColor : MonoBehaviour
{
    public bool isBought;
    public bool isDisabled;

    [Header("Unbought Color")]
    public Color unboughtColor = new Color(1f, 1f, 1f, 1f);
    public Color unboughtSelectedColor = new Color(1f, 1f, 1f, 1f);

    [Header("Bought Color")]
    public Color boughtColor = new Color(1f, 1f, 1f, 1f);
    public Color boughtSelectedColor = new Color(1f, 1f, 1f, 1f);

    [Header("Disabled Color")]
    public Color disabledColor = new Color(1f, 1f, 1f, 1f);
    public Color disabledSelectedColor = new Color(1f, 1f, 1f, 1f);

    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
        ApplyColor();
    }

    void OnValidate()
    {
        image = GetComponent<Image>();
        ApplyColor();
    }

    public void ApplyColor()
    {
        if (image == null) return;
        image.color = GetColor(false);
    }

    public Color GetColor(bool isSelected)
    {
        if (isDisabled)
            return isSelected ? disabledSelectedColor : disabledColor;
        
        if (isBought)
            return isSelected ? boughtSelectedColor : boughtColor;

        return isSelected ? unboughtSelectedColor : unboughtColor;
    }

    public void BuyUpgradeColor()
    {
        isBought = true;
        ApplyColor();
    }
}
