using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonBorderHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject borderTint;

    public void OnPointerEnter(PointerEventData eventData)
    {
        borderTint.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        borderTint.SetActive(false);
    }
}
