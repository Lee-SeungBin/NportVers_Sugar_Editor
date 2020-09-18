using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextMapMissionUI : MonoBehaviour
{    
    public MissionUIBase missionPrefab;

    public Sprite[] missionSprites;

    public GameObject container;

    public void ChangeMissionValue()
    {
        if (container.transform.childCount > 0)
        {
            for (int i = container.transform.childCount - 1; i > -1; --i)
            {
                Destroy(container.transform.GetChild(i).gameObject);
            }
        }

        NextMapPopup popup = UIManager.Instance.mapEditManager.popups.nextMap;

        if (int.Parse(popup.creamScoreText.text) > 0)
        {
            CreateMission(Enums.NEXT_MAP_TYPE.CREAM, popup.creamScoreText.text);
        }

        if (int.Parse(popup.strawberryScoreText.text) > 0)
        {
            CreateMission(Enums.NEXT_MAP_TYPE.STRAWBERRY, popup.strawberryScoreText.text);
        }

        if (int.Parse(popup.chocolateScoreText.text) > 0)
        {
            CreateMission(Enums.NEXT_MAP_TYPE.CHOCOLATE, popup.chocolateScoreText.text);
        }

        if (int.Parse(popup.eggScoreText.text) > 0)
        {
            CreateMission(Enums.NEXT_MAP_TYPE.EGG, popup.eggScoreText.text);
        }

        if (int.Parse(popup.breadScoreText.text) > 0)
        {
            CreateMission(Enums.NEXT_MAP_TYPE.BREAD, popup.breadScoreText.text);
        }

        if (int.Parse(popup.breakIceText.text) > 0)
        {
            CreateMission(Enums.NEXT_MAP_TYPE.ICE, popup.breakIceText.text);
        }

        if (int.Parse(popup.jellyText.text) > 0)
        {
            CreateMission(Enums.NEXT_MAP_TYPE.JELLY, popup.jellyText.text);
        }

        if (popup.frogSoup.isOn)
        {
            CreateMission(Enums.NEXT_MAP_TYPE.FROG_SOUP, "1");
        }
    }


    private void CreateMission(Enums.NEXT_MAP_TYPE missionType, string value)
    {
        MissionUIBase obj = Instantiate(missionPrefab);
        obj.transform.SetParent(container.transform);
        obj.transform.localScale = Vector3.one;
        obj.SetData(missionSprites[(int)missionType], value);
    }
}
