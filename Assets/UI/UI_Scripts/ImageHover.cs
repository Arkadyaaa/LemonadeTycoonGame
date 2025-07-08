using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening; 

public class ImageHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image imageToFade;
    [SerializeField] private float fadeDuration = 0.3f;

    private void Awake()
    {
        if (imageToFade != null)
        {
            var color = imageToFade.color;
            color.a = 0f;
            imageToFade.color = color;
            imageToFade.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (imageToFade != null)
        {
            imageToFade.gameObject.SetActive(true);
            imageToFade.DOFade(0.75f, fadeDuration).SetEase(Ease.InOutQuart);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (imageToFade != null)
        {
            imageToFade.DOFade(0f, fadeDuration).SetEase(Ease.InOutQuart)
                .OnComplete(() => imageToFade.gameObject.SetActive(false));
        }
    }
}
