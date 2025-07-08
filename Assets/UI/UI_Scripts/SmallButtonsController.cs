using UnityEngine;

public class SmallButtonsController : MonoBehaviour
{
    public GameObject[] uiPanels;

    public void OpenPanel(int index)
    {
        if (index >= 0 && index < uiPanels.Length)
        {
            foreach (GameObject panel in uiPanels)
            {
                panel.SetActive(false);
            }

            uiPanels[index].SetActive(true);
        }
    }
}
