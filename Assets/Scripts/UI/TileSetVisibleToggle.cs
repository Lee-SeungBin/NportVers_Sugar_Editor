using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TileSetVisibleToggle : MonoBehaviour, IPointerClickHandler
{
    public GameObject ok;
    public GameObject no;

    public UnityEvent onClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }

    public bool isOn
    {
        get
        {
            return ok.activeSelf;
        }

        set
        {
            SetVisible(value);
        }
    }

    public void SetVisible(bool isActive)
    {
        ok.SetActive(isActive);
        no.SetActive(!isActive);
    }

}
