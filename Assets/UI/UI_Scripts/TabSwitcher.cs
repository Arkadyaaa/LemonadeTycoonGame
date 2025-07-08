using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TabSwitcher : MonoBehaviour
{
    public RectTransform[] tabs;
    public float slideDistance = 800f;
    public float slideDuration = 0.4f;
    public float fadeDuration = 0.4f;
    public Ease ease = Ease.InOutQuart;

    public TextMeshProUGUI[] tabButtons;
    public Color normalTextColor = new Color(1f, 1f, 1f, 1f);
    public Color selectedTextColor = new Color(1f, 1f, 1f, 1f);

    private int currentIndex = 0;
    private bool isSwitching = false;

    void Start()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            CanvasGroup cg = tabs[i].GetComponent<CanvasGroup>();
            if (!cg) cg = tabs[i].gameObject.AddComponent<CanvasGroup>();

            tabs[i].anchoredPosition = Vector2.zero;
            tabs[i].gameObject.SetActive(i == currentIndex);
            cg.alpha = i == currentIndex ? 1 : 0;
        }
    }

    public void SwitchTab(int index)
    {
        if (isSwitching || index == currentIndex || index < 0 || index >= tabs.Length) return;

        isSwitching = true;

        int direction = index > currentIndex ? 1 : -1;

        RectTransform currentTab = tabs[currentIndex];
        RectTransform nextTab = tabs[index];

        float exitX = -direction * slideDistance;
        float enterX = direction * slideDistance;

        CanvasGroup currentCG = currentTab.GetComponent<CanvasGroup>();
        CanvasGroup nextCG = nextTab.GetComponent<CanvasGroup>();

        nextTab.anchoredPosition = new Vector2(enterX, 0);
        nextTab.gameObject.SetActive(true);
        nextCG.alpha = 0;

        currentIndex = index;

        changeTabTextColor();

        Sequence seq = DOTween.Sequence();

        // Animate current tab out
        seq.Join(currentTab.DOAnchorPosX(exitX, slideDuration).SetEase(ease));
        seq.Join(currentCG.DOFade(0f, fadeDuration).SetEase(ease));

        // Animate next tab in
        seq.Join(nextTab.DOAnchorPosX(0, slideDuration).SetEase(ease));
        seq.Join(nextCG.DOFade(1f, fadeDuration).SetEase(ease));

        seq.OnComplete(() =>
        {
            currentTab.gameObject.SetActive(false);
            currentTab.anchoredPosition = Vector2.zero;
            isSwitching = false;
        });
    }

    void changeTabTextColor()
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            var text = tabButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                text.color = (i == currentIndex) ? selectedTextColor : normalTextColor;
            }
        }
    }
}
