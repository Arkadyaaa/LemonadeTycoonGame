using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InventorySlideAnimation : MonoBehaviour
{
    public float startX = 0f;
    public float endX = 0f;
    public float duration = 0.5f;

    private RectTransform rectTransform;
    private float currentX;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(startX, rectTransform.anchoredPosition.y);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        rectTransform.DOAnchorPosX(endX, duration).SetEase(Ease.OutCubic);
    }

    public void Hide()
    {
        rectTransform.DOAnchorPosX(startX, duration).SetEase(Ease.InCubic)
            .OnComplete(() => gameObject.SetActive(false));
    }
}
