using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RailSelectPopup : MonoBehaviour
{
    public delegate void OnSelectMoveNumberOfTile(int moveNumber);
    public OnSelectMoveNumberOfTile onSelectMoveNumberOfTileCallback;

    private RailGroup railGroup;
    [SerializeField]
    private Toggle straight;
    [SerializeField]
    private Toggle rotation;

    public void SetData(TileSet tileSet)
    {
        railGroup = tileSet.railGroup;

        if (railGroup.railType == RailGroup.RAIL_TYPE.STRIGHT)
        {
            straight.isOn = true;
        }
        else
        {
            rotation.isOn = true;
        }
    }

    public void OnChangeRailType()
    {
        if (railGroup != null)
        {
            if (straight.isOn)
            {
                railGroup.railType = RailGroup.RAIL_TYPE.STRIGHT;
            }
            else
            {
                railGroup.railType = RailGroup.RAIL_TYPE.ROTATION;
            }
        }
        else
        {
            UIManager.Instance.errorPopup.SetMessage("레일이 없습니다.");
        }
    }

    public void OnClickDeleteButton()
    {
        MapManager.Instance.railMode.SetSelectTileSetNull();
        if (railGroup != null)
            railGroup.DeleteRailGroup();
        else
            UIManager.Instance.errorPopup.SetMessage("레일을 선택해주세요.");
        railGroup = null;
    }
}
