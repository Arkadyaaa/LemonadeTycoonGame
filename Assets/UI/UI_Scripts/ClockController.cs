using UnityEngine;
using TMPro;
using System.Collections;

public class ClockController : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    // real life seconds per 1 minutes in-game
    public float timeRate = 0.5f;

    [Header("Starting Time")]
    public int startHour = 8;
    public int startMinute = 0;

    private int hours;
    private int minutes;
    private float timeAccumulator = 0f;

    void OnEnable()
    {
        hours = startHour;
        minutes = startMinute;
        UpdateClockText();
    }

    void Update()
    {
        timeAccumulator += Time.deltaTime;

        if (timeAccumulator >= timeRate)
        {
            timeAccumulator -= timeRate;

            minutes += 1;
            if (minutes >= 60)
            {
                minutes = 0;
                hours = (hours + 1) % 24;
            }

            UpdateClockText();
        }
    }

    void UpdateClockText()
    {
        timerText.text = $"{hours:D2}:{minutes:D2}";
    }
}
