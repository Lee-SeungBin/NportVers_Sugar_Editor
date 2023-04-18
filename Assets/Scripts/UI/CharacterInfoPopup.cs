using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoPopup : MonoBehaviour
{

    public Dropdown iceStepDropdown;
    public Toggle starToggle;
    public Toggle directionToggle;
    public Toggle userCharacterToggle;

    public void Show(Charactor character)
    {
        gameObject.SetActive(true);

        SetData(character);
    }

    public void SetData(Charactor character)
    {
        iceStepDropdown.value = character.iceStep;
        starToggle.isOn = character.isStar;
        directionToggle.interactable = (character.characterType == Enums.CHARCTER_TYPE.TURN_WHBOMBMON);
        directionToggle.isOn = (character.characterType == Enums.CHARCTER_TYPE.TURN_WHBOMBMON) ? character.isHeightDirection : false;
        userCharacterToggle.isOn = character.isUser;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnChangeIceStep()
    {
        MapManager.Instance.ChangeIceStepOfCharactor(iceStepDropdown.value);
    }

    public void OnChangeStar()
    {
        MapManager.Instance.ChangeStarOfCharactor(starToggle.isOn);
    }
    
    public void OnChangeDirection()
    {
        MapManager.Instance.ChangeDirectionOfCharactor(directionToggle.isOn);
    }

    public void OnClickCharacterFenceButton()
    {
        if (userCharacterToggle.isOn)
        {
            userCharacterToggle.isOn = false;
            MapManager.Instance.ChangeUserCharacterState(false);
        }
        else if (MapManager.Instance.IsAbleToChangeToUserFence())
        {
            userCharacterToggle.isOn = true;
            MapManager.Instance.ChangeUserCharacterState(true);
        }
    }


    public void DeleteCharacterOnSelectFence()
    {
        MapManager.Instance.DeleteCharacterOnSelectFence();

        Hide();
    }

}
