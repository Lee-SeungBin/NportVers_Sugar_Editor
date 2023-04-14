using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapcountManager : MonoBehaviour
{
    public Text mapAllCountText;
    public Text mapCurrentNumberText;

    public int mapCurrentNumber;

    private static MapcountManager _instance;
    public static MapcountManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }
    public void Init(int mapAllCount, int mapCurrentNum = 1)
    {
        mapAllCountText.text = mapAllCount.ToString();
        mapCurrentNumber = mapCurrentNum;
        mapCurrentNumberText.text = mapCurrentNumber.ToString();

        if (MapManager.Instance.Maps.Count > 0)
        {
            UIManager.Instance.mapEditManager.popups.nextMap.SetData(MapManager.Instance.Maps[mapCurrentNumber - 1].nextStageDatas);
        }
        
    }
    public void MapMoveButtonClick(bool isNextClick = true)
    {
        if (MapManager.Instance.Maps.Count < 2)
            return;

        if (isNextClick)
        {
            if (mapCurrentNumber == MapManager.Instance.Maps.Count)
            {
                mapCurrentNumber = 1;
            }
            else
            {
                mapCurrentNumber++;
            }
        }
        else
        {
            if (mapCurrentNumber == 1)
            {
                mapCurrentNumber = MapManager.Instance.Maps.Count;
            }
            else
            {
                mapCurrentNumber--;
            }
        }

        mapCurrentNumberText.text = mapCurrentNumber.ToString();


        Vector3 cameraPos = MapManager.Instance.mainCamera.transform.position;

        cameraPos.x = MapManager.Instance.Maps[mapCurrentNumber - 1].transform.localPosition.x;

        UIManager.Instance.mapEditManager.popups.nextMap.SetData(MapManager.Instance.Maps[mapCurrentNumber - 1].nextStageDatas);

        MapManager.Instance.HideWoodenFence();

        CameraMove(cameraPos);
    }

    void CameraMove(Vector3 newPosVec3)
    {
        Camera.allCameras[0].transform.localPosition = newPosVec3;
    }
}
