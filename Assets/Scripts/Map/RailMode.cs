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
        if (EventSystem.current.IsPointerOverGameObject()) return;

        RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), Mathf.Infinity, 1 << 9);

        if (hit == false || hit.transform.CompareTag("TileSet") == false)
            return;

        TileSet clickedTileSet = hit.transform.GetComponent<TileSet>();

        int emptyTiles = 0; // 비어 있는 타일 체크
        foreach (Tile tile in clickedTileSet.tiles)
        {
            if (!tile.isVisible)
            {
                emptyTiles++;
            }
        }
        if (emptyTiles > 0 && emptyTiles < 4)
        {
            UIManager.Instance.errorPopup.SetMessage("레일은 타일이 전부 없거나 있어야 설치 가능합니다.");
            return;
        }

        if (selectTileSet == null)
        {
            selectTileSet = clickedTileSet;
            if (selectTileSet.railGroup == null)
            {
                RailGroup railGroup = selectTileSet.map.railManager.CreateRailGroup();
                railGroup.PushTileSet(selectTileSet);
            }
        }
        else if (selectTileSet == clickedTileSet)
        {
            if (selectTileSet.railGroup != null && CheckErrorRail(selectTileSet) == false) // 레일 그룹을 만들고 선택 해제하려는 순간 현재 레일 체크
                return;
            selectTileSet.railGroup?.UnselectGroup();
            selectTileSet = null;
        }
        else
        {
            bool isVisibleTiles = clickedTileSet.isVisibleTiles;
            if (isVisibleTiles == false)
            {
                if (selectTileSet.railGroup != null && CheckErrorRail(selectTileSet) == false) // 다른 레일 그룹을 만들기 전에 현재 레일 체크
                    return;
                selectTileSet.railGroup?.UnselectGroup();
                selectTileSet = clickedTileSet;
                if (selectTileSet.railGroup == null)
                {
                    RailGroup railGroup = selectTileSet.map.railManager.CreateRailGroup();
                    railGroup.PushTileSet(selectTileSet);
                }
            }
            else
            {
                UnselectGroup(selectTileSet);
                selectTileSet.railGroup.PushTileSet(clickedTileSet);
            }
        }

        if (selectTileSet != null)
        {
            selectTileSet.railGroup?.SelectGroup();
            //selectTileSet = selectTileSet.railGroup.GetLastTileSet();
            UIManager.Instance.railEditManager.SetActiveRailSelectPopup(true, selectTileSet);
        }
        else
        {
            UIManager.Instance.railEditManager.SetActiveRailSelectPopup(false, selectTileSet);
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
        if (w < 0 || h < 0 || w >= map.width || h >= map.height)
        {
            return;
        }

        TileSet tileSet = map.tileSets[w][h];
        if (isActive && (tileSet.railGroup == null || tileSet.railGroup == selectTileSet.railGroup))
        {
            tileSet.SetVisibleAllTilesForRailMode(true);
        }
        else
        {
            tileSet.SetVisibleAllTilesForRailMode(false);
        }
    }

    public void SetSelectTileSetNull()
    {
        if (selectTileSet != null)
        {
            selectTileSet.railGroup.UnselectGroup();
            selectTileSet = null;
        }
    }
    /// <summary>
    /// 직선과 회전 레일의 조건 체크 함수
    /// </summary>
    /// <param name="tileSet"></param>
    /// <returns></returns>
    public bool CheckErrorRail(TileSet tileSet)
    {
        TileSet lastTile = tileSet.railGroup.GetLastTileSet();

        int w = lastTile.tileSetIndex / lastTile.map.height;
        int h = lastTile.tileSetIndex % lastTile.map.height;

        if (tileSet.railGroup.railType == 0) // 회전 레일이면
        {
            if (!((w > 0 && lastTile.map.tileSets[w - 1][h].tileSetIndex == tileSet.railGroup.tileSets[0].tileSetIndex) ||
                  (w < lastTile.map.width - 1 && lastTile.map.tileSets[w + 1][h].tileSetIndex == tileSet.railGroup.tileSets[0].tileSetIndex) ||
                  (h > 0 && lastTile.map.tileSets[w][h - 1].tileSetIndex == tileSet.railGroup.tileSets[0].tileSetIndex) ||
                  (h < lastTile.map.height - 1 && lastTile.map.tileSets[w][h + 1].tileSetIndex == tileSet.railGroup.tileSets[0].tileSetIndex)) ||
                  (tileSet.railGroup.tileSets.Count <= 2)) // 마지막 레일의 인덱스를 통해 4방향에서 첫번째 타일과 인접한지 체크 및 2개 이하 인지 체크
            {
                UIManager.Instance.errorPopup.SetMessage("!!주의!!\n\n회전 레일은 두 개 일수 없으며 맨 끝과 맨 처음의 타일이 맞닿아 있어야 합니다.");
                return false;
            }
        }
        else // 직선 레일 이면
        {
            if (lastTile.isVisible || tileSet.railGroup.tileSets.Count <= 1) // 마지막 레일 타일이 비어 있는지 체크
            {
                UIManager.Instance.errorPopup.SetMessage("!!주의!!\n\n직선 레일은 한 개 일수 없으며 맨 끝 타일은 무조건 비어 있어야 합니다.");
                return false;
            }
        }
        return true;
    }

}
