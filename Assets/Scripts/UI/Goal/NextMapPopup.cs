using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextMapPopup : MonoBehaviour
{
    public InputField creamScoreText;
    public InputField strawberryScoreText;
    public InputField chocolateScoreText;
    public InputField eggScoreText;
    public InputField breadScoreText;
    public InputField breakIceText;
    public InputField jellyText;
    public Toggle frogSoup;
    public void SetData(List<NextStageData> data)
    {
        creamScoreText.text = "0";
        strawberryScoreText.text = "0";
        chocolateScoreText.text = "0";
        eggScoreText.text = "0";
        breadScoreText.text = "0";
        breakIceText.text = "0";
        jellyText.text = "0";
        frogSoup.isOn = false;

        for (int i = 0; i < data.Count; ++i)
        {
            switch (data[i].type)
            {
                case 0: creamScoreText.text = data[i].qty.ToString(); break;
                case 1: strawberryScoreText.text = data[i].qty.ToString(); break;
                case 2: chocolateScoreText.text = data[i].qty.ToString(); break;
                case 3: eggScoreText.text = data[i].qty.ToString(); break;
                case 4: breadScoreText.text = data[i].qty.ToString(); break;
                case 5: breakIceText.text = data[i].qty.ToString(); break;
                case 6: jellyText.text = data[i].qty.ToString(); break;
                case 7: frogSoup.isOn = true; break;
            }
        }

        UIManager.Instance.gameDataUI.nextMap.ChangeMissionValue();
    }

    public void OnChangeValue()
    {
        UIManager.Instance.gameDataUI.nextMap.ChangeMissionValue();

        MapManager.Instance.currentMap.nextStageDatas = GetNextStageDatas();
    }

    private NextStageData GetNextStageData(Enums.NEXT_MAP_TYPE type, int value)
    {
        NextStageData data = new NextStageData();
        data.type = (int)type;
        data.qty = value;
        return data;
    }

    public List<NextStageData> GetNextStageDatas()
    {
        List<NextStageData> datas = new List<NextStageData>();

        if (int.TryParse(creamScoreText.text, out int creamScore) && creamScore > 0)
        {
            datas.Add(GetNextStageData(Enums.NEXT_MAP_TYPE.CREAM, creamScore));
        }
        if (int.TryParse(strawberryScoreText.text, out int strawberryScore) && strawberryScore > 0)
        {
            datas.Add(GetNextStageData(Enums.NEXT_MAP_TYPE.STRAWBERRY, strawberryScore));
        }
        if (int.TryParse(chocolateScoreText.text, out int chocolateScore) && chocolateScore > 0)
        {
            datas.Add(GetNextStageData(Enums.NEXT_MAP_TYPE.CHOCOLATE, chocolateScore));
        }
        if (int.TryParse(eggScoreText.text, out int eggScore) && eggScore > 0)
        {
            datas.Add(GetNextStageData(Enums.NEXT_MAP_TYPE.EGG, eggScore));
        }
        if (int.TryParse(breadScoreText.text, out int breadScore) && breadScore > 0)
        {
            datas.Add(GetNextStageData(Enums.NEXT_MAP_TYPE.BREAD, breadScore));
        }
        if (int.TryParse(breakIceText.text, out int breakIceScore) && breakIceScore > 0)
        {
            datas.Add(GetNextStageData(Enums.NEXT_MAP_TYPE.ICE, breakIceScore));
        }
        if (int.TryParse(jellyText.text, out int jellyScore) && jellyScore > 0)
        {
            datas.Add(GetNextStageData(Enums.NEXT_MAP_TYPE.JELLY, jellyScore));
        }
        if (frogSoup.isOn)
        {
            datas.Add(GetNextStageData(Enums.NEXT_MAP_TYPE.FROG_SOUP, 1));
        }

        return datas;
    }
}
