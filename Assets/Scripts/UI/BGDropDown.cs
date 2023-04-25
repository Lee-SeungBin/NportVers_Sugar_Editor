using UnityEngine;
using UnityEngine.UI;

public class BGDropDown : MonoBehaviour
{
    public SpriteChanger bg;
    public Dropdown dropDown;

    public void OnChangeBG()
    {
        bg.SetSprite(dropDown.value);
        MapManager.Instance.ChangeTheme(dropDown.value);
    }

    public int value
    {
        get
        {
            return dropDown.value;
        }

        set
        {
            dropDown.value = value;

            OnChangeBG();
        }
    }
}
