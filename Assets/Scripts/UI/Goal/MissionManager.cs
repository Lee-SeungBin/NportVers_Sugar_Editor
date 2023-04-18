using UnityEngine;
using UnityEngine.UI;

public class MissionManager : MonoBehaviour
{
    public MissionPopup missionPopup;

    public Text stageNumber;

    public MissionUIBase missionPrefab;

    public Sprite[] missionSprites;

    public GameObject container;

    public void ChangeMissionValue()
    {
        DestroyMissions();
        AddMisions(Enums.MISSION_TYPE.CREAM, missionPopup.creamScoreText.text);
        AddMisions(Enums.MISSION_TYPE.STRAWBERRY, missionPopup.strawberryScoreText.text);
        AddMisions(Enums.MISSION_TYPE.CHOCOLATE, missionPopup.chocolateScoreText.text);
        AddMisions(Enums.MISSION_TYPE.EGG, missionPopup.eggScoreText.text);
        AddMisions(Enums.MISSION_TYPE.BREAD, missionPopup.breadScoreText.text);
        AddMisions(Enums.MISSION_TYPE.ICE, missionPopup.breakIceText.text);
        AddMisions(Enums.MISSION_TYPE.JELLY, missionPopup.jellyText.text);

        if (missionPopup.frogSoup.isOn)
        {
            CreateMission(Enums.MISSION_TYPE.FROG_SOUP, "1");
        }
    }

    private void DestroyMissions()
    {
        for (int i = container.transform.childCount - 1; i >= 0; --i)
        {
            Destroy(container.transform.GetChild(i).gameObject);
        }
    }

    private void AddMisions(Enums.MISSION_TYPE type, string score)
    {
        if (int.Parse(score) > 0)
        {
            CreateMission(type, score);
        }
    }

    private void CreateMission(Enums.MISSION_TYPE missionType, string value)
    {
        MissionUIBase obj = Instantiate(missionPrefab);
        obj.transform.SetParent(container.transform);
        obj.transform.localScale = Vector3.one;
        obj.SetData(missionSprites[(int)missionType], value);
    }
}
