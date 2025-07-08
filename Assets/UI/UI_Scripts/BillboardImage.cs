using UnityEngine;

public class BillboardImage : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main != null)
        {
            transform.forward = Camera.main.transform.forward;
        }
        else
        {
            Debug.Log("No Camera");
        }
    }
}
