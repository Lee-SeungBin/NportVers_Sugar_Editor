using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MonsterSetMode : MonoBehaviour
{
    private int tileIndex;
    private TileSet selectTileSet;

    public void TouchControll()
    {
        if (Input.GetMouseButtonDown(0))
        {
             OnMouseDownForSetMonster();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (EventSystem.current.IsPointerOverGameObject() == true) return;

            OnMouseUpForSetMonster();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            UIManager.Instance.mapEditManager.characterList.GetComponent<CharacterList>().OnClickRemoveCharacterInList();
        }
    }


    private void OnMouseDownForSetMonster()
    {
        if (EventSystem.current.IsPointerOverGameObject() == true) return;

        SetNullToSelectTileSet();

        UIManager.Instance.mapEditManager.HideCharactorInfoPopup();

        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0, 1 << 10);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                Tile tile = hit.collider.transform.GetComponent<Tile>();
                if (hit.transform.tag.Equals("Tile") && tile.isVisible && !tile.box)
                {
                    tileIndex = hit.transform.GetComponent<Tile>().tileIndex;

                    selectTileSet = hit.transform.parent.gameObject.GetComponent<TileSet>();

                    if (selectedCharacterFromList == null) return;

                    if (selectTileSet.character != null)
                    {
                        Destroy(selectTileSet.character.gameObject);
                        selectTileSet.character = null;
                    }

                    selectTileSet.character = Instantiate(selectedCharacterFromList).GetComponent<Charactor>();
                    selectTileSet.character.name = selectedCharacterFromList.name;
                    selectTileSet.character.tileIndex = tileIndex;
                    selectTileSet.character.transform.SetParent(selectTileSet.transform);
                    selectTileSet.character.transform.localScale = Vector3.one;
                    selectTileSet.character.transform.localPosition = selectTileSet.tiles[tileIndex].transform.localPosition;


                    UIManager.Instance.mapEditManager.ShowCharactorInfoPopup(selectTileSet.character);
                    break;
                }
            }
        }
    }

    private void OnMouseUpForSetMonster()
    {
        if (EventSystem.current.IsPointerOverGameObject() == true) return;

        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0, 1 << 10);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                if (hit.transform.tag.Equals("Tile"))
                {
                    if (selectTileSet == hit.transform.parent.gameObject.GetComponent<TileSet>())
                    {
                        if (selectTileSet.character != null)
                        {
                            UIManager.Instance.mapEditManager.ShowCharactorInfoPopup(selectTileSet.character);
                        }

                        break;
                    }
                }
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
        //if (selectTileSet == null) return;

            //if (selectTileSet.character != null)
            //{
            //    Destroy(selectTileSet.character.gameObject);
            //    selectTileSet.character = null;
            //}

            //if (character == null) return;

            //selectTileSet.character = Instantiate(character).GetComponent<Charactor>();
            //selectTileSet.character.name = character.name;
            //selectTileSet.character.tileIndex = tileIndex;
            //selectTileSet.character.transform.SetParent(selectTileSet.transform);
            //selectTileSet.character.transform.localPosition = selectTileSet.tiles[tileIndex].transform.localPosition;

            //UIManager.Instance.mapEditManager.ShowCharactorInfoPopup(selectTileSet.character);
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
