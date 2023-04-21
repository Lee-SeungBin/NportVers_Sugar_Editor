using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MonsterSetMode : MonoBehaviour
{
    private TileSet selectTileSet;

    public void TouchControll()
    {
        if (Input.GetMouseButtonDown(0))
        {
             OnMouseDownForSetMonster();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            OnMouseUpForSetMonster();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            UIManager.Instance.mapEditManager.characterList.GetComponent<CharacterList>().OnClickRemoveCharacterInList();
        }
    }


    private void OnMouseDownForSetMonster()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        SetNullToSelectTileSet();

        UIManager.Instance.mapEditManager.HideCharactorInfoPopup();

        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0, 1 << 10);

        foreach (RaycastHit2D hit in hits)
        {
            Tile tile = hit.collider?.GetComponent<Tile>();
            if (tile != null && tile.isVisible && !tile.box)
            {
                selectTileSet = tile.transform.parent.GetComponent<TileSet>();
                if (selectedCharacterFromList == null || selectTileSet.character != null) return;

                selectTileSet.character = Instantiate(selectedCharacterFromList, selectTileSet.transform).GetComponent<Charactor>();
                selectTileSet.character.name = selectedCharacterFromList.name;
                selectTileSet.character.tileIndex = tile.tileIndex;
                selectTileSet.character.transform.localPosition = selectTileSet.tiles[tile.tileIndex].transform.localPosition;
                UIManager.Instance.mapEditManager.ShowCharactorInfoPopup(selectTileSet.character);

                break;
            }
        }
    }

    private void OnMouseUpForSetMonster()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        foreach (RaycastHit2D hit in Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0, 1 << 10))
        {
            TileSet hitTileSet = hit.transform.parent.GetComponent<TileSet>();
            if (hitTileSet != null && hit.transform.CompareTag("Tile") && hitTileSet == selectTileSet && selectTileSet.character != null)
            {
                UIManager.Instance.mapEditManager.ShowCharactorInfoPopup(selectTileSet.character);
                break;
            }
        }
    }


    public void SetNullToSelectTileSet()
    {
        selectTileSet = null;
    }

    private GameObject selectedCharacterFromList;
    public void ChangeCharactor(GameObject character)
    {
        if (selectedCharacterFromList != character)
            selectedCharacterFromList = character;
        else
            selectedCharacterFromList = null;
    }

    public void DeleteCharacterOnSelectFence()
    {
        if (selectTileSet.character == null) return;
        ChangeUserCharacterState(false);
        Destroy(selectTileSet.character.gameObject);

        selectTileSet.character = null;
    }

    public void ChangeIceStepOfCharactor(int step)
    {
        selectTileSet.character.iceStep = step;
    }

    public void ChangeStarOfCharactor(bool isStar)
    {
        selectTileSet.character.isStar = isStar;
    }

    public void ChangeDirectionOfCharactor(bool isHeightDirection)
    {
        selectTileSet.character.isHeightDirection = isHeightDirection;
    }

    public bool IsUserCharacter()
    {
        return selectTileSet.character.isUser;
    }

    public void ChangeUserCharacterState(bool isUser)
    {
        selectTileSet.character.isUser = isUser;
        selectTileSet.SetTheme();
    }
}
