using UnityEngine;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class NotificationAnimation : MonoBehaviour
{
    private RectTransform panel;

    public TextMeshProUGUI text;

    [Range(0f, 1f)] public float offScreenYPercent = 0.4f;
    [Range(0f, 1f)] public float onScreenYPercent = 0.25f;

    public float slideDuration = 1f;
    public float stayDuration = 1f;

    void Awake()
    {
        panel = GetComponent<RectTransform>();

        // Set anchor and pivot to top-center
        panel.anchorMin = new Vector2(0.5f, 1f);
        panel.anchorMax = new Vector2(0.5f, 1f);
        panel.pivot = new Vector2(0.5f, 1f);
    }

    void OnEnable()
    {
        AnimatePanel();
    }

    public void ShowNotification(string notifText)
    {
        text.text = notifText;
        gameObject.SetActive(true);
    }

    void AnimatePanel()
    {
        float canvasHeight = ((RectTransform)panel.parent).rect.height;

        float offY = canvasHeight * offScreenYPercent;
        float onY = -canvasHeight * onScreenYPercent;

        Vector2 targetPos = new Vector2(0, onY);    // Final on-screen position
        Vector2 fromPos = new Vector2(0, offY);     // Start from offscreen (above)

        panel.anchoredPosition = targetPos;

        panel.DOAnchorPos(targetPos, slideDuration)
            .From(fromPos)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                DOVirtual.DelayedCall(stayDuration, () =>
                {
                    // Slide back out
                    panel.DOAnchorPos(fromPos, slideDuration)
                        .SetEase(Ease.InCubic)
                        .OnComplete(() =>
                        {
                            gameObject.SetActive(false);
                        });
                });
            });
    }
}
