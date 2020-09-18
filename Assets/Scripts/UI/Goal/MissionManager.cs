using UnityEngine;
using UnityEngine.UI;

public class MissionManager : MonoBehaviour
{
    public enum MISSION_TYPE
    {
        CREAM = 0,
        STRAWBERRY = 1,
        CHOCOLATE = 2,
        EGG= 3,
        BREAD = 4,
        ICE = 5,
        JELLY = 6,
        FROG_SOUP = 7
    }


    public MissionPopup missionPopup;

    public Text stageNumber;

    public MissionUIBase missionPrefab;

    public Sprite[] missionSprites;

    public GameObject container;

    public void ChangeMissionValue()
    {
        if(container.transform.childCount > 0)
        {
            for(int i = container.transform.childCount - 1; i > -1; --i)
            {
                Destroy(container.transform.GetChild(i).gameObject);
            }
        }

        if (int.Parse(missionPopup.creamScoreText.text) > 0)
        {
            CreateMission(MISSION_TYPE.CREAM, missionPopup.creamScoreText.text);
        }

        if (int.Parse(missionPopup.strawberryScoreText.text) > 0)
        {
            CreateMission(MISSION_TYPE.STRAWBERRY, missionPopup.strawberryScoreText.text);
        }

        if (int.Parse(missionPopup.chocolateScoreText.text) > 0)
        {
            CreateMission(MISSION_TYPE.CHOCOLATE, missionPopup.chocolateScoreText.text);
        }

        if (int.Parse(missionPopup.eggScoreText.text) > 0)
        {
            CreateMission(MISSION_TYPE.EGG, missionPopup.eggScoreText.text);
        }
        
        if (int.Parse(missionPopup.breadScoreText.text) > 0)
        {
            CreateMission(MISSION_TYPE.BREAD, missionPopup.breadScoreText.text);
        }

        if (int.Parse(missionPopup.breakIceText.text) > 0)
        {
            CreateMission(MISSION_TYPE.ICE, missionPopup.breakIceText.text);
        }

        if (int.Parse(missionPopup.jellyText.text) > 0)
        {
            CreateMission(MISSION_TYPE.JELLY, missionPopup.jellyText.text);
        }
        
        if (missionPopup.frogSoup.isOn)
        {
            CreateMission(MISSION_TYPE.FROG_SOUP, "1");
        }
    }

    private void CreateMission(MISSION_TYPE missionType, string value)
    {
        MissionUIBase obj = Instantiate(missionPrefab);
        obj.transform.SetParent(container.transform);
        obj.transform.localScale = Vector3.one;
        obj.SetData(missionSprites[(int)missionType], value);
    }
}
