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

        MapManager.Instance.ChangeCharactor(Enums.CHARACTOR_TASTY_TYPE.NONE, 0);
    }

    public void OnClickCharacterInList(int typeIndex)
    {
        int charactorIndex = int.Parse(EventSystem.current.currentSelectedGameObject.name.Substring(2, 2)) - 1;
        SetAllNonSelectState();
        types[typeIndex].SetSelectCharacter(charactorIndex);
        MapManager.Instance.ChangeCharactor((Enums.CHARACTOR_TASTY_TYPE)typeIndex, charactorIndex);
    }

}
