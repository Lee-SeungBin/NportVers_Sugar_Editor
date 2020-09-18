using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectTileSetMode : MonoBehaviour
{
    private TileSet selectTileSet;

    private Vector2 prevMousePosition;

    public void TouchControll()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnMouseDownForTileSet();

            prevMousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (EventSystem.current.IsPointerOverGameObject() == true) return;

             OnMouseUpForTileSet();
        }
        else if (Input.GetMouseButton(0))
        {
            if (selectTileSet != null)
            {
                if (prevMousePosition != (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition))
                {
                    selectTileSet = null;
                }
            }
        }
    }


    private void OnMouseDownForTileSet()
    {
        if (EventSystem.current.IsPointerOverGameObject() == true) return;

        //if (selectTileSet != null) return;

        //RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0, 1 << 9);
        //foreach (RaycastHit2D hit in hits)
        //{
        //    if (hit.collider != null)
        //    {
        //        if (hit.transform.tag.Equals("TileSet"))
        //        {
        //            selectMapContainer = hit.transform.parent.gameObject;

        //            prevMousePosition = Input.mousePosition;

        //            return;
        //        }
        //    }
        //}

        UIManager.Instance.mapEditManager.SetVisibleTileSetPopup(false);


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, 1 << 9);

        if (hit.collider == null)
        {
            UIManager.Instance.mapEditManager.SetVisibleTileSetPopup(false);
            return;
        }
        if (hit.transform.CompareTag("TileSet") == false)
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
        if (EventSystem.current.IsPointerOverGameObject() == true) return;

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
            if(selectTileSet.character != null)
            {
                UIManager.Instance.errorPopup.SetMessage("캐릭터가 있는 울타리는 없앨 수 없습니다.");
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
            if (selectTileSet.tiles[index].character != null)
            {
                UIManager.Instance.errorPopup.SetMessage("캐릭터가 있는 타일은 없앨 수 없습니다.");
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
