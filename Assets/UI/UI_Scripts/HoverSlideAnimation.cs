using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections.Generic;

public class HoverSlideController : MonoBehaviour
{
    [System.Serializable]
    public class SlideItem
    {
        public RectTransform target;
        [HideInInspector] public Vector2 originalPos;
        [HideInInspector] public Tween currentTween;
        [HideInInspector] public bool isHovered;
    }

    [Header("Slide Settings")]
    public float slideDistance = 50f;
    public float slideDuration = 0.3f;

    [Header("Images To Slide")]
    public List<SlideItem> slideItems = new List<SlideItem>();

    private void Start()
    {
        foreach (var item in slideItems)
        {
            if (item.target != null)
                item.originalPos = item.target.anchoredPosition;
        }
    }

    private void Update()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> rayResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, rayResults);

        foreach (var item in slideItems)
        {
            if (item.target == null) continue;

            bool isOver = false;

            foreach (var hit in rayResults)
            {
                if (hit.gameObject == item.target.gameObject)
                {
                    isOver = true;
                    break;
                }
            }

            if (isOver && !item.isHovered)
            {
                SlideLeft(item);
                item.isHovered = true;
            }
            else if (!isOver && item.isHovered)
            {
                SlideBack(item);
                item.isHovered = false;
            }
        }
    }

    private void SlideLeft(SlideItem item)
    {
        Vector2 targetPos = item.originalPos + Vector2.left * slideDistance;
        item.currentTween?.Kill();
        item.currentTween = item.target.DOAnchorPos(targetPos, slideDuration).SetEase(Ease.OutCubic);
    }

    private void SlideBack(SlideItem item)
    {
        item.currentTween?.Kill();
        item.currentTween = item.target.DOAnchorPos(item.originalPos, slideDuration).SetEase(Ease.OutCubic);
    }
}
