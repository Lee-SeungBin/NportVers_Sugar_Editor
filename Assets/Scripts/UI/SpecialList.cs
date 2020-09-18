using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialList : MonoBehaviour
{
    public Sprite[] specialSprites;

    public enum SPECIAL_TYPE
    {
        NONE = -1,
        JELLY = 0,
        FROG_SOUP = 1,
        BOX = 2,
        WOODEN_FENCE = 3
    }

    public bool removeModeOn = false;

    public void SelectEmtpy()
    {
        removeModeOn = false;
        UIManager.Instance.dragItem.SetSpecial(SPECIAL_TYPE.NONE, null);
    }

    public void SelectJelly()
    {
        removeModeOn = false;
        UIManager.Instance.dragItem.SetSpecial(SPECIAL_TYPE.JELLY, specialSprites[(int)SPECIAL_TYPE.JELLY]);
    }

    public void SelectFrogSoup()
    {
        removeModeOn = false;
        UIManager.Instance.dragItem.SetSpecial(SPECIAL_TYPE.FROG_SOUP, specialSprites[(int)SPECIAL_TYPE.FROG_SOUP]);
    }

    public void SelectBox()
    {
        removeModeOn = false;
        UIManager.Instance.dragItem.SetSpecial(SPECIAL_TYPE.BOX, specialSprites[(int)SPECIAL_TYPE.BOX]);
    }

    public void SelectWoodenFence()
    {
        removeModeOn = false;
        UIManager.Instance.dragItem.SetSpecial(SPECIAL_TYPE.WOODEN_FENCE, specialSprites[(int)SPECIAL_TYPE.WOODEN_FENCE]);
    }
}
