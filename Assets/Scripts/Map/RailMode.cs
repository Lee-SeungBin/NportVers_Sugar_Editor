using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RailMode : MonoBehaviour
{
    public TileSet selectTileSet;

    public void TouchControll()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnMouseDown();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            //if (selectTileSet.railGroup != null)
            //    selectTileSet.railGroup.UnselectGroup();
        }
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject() == true) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, 1 << 9);

        if (hit.collider == null) return;
        if (hit.transform.CompareTag("TileSet") == false) return;

        if(selectTileSet == null)
        {
            selectTileSet = hit.transform.GetComponent<TileSet>();
            int cnt = 0;
            for (int i = 0; i < 4; i++)
            {
                if (!selectTileSet.tiles[i].isVisible)
                    cnt++;
            }
            if (cnt > 0 && cnt < 4)
            {
                UIManager.Instance.errorPopup.SetMessage("레일은 타일이 전부 없거나 있어야 설치 가능합니다.");
                return;
            }

            if (selectTileSet.railGroup == null)
            {
                RailGroup railGroup = selectTileSet.map.railManager.CreateRailGroup();
                railGroup.PushTileSet(selectTileSet);
            }

            selectTileSet.railGroup.SelectGroup();
            selectTileSet = selectTileSet.railGroup.GetLastTileSet();

            UIManager.Instance.railEditManager.SetActiveRailSelectPopup(true, selectTileSet);
        }
        else
        {
            TileSet tempTileSet = hit.transform.GetComponent<TileSet>();
            int cnt = 0;
            for (int i = 0; i < 4; i++)
            {
                if (!tempTileSet.tiles[i].isVisible)
                    cnt++;
            }
            if (cnt > 0 && cnt < 4)
            {
                UIManager.Instance.errorPopup.SetMessage("레일은 타일이 전부 없거나 있어야 설치 가능합니다.");
                return;
            }
            if (selectTileSet == tempTileSet)
            {
                if (selectTileSet.railGroup != null)
                {
                    if (!CheckErrorRail(selectTileSet))
                        return;
                    selectTileSet.railGroup.UnselectGroup();

                }

                selectTileSet = null;
            }
            else
            {
                bool isVisibleTiles = tempTileSet.isVisibleTiles;

                if (isVisibleTiles == false) //새로운 그룹 생성
                {
                    if (!CheckErrorRail(selectTileSet))
                        return;
                    if (selectTileSet.railGroup != null)
                    {
                        selectTileSet.railGroup.UnselectGroup();
                    }
                    selectTileSet = tempTileSet;

                    if (tempTileSet.railGroup == null)
                    {
                        RailGroup railGroup = selectTileSet.map.railManager.CreateRailGroup();
                        railGroup.PushTileSet(selectTileSet);

                    }

                }
                else
                {
                    UnselectGroup(selectTileSet);
                    selectTileSet.railGroup.PushTileSet(tempTileSet);
                }
    
                selectTileSet.railGroup.SelectGroup();
                selectTileSet = selectTileSet.railGroup.GetLastTileSet();

                UIManager.Instance.railEditManager.SetActiveRailSelectPopup(true, selectTileSet);
            }



        }
    }

    public void SelectGroup(TileSet tileSet)
    {
        int w = tileSet.tileSetIndex / tileSet.map.height;
        int h = tileSet.tileSetIndex % tileSet.map.height;

        SelectableFenceActivation(tileSet.map, w, h - 1, true);
        SelectableFenceActivation(tileSet.map, w, h + 1, true);
        SelectableFenceActivation(tileSet.map, w - 1, h, true);
        SelectableFenceActivation(tileSet.map, w + 1, h, true);
    }

    public void UnselectGroup(TileSet tileSet)
    {
        int w = tileSet.tileSetIndex / tileSet.map.height;
        int h = tileSet.tileSetIndex % tileSet.map.height;

        SelectableFenceActivation(tileSet.map, w, h - 1, false);
        SelectableFenceActivation(tileSet.map, w, h + 1, false);
        SelectableFenceActivation(tileSet.map, w - 1, h, false);
        SelectableFenceActivation(tileSet.map, w + 1, h, false);
    }

    private void SelectableFenceActivation(Map map, int w, int h, bool isActive)
    {
        if(w < 0 || h < 0 || w >= map.width || h >= map.height)
        {
            return;
        }

        TileSet tileSet = map.tileSets[w][h];
        if(isActive)
        {
            if(tileSet.railGroup == null || tileSet.railGroup == selectTileSet.railGroup)
            {
                tileSet.SetVisibleAllTilesForRailMode(true);
            }
        }
        else
        {
            tileSet.SetVisibleAllTilesForRailMode(false);
        }
    }

    public void SetSelectTileSetNull()
    {
        if(selectTileSet != null)
        {
            selectTileSet.railGroup.UnselectGroup();
            selectTileSet = null;
        }
    }

    public bool CheckErrorRail(TileSet tileSet)
    {

        TileSet lastTile = tileSet.railGroup.GetLastTileSet();

        int a = lastTile.tileSetIndex / lastTile.map.height;
        int b = lastTile.tileSetIndex % lastTile.map.height;

        if (tileSet.railGroup.railType == 0)
        {
            if (!((a > 0 && lastTile.map.tileSets[a - 1][b].tileSetIndex == tileSet.railGroup.tileSets[0].tileSetIndex) ||
                  (a < lastTile.map.width - 1 && lastTile.map.tileSets[a + 1][b].tileSetIndex == tileSet.railGroup.tileSets[0].tileSetIndex) ||
                  (b > 0 && lastTile.map.tileSets[a][b - 1].tileSetIndex == tileSet.railGroup.tileSets[0].tileSetIndex) ||
                  (b < lastTile.map.height - 1 && lastTile.map.tileSets[a][b + 1].tileSetIndex == tileSet.railGroup.tileSets[0].tileSetIndex)))
            {
                UIManager.Instance.errorPopup.SetMessage("!!주의!!\n\n회전 레일은 맨 끝과 맨 처음의 타일이 맞닿아 있어야 합니다.");
                return false;
            }
        }
        else
        {
            if (tileSet.railGroup.tileSets[0].isVisible && tileSet.railGroup.tileSets[tileSet.railGroup.tileSets.Count - 1].isVisible)
            {
                UIManager.Instance.errorPopup.SetMessage("!!주의!!\n\n직선 레일은 맨 끝 또는 맨 처음의 타일이 비어 있어야 합니다.");
                return false;
            }
        }

        return true;
    }

}
