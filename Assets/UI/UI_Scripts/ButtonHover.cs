using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image image;
    private Color originalColor;

    [SerializeField] private Color hoverColor = new Color(0.933f, 0.859f, 0.588f, 1f);

    void Awake()
    {
        image = GetComponent<Image>();
        originalColor = image.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = hoverColor;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = originalColor;
    }
}
