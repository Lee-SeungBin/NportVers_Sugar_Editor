using UnityEngine;

public class RailEditManager : MonoBehaviour
{
    public RailSelectPopup railSelectPopup;

    public void OnChangeSelectModeDropDown(Enums.MAP_SELECT_MODE selectMode)
    {
        gameObject.SetActive(selectMode == Enums.MAP_SELECT_MODE.RAIL_SET);
        //if (selectMode == Enums.MAP_SELECT_MODE.RAIL_SET)
        //{
        //    railSelectPopup.gameObject.SetActive(true);
        //}
    }

    public void SetActiveRailSelectPopup(bool isActive, TileSet tileSet)
    {
        railSelectPopup.gameObject.SetActive(isActive);
        if (isActive)
        {
            railSelectPopup.SetData(tileSet);
        }
    }
}
