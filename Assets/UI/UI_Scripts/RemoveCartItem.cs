using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RemoveCartItem : MonoBehaviour, IPointerClickHandler
{
    public SuppliesPanelController controller;

    public void OnPointerClick(PointerEventData eventData)
    {
        var text = GetComponent<TextMeshProUGUI>();
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, eventData.position, null);

        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = text.textInfo.linkInfo[linkIndex];
            if (int.TryParse(linkInfo.GetLinkID(), out int index))
            {
                controller.RemoveFromCart(index);
            }
        }
    }
}
