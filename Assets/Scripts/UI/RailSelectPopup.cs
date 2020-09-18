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
        if(straight.isOn)
        {
            railGroup.railType = RailGroup.RAIL_TYPE.STRIGHT;
        }
        else
        {
            railGroup.railType = RailGroup.RAIL_TYPE.ROTATION;
        }
    }

    public void OnClickDeleteButton()
    {
        MapManager.Instance.railMode.SetSelectTileSetNull();
        railGroup.DeleteRailGroup();
        railGroup = null;
    }
}
