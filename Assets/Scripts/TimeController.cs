using UnityEngine;

public class TimeController : MonoBehaviour
{
    private float normalSpeed = 1f;
    public float fastforwardSpeed = 20f;
    public float skipSpeed = 1000f;
    private float fastSpeed = 20f;

    private bool fastForwardToggle = false;

    void OnEnable()
    {
        fastSpeed = fastforwardSpeed;
    }

    void Update()
    {
        Time.timeScale = fastForwardToggle ? fastSpeed : normalSpeed;
    }

    public void ToggleFastForward()
    {
        fastSpeed = fastforwardSpeed;
        if (fastForwardToggle)
        {
            fastForwardToggle = false;
        }
        else
        {
            fastForwardToggle = true;
        }
    }

    public void SetFastForward(bool state)
    {
        fastSpeed = fastforwardSpeed;
        fastForwardToggle = state;
    }

    public void SkipDay()
    {
        fastSpeed = skipSpeed;
        fastForwardToggle = true;
    }
}
    