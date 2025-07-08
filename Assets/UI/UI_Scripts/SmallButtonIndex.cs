using UnityEngine;
using UnityEngine.EventSystems;

public class SmallButtonIndex : MonoBehaviour, IPointerClickHandler
{
    public int panelIndex;
    public SmallButtonsController panelController;

    public void OnPointerClick(PointerEventData eventData)
    {
        panelController.OpenPanel(panelIndex);
    }
}
