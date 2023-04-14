using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialList : MonoBehaviour
{
    public Sprite[] specialSprites;

    [SerializeField]
    private GameObject[] BoxTypes;
    public enum SPECIAL_TYPE
    {
        NONE = -1,
        JELLY = 0,
        FROG_SOUP = 1,
        BOX = 2,
        WOODEN_FENCE = 3,
        VINE = 4,
    }

    public bool removeModeOn = false;
    public int boxlayer;
    public int vinelayer;
    public int boxtype;

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

    public void SelectBoxList(Toggle toggle)
    {
        for (int i = 0; i < BoxTypes.Length; i++)
        {
            if (!BoxTypes[i].activeSelf)
            {
                BoxTypes[i].SetActive(toggle.isOn);
                toggle.interactable = !toggle.isOn;
            }
            else
            {
                BoxTypes[i].SetActive(false);
                toggle.interactable = toggle.isOn;
                toggle.isOn = false;
            }
        }
    }
    public void SelectWoodenFence()
    {
        removeModeOn = false;
        UIManager.Instance.dragItem.SetSpecial(SPECIAL_TYPE.WOODEN_FENCE, specialSprites[(int)SPECIAL_TYPE.WOODEN_FENCE]);
    }
    public void SelectBox(int boxLayer)
    {
        removeModeOn = false;
            if (boxLayer == 1)
                UIManager.Instance.dragItem.SetSpecial(SPECIAL_TYPE.BOX, specialSprites[(int)SPECIAL_TYPE.BOX]);
            else if (boxLayer == 3)
                UIManager.Instance.dragItem.SetSpecial(SPECIAL_TYPE.BOX, specialSprites[4]);
            else if (boxLayer == 5)
                UIManager.Instance.dragItem.SetSpecial(SPECIAL_TYPE.BOX, specialSprites[5]); // 추후 수정 예정 specialSprites[]
        boxlayer = boxLayer;
    }
    public void SelectBoxtype(int boxType)
    {
        removeModeOn = false;
        if (boxType == 1)
            UIManager.Instance.dragItem.SetSpecial(SPECIAL_TYPE.BOX, specialSprites[8]);
        else if (boxType == 2)
            UIManager.Instance.dragItem.SetSpecial(SPECIAL_TYPE.BOX, specialSprites[7]);
        boxtype = boxType;
    }

    public void SelectVine(int vineLayer)
    {
        removeModeOn = false;
        UIManager.Instance.dragItem.SetSpecial(SPECIAL_TYPE.VINE, specialSprites[6]);
        vinelayer = vineLayer;
    }
}
