using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterList : MonoBehaviour
{
    [SerializeField]
    private Dropdown typeDropdown;

    [SerializeField]
    private CharacterListBar[] types;
    
    private void Start()
    {
        OnChangeSelectMonsterType();
    }


    public void OnChangeSelectMonsterType()
    {
        for(int i = 0; i < types.Length; ++i)
        {
            types[i].gameObject.SetActive(false);
        }

        types[typeDropdown.value].gameObject.SetActive(true);
    }

    private void SetAllNonSelectState()
    {
        for (int i = 0; i < types.Length; ++i)
        {
            types[i].SetSelectCharacter(-1);
        }
    }

    public void OnClickRemoveCharacterInList()
    {
        SetAllNonSelectState();

        MapManager.Instance.ChangeCharactor(Charactor.TASTY_TYPE.NONE, 0);
    }

    public void OnClickSTCharacterInList()
    {
        int charactorIndex = int.Parse(EventSystem.current.currentSelectedGameObject.name.Substring(2, 2)) - 1;
        SetAllNonSelectState();
        types[1].SetSelectCharacter(charactorIndex);
        MapManager.Instance.ChangeCharactor(Charactor.TASTY_TYPE.ST, charactorIndex);
    }

    public void OnClickCHCharacterInList()
    {
        int charactorIndex = int.Parse(EventSystem.current.currentSelectedGameObject.name.Substring(2, 2)) - 1;
        SetAllNonSelectState();
        types[2].SetSelectCharacter(charactorIndex);
        MapManager.Instance.ChangeCharactor(Charactor.TASTY_TYPE.CH, charactorIndex);
    }

    public void OnClickCRCharacterInList()
    {
        int charactorIndex = int.Parse(EventSystem.current.currentSelectedGameObject.name.Substring(2, 2)) - 1;
        SetAllNonSelectState();
        types[0].SetSelectCharacter(charactorIndex);
        MapManager.Instance.ChangeCharactor(Charactor.TASTY_TYPE.CR, charactorIndex);
    }

    public void OnClickEGCharacterInList()
    {
        int charactorIndex = int.Parse(EventSystem.current.currentSelectedGameObject.name.Substring(2, 2)) - 1;
        SetAllNonSelectState();
        types[3].SetSelectCharacter(charactorIndex);
        MapManager.Instance.ChangeCharactor(Charactor.TASTY_TYPE.EG, charactorIndex);
    }
    public void OnClickBRCharacterInList()
    {
        int charactorIndex = int.Parse(EventSystem.current.currentSelectedGameObject.name.Substring(2, 2)) - 1;
        SetAllNonSelectState();
        types[4].SetSelectCharacter(charactorIndex);
        MapManager.Instance.ChangeCharactor(Charactor.TASTY_TYPE.BR, charactorIndex);
    }
    public void OnClickSPCharacterInList()
    {
        int charactorIndex = int.Parse(EventSystem.current.currentSelectedGameObject.name.Substring(2, 2)) - 1;
        SetAllNonSelectState();
        types[5].SetSelectCharacter(charactorIndex);
        MapManager.Instance.ChangeCharactor(Charactor.TASTY_TYPE.SP, charactorIndex);
    }


}
