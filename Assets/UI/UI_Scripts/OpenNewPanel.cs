using UnityEngine;

public class OpenNewPanel : MonoBehaviour
{
    public GameObject[] panelLists;
    public GameObject[] selectLists;

    public void CloseOthersExcept(GameObject selectedPanel)
    {
        foreach (GameObject panel in panelLists)
        {
            if (panel != selectedPanel)
            {
                panel.SetActive(false);
            }
            else
            {
                panel.SetActive(true);
            }
        }
    }

    public void SelectedImage(GameObject image)
    {
        foreach (GameObject panel in selectLists)
        {
            if (panel != image)
            {
                panel.SetActive(false);
            }
            else
            {
                panel.SetActive(true);
            }
        }
    }
}
