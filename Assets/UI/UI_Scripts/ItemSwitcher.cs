using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ItemSwitcherSlide : MonoBehaviour
{
    public Button leftButton;
    public Button rightButton;
    public RectTransform[] items;
    public float slideDistance = 800f;
    public float slideDuration = 0.4f;
    public float fadeDuration = 0.4f;
    public Ease ease = Ease.InOutQuart;

    public Image[] progress;

    private int currentIndex = 0;
    private bool isSwitching = false;

    void Start()
    {
        for (int i = 0; i < items.Length; i++)
        {
            CanvasGroup cg = items[i].GetComponent<CanvasGroup>();
            if (!cg) cg = items[i].gameObject.AddComponent<CanvasGroup>();

            items[i].anchoredPosition = Vector2.zero;
            items[i].gameObject.SetActive(i == currentIndex);
            cg.alpha = i == currentIndex ? 1 : 0;
        }

        UpgradeProgress();

        leftButton.onClick.AddListener(() => SwitchItem(-1));
        rightButton.onClick.AddListener(() => SwitchItem(1));
    }

    void SwitchItem(int direction)
    {
        if (isSwitching) return;

        int nextIndex = currentIndex + direction;
        if (nextIndex < 0 || nextIndex >= items.Length) return;

        isSwitching = true;

        RectTransform currentItem = items[currentIndex];
        RectTransform nextItem = items[nextIndex];

        float exitX = -direction * slideDistance;
        float enterX = direction * slideDistance;

        CanvasGroup currentCG = currentItem.GetComponent<CanvasGroup>();
        CanvasGroup nextCG = nextItem.GetComponent<CanvasGroup>();

        // Prepare next item
        nextItem.anchoredPosition = new Vector2(enterX, 0);
        nextItem.gameObject.SetActive(true);
        nextCG.alpha = 0;

        currentIndex = nextIndex;
        UpgradeProgress();

        Sequence seq = DOTween.Sequence();

        // Animate current out
        seq.Join(currentItem.DOAnchorPosX(exitX, slideDuration).SetEase(ease));
        seq.Join(currentCG.DOFade(0f, fadeDuration).SetEase(ease));

        // Animate next in
        seq.Join(nextItem.DOAnchorPosX(0, slideDuration).SetEase(ease));
        seq.Join(nextCG.DOFade(1f, fadeDuration).SetEase(ease));

        seq.OnComplete(() =>
        {
            currentItem.gameObject.SetActive(false);
            currentItem.anchoredPosition = Vector2.zero;
            isSwitching = false;
        });
    }

    void UpgradeProgress()
    {
        if (progress == null || progress.Length == 0) return;

        bool lockNext = false;

        for (int i = 0; i < progress.Length; i++)
        {
            UpgradeProgressColor progressColor = progress[i].GetComponent<UpgradeProgressColor>();

            bool isThisDisabled = lockNext;
            if (!progressColor.isBought)
                lockNext = true;

            progressColor.isDisabled = isThisDisabled;

            Color targetColor = progressColor.GetColor(i == currentIndex);
            progress[i].DOColor(targetColor, 0.2f);
        }
    }
}
