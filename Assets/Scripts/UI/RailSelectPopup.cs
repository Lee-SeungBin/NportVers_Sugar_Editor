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
        straight.isOn = railGroup.railType == Enums.RAIL_TYPE.STRIGHT;
        rotation.isOn = railGroup.railType == Enums.RAIL_TYPE.ROTATION;
    }

    public void OnChangeRailType()
    {
        if (railGroup == null)
        {
            UIManager.Instance.errorPopup.SetMessage("레일이 없습니다.");
            return;
        }

        railGroup.railType = straight.isOn ? Enums.RAIL_TYPE.STRIGHT : Enums.RAIL_TYPE.ROTATION;
    }

    public void OnClickDeleteButton()
    {
        if (railGroup == null)
        {
            UIManager.Instance.errorPopup.SetMessage("레일을 선택해주세요.");
            return;
        }

        MapManager.Instance.railMode.SetSelectTileSetNull();
        railGroup.DeleteRailGroup();
        railGroup = null;
    }
}
