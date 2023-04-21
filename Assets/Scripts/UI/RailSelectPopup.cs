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

    public Text railinfo;

    public void SetData(TileSet tileSet)
    {
        railGroup = tileSet.railGroup;
        if (railGroup.railType == Enums.RAIL_TYPE.STRIGHT)
            straight.isOn = true;
        if (railGroup.railType == Enums.RAIL_TYPE.ROTATION)
            rotation.isOn = true;
        railinfo.text = "현재 레일 모드 : " + railGroup.railType + "\n현재 레일 그룹 : " + MapManager.Instance.currentMap.railManager.railGroups.IndexOf(MapManager.Instance.railMode.selectTileSet.railGroup);
        //straight.isOn = railGroup.railType == Enums.RAIL_TYPE.STRIGHT;
        //rotation.isOn = railGroup.railType == Enums.RAIL_TYPE.ROTATION;
    }

    public void OnChangeRailType()
    {
        if (railGroup == null)
        {
            UIManager.Instance.errorPopup.SetMessage("레일이 없습니다.");
            return;
        }

        railGroup.railType = straight.isOn ? Enums.RAIL_TYPE.STRIGHT : Enums.RAIL_TYPE.ROTATION;
        SetData(MapManager.Instance.railMode.selectTileSet);
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
