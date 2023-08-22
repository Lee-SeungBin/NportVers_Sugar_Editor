using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Image bg;
    private Slider slider;

    private RectTransform rectTransform;
    private Vector3 initialPosition;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialPosition = rectTransform.position;
        bg = GetComponent<Image>();
        slider = GetComponentInChildren<Slider>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

    public void ChangeAlpha()
    {
        float alphaValue = slider.value;
        Color newColor = bg.color;
        newColor.a = alphaValue;
        bg.color = newColor;
    }
}
