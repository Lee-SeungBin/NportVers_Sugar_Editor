using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectTileSetMode : MonoBehaviour
{
    public TileSet selectTileSet;

    //private Vector2 prevMousePosition;

    public void TouchControll()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnMouseDownForTileSet();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
             OnMouseUpForTileSet();
        }
    }


    private void OnMouseDownForTileSet()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        UIManager.Instance.mapEditManager.SetVisibleTileSetPopup(false);

        RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), Mathf.Infinity, 1 << 9);

        if (hit.collider == null || hit.transform.CompareTag("TileSet") == false)
        {
            UIManager.Instance.mapEditManager.SetVisibleTileSetPopup(false);
            return;
        }

        selectTileSet = hit.transform.gameObject.GetComponent<TileSet>();

        if (selectTileSet == null)
        {
            UIManager.Instance.mapEditManager.SetVisibleTileSetPopup(false);
        }
    }

    public void OnMouseUpForTileSet()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (selectTileSet != null)
        {
            UIManager.Instance.mapEditManager.SetVisibleTileSetPopup(true);
            UIManager.Instance.mapEditManager.SetTileSetData(selectTileSet);
        }
    }

    public bool SetTileSetVisible(bool isActive)
    {
        if (selectTileSet != null)
        {
            bool flag = false;
            for (int i = 0; i < 4; ++i)
            {
                if (selectTileSet.tiles[i].jelly != null ||
                    selectTileSet.tiles[i].frogSoup != null ||
                    selectTileSet.tiles[i].box != null)
                    flag = true;
            }
            if (selectTileSet.character != null || 
                selectTileSet.vine != null ||
                flag)
            {
                UIManager.Instance.errorPopup.SetMessage("캐릭터 또는 장애물이 있는 타일 셋은 변경할 수 없습니다.");
            }
            else
            {
                selectTileSet.isVisible = isActive;
                return true;
            }
        }

        return false;
    }

    public bool SetTileVisible(int index, bool isActive)
    {
        if (selectTileSet != null)
        {
            if (selectTileSet.tiles[index].character != null ||
                selectTileSet.tiles[index].jelly != null ||
                selectTileSet.tiles[index].frogSoup != null ||
                selectTileSet.tiles[index].box != null ||
                selectTileSet.vine != null || 
                selectTileSet.railGroup)
            {
                UIManager.Instance.errorPopup.SetMessage("캐릭터 또는 장애물이 있는 타일은 변경할 수 없습니다. 또, 레일이 있는 타일은 타일셋만 변경 가능합니다.");
            }
            else
            {
                selectTileSet.tiles[index].isVisible = isActive;
                return true;
            }
        }

        return false;
    }


    public void SetNullToSelectTileSet()
    {
        selectTileSet = null;
    }
}
