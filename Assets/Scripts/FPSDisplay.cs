using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    [SerializeField] private Text display_Text;
    [SerializeField] private float fpsRefreshRate = 1f;

    private float timer;
    
    public void Update()
    {
        float current = 0;
        if (Time.unscaledTime > timer)
        {
            current = (int)(1f / Time.unscaledDeltaTime);
            display_Text.text = current.ToString() + " FPS";

            timer = Time.unscaledTime + fpsRefreshRate;
        }
    }
}