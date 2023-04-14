using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RailEditManager : MonoBehaviour
{
    public Button createRailGroupButton;
    public RailCreateView railCreateView;

    public RailSelectPopup railSelectPopup;

    public void OnChangeSelectModeDropDown(MapManager.SELECT_MODE selectMode)
    {
        if (selectMode != MapManager.SELECT_MODE.RAIL_SET)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        createRailGroupButton.gameObject.SetActive(false);
        railCreateView.gameObject.SetActive(false);
        railSelectPopup.gameObject.SetActive(false);
    }

    public void SetInitState()
    {
        createRailGroupButton.gameObject.SetActive(true);
        railCreateView.gameObject.SetActive(false);
    }

    public void SetCreateState()
    {
        createRailGroupButton.gameObject.SetActive(false);
        railCreateView.gameObject.SetActive(true);
    }

    public void SetActiveRailSelectPopup(bool isActive, TileSet tileSet)
    {
        //print("isActive : " + isActive);
        railSelectPopup.gameObject.SetActive(isActive);

        if(isActive)
        {
            railSelectPopup.SetData(tileSet);
        }
    }
}
