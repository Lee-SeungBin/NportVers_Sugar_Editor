using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSetting : MonoBehaviour
{
    public MapManager mapManager;
    public SpriteChanger bg;

    public Text mapCountText;

    public Dropdown bgDropdown;
    public Dropdown bgmDropdown;

    public Toggle startMove;
    public Toggle moveBuff;
    public Toggle fenceBuff;

    public InputField starPercentText;
    public InputField stageNumberText;


    private void Awake()
    {
        mapCountText.text = "0";

        mapManager.onChangeMaps += ChangeMap;

    }


    public void ChangeMap()
    {
        mapCountText.text = mapManager.Maps.Count.ToString();
    }

    public void SetLoadedData(StageData stageData)
    {
        mapCountText.text = stageData.mapDatas.Length.ToString();
        bgDropdown.value = stageData.bgNumber;
        bgmDropdown.value = stageData.bgmNumber;

        startMove.isOn = stageData.isMoveAtStart == 1;
        moveBuff.isOn = stageData.usePossibleMoveBuff == 1;
        fenceBuff.isOn = stageData.usePossibleJumpBuff == 1;
        starPercentText.text = stageData.starPercent.ToString();
        stageNumberText.text = stageData.stageNumber.ToString();
    }

    public void OnChangeBG()
    {
        bg.SetSprite(bgDropdown.value);
    }
}
