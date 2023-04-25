using UnityEngine;

public class NextMapMissionUI : MonoBehaviour
{
    public MissionUIBase missionPrefab;

    public Sprite[] missionSprites;

    public GameObject container;

    public void ChangeMissionValue()
    {
        if (container.transform.childCount == 0)
        {
            return;
        }

        NextMapPopup popup = UIManager.Instance.mapEditManager.popups.nextMap;

        AddNextMissions(Enums.NEXT_MAP_TYPE.CREAM, popup.creamScoreText.text);
        AddNextMissions(Enums.NEXT_MAP_TYPE.STRAWBERRY, popup.strawberryScoreText.text);
        AddNextMissions(Enums.NEXT_MAP_TYPE.CHOCOLATE, popup.chocolateScoreText.text);
        AddNextMissions(Enums.NEXT_MAP_TYPE.EGG, popup.eggScoreText.text);
        AddNextMissions(Enums.NEXT_MAP_TYPE.BREAD, popup.breadScoreText.text);
        AddNextMissions(Enums.NEXT_MAP_TYPE.ICE, popup.breakIceText.text);
        AddNextMissions(Enums.NEXT_MAP_TYPE.JELLY, popup.jellyText.text);

        if (popup.frogSoup.isOn)
        {
            CreateMission(Enums.NEXT_MAP_TYPE.FROG_SOUP, "1");
        }
    }

    private void AddNextMissions(Enums.NEXT_MAP_TYPE type, string scoreText)
    {
        if (int.TryParse(scoreText, out int score) && score > 0)
        {
            CreateMission(type, scoreText);
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
