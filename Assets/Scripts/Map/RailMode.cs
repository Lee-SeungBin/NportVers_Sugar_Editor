﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RailMode : MonoBehaviour
{
    private TileSet selectTileSet;

    public void TouchControll()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnMouseDown();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (selectTileSet.railGroup != null)
                selectTileSet.railGroup.UnselectGroup();
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
                    selectTileSet.railGroup.UnselectGroup();
                selectTileSet = null;
            }
            else
            {
                bool isVisibleTiles = tempTileSet.isVisibleTiles;

                if (isVisibleTiles == false) //새로운 그룹 생성
                {
                    if (selectTileSet.railGroup != null)
                        selectTileSet.railGroup.UnselectGroup();

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

}
