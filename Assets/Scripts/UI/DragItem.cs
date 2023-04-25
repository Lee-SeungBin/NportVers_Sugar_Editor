using UnityEngine;
using UnityEngine.UI;

public class DragItem : MonoBehaviour
{
    [SerializeField]
    private Image image;

    public Enums.SPECIAL_TYPE specialType { get; private set; }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void SetSpecial(Enums.SPECIAL_TYPE type, Sprite sprite)
    {
        specialType = type;

        if (specialType != Enums.SPECIAL_TYPE.NONE)
        {
            gameObject.SetActive(true);
            image.sprite = sprite;
            image.SetNativeSize();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }


}
