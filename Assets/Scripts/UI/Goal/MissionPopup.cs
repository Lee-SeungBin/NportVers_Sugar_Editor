using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionPopup : MonoBehaviour
{

    public InputField creamScoreText;
    public InputField strawberryScoreText;
    public InputField chocolateScoreText;
    public InputField eggScoreText;
    public InputField breadScoreText;
    public InputField breakIceText;
    public InputField jellyText;
    public InputField BoxText, Box3Text, Box5Text, ChurrosText;
    public Toggle frogSoup;


    public void SetLoadedData(StageData stageData)
    {
        creamScoreText.text = "0";
        strawberryScoreText.text = "0";
        chocolateScoreText.text = "0";
        eggScoreText.text = "0";
        breadScoreText.text = "0";
        breakIceText.text = "0";
        jellyText.text = "0";
        frogSoup.isOn = false;
        BoxText.text = "0";
        Box3Text.text = "0";
        Box5Text.text = "0";
        ChurrosText.text = "0";

        for (int i = 0; i < stageData.missions.Length; ++i)
        {
            switch (stageData.missions[i].type)
            {
                case 0: creamScoreText.text = stageData.missions[i].qty.ToString(); break;
                case 1: strawberryScoreText.text = stageData.missions[i].qty.ToString(); break;
                case 2: chocolateScoreText.text = stageData.missions[i].qty.ToString(); break;
                case 3: eggScoreText.text = stageData.missions[i].qty.ToString(); break;
                case 4: breadScoreText.text = stageData.missions[i].qty.ToString(); break;
                case 5: breakIceText.text = stageData.missions[i].qty.ToString(); break;
                case 6: jellyText.text = stageData.missions[i].qty.ToString(); break;
                case 7: frogSoup.isOn = true; break;
                case 8: BoxText.text = stageData.missions[i].qty.ToString(); break;
                case 9: Box3Text.text = stageData.missions[i].qty.ToString(); break;
                case 10: Box5Text.text = stageData.missions[i].qty.ToString(); break;
                case 11: ChurrosText.text = stageData.missions[i].qty.ToString(); break;

            }
        }

    }

    public int missionCount
    {
        get
        {
            int count = 0;
            if (creamScoreText.text != "0") ++count;
            if (strawberryScoreText.text != "0") ++count;
            if (chocolateScoreText.text != "0") ++count;
            if (eggScoreText.text != "0") ++count;
            if (breadScoreText.text != "0") ++count;
            if (breakIceText.text != "0") ++count;
            if (jellyText.text != "0") ++count;
            if (frogSoup.isOn) ++count;
            if (BoxText.text != "0") ++count;
            if (Box3Text.text != "0") ++count;
            if (Box5Text.text != "0") ++count;
            if (ChurrosText.text != "0") ++count;
            return count;
        }
    }

    public List<Mission> GetUsingMissionDatas()
    {
        List<Mission> missions = new List<Mission>();
        PushMissionData(missions, 0, creamScoreText.text);
        PushMissionData(missions, 1, strawberryScoreText.text);
        PushMissionData(missions, 2, chocolateScoreText.text);
        PushMissionData(missions, 3, eggScoreText.text);
        PushMissionData(missions, 4, breadScoreText.text);
        PushMissionData(missions, 5, breakIceText.text);
        PushMissionData(missions, 6, jellyText.text);
        if (frogSoup.isOn)
        {
            int frog = 0;
            for (int i = MapManager.Instance.Maps.Count - 1; i > -1; --i)
            {
                frog += MapManager.Instance.Maps[i].frogSoups.Count;
            }
            PushMissionData(missions, 7, frog.ToString());
        }
        PushMissionData(missions, 8, BoxText.text);
        PushMissionData(missions, 9, Box3Text.text);
        PushMissionData(missions, 10, Box5Text.text);
        PushMissionData(missions, 11, ChurrosText.text);


        return missions;
    }

    private void PushMissionData(List<Mission> list, int type, string qty)
    {
        if (qty == "0") return;

        if (int.Parse(qty) > 0)
        {
            Mission mission = new Mission();
            mission.type = type;
            mission.qty = int.Parse(qty);
            list.Add(mission);
        }
    }

    public void OnChangeValue()
    {
        if (string.IsNullOrEmpty(creamScoreText.text) ||
            string.IsNullOrEmpty(chocolateScoreText.text) ||
            string.IsNullOrEmpty(strawberryScoreText.text) ||
            string.IsNullOrEmpty(eggScoreText.text) ||
            string.IsNullOrEmpty(breadScoreText.text) ||
            string.IsNullOrEmpty(jellyText.text) ||
            string.IsNullOrEmpty(BoxText.text) ||
            string.IsNullOrEmpty(Box3Text.text) ||
            string.IsNullOrEmpty(Box5Text.text) ||
            string.IsNullOrEmpty(ChurrosText.text)
            )
        {
            return;
        }
        UIManager.Instance.gameDataUI.mission.ChangeMissionValue();
    }
}
