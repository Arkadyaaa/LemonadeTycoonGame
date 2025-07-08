using UnityEngine;
using DG.Tweening;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Groups to Fade Out")]
    public CanvasGroup fadeGroup;         // fades quickly
    public RectTransform logo;            // slides up out
    public CanvasGroup backgroundFade;    // fades slowly

    [Header("UI Groups to Slide In")]
    public RectTransform groupSlideUp;
    public RectTransform groupSlideLeft;
    public RectTransform groupSlideDown;

    [Header("Main Menu BG")]
    public GameObject bg;

    [Header("Timings")]
    public float fadeOutDuration = 0.5f;
    public float logoSlideDuration = 0.6f;
    public float backgroundFadeDuration = 1.5f;
    public float delayBetweenStages = 0.2f;
    public float slideInDuration = 0.6f;

    void Start()
    {
        groupSlideUp.GetComponent<CanvasGroup>().alpha = 0f;
        groupSlideLeft.GetComponent<CanvasGroup>().alpha = 0f;
        groupSlideDown.GetComponent<CanvasGroup>().alpha = 0f;
    }

    public void StartGame()
    {
        Sequence sequence = DOTween.Sequence();

        // Fade out UI group
        sequence.Append(fadeGroup.DOFade(0, fadeOutDuration).SetEase(Ease.InOutQuad));

        // Move logo up off-screen
        sequence.Join(logo.DOAnchorPosY(logo.anchoredPosition.y + 500, logoSlideDuration).SetEase(Ease.InBack));

        // Fade background slowly
        sequence.AppendInterval(delayBetweenStages);
        sequence.Append(backgroundFade.DOFade(0, backgroundFadeDuration).SetEase(Ease.InOutQuad));

        // Slide in new UI groups
        sequence.AppendInterval(delayBetweenStages);
        groupSlideUp.GetComponent<CanvasGroup>().alpha = 1f;
        groupSlideLeft.GetComponent<CanvasGroup>().alpha = 1f;
        groupSlideDown.GetComponent<CanvasGroup>().alpha = 1f;

        // Slide in from bottom
        Vector2 upStartPos = groupSlideUp.anchoredPosition - new Vector2(0, 400);
        groupSlideUp.anchoredPosition = upStartPos;
        sequence.Append(groupSlideUp.DOAnchorPosY(upStartPos.y + 400, slideInDuration).SetEase(Ease.OutCirc));

        // Slide in from right
        Vector2 rightStartPos = groupSlideLeft.anchoredPosition + new Vector2(400, 0);
        groupSlideLeft.anchoredPosition = rightStartPos;
        sequence.Join(groupSlideLeft.DOAnchorPosX(rightStartPos.x - 400, slideInDuration).SetEase(Ease.OutCirc));

        // Slide in from top
        Vector2 downStartPos = groupSlideDown.anchoredPosition + new Vector2(0, 400);
        groupSlideDown.anchoredPosition = downStartPos;
        sequence.Join(groupSlideDown.DOAnchorPosY(downStartPos.y - 400, slideInDuration).SetEase(Ease.OutCirc));

        // Disable the whole Main Menu Object
        sequence.OnComplete(() => {
            bg.SetActive(false);
        });
    }
}
