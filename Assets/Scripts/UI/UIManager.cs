using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public MapEditManager mapEditManager;
    public RailEditManager railEditManager;

    public BGDropDown bgDropdown;
    public Dropdown bgmDropdown;

    public Toggle starGauge, startingMove, moveBuff, jumpBuff;

    public InputField starPercent, stageNumber, moveText, jumpText, loadStageNumber;
    public InputField mapPositionX, mapPositionY;

    public Button uiOnOffButton;
    private bool isUIOn;

    public GameObject container, stagePosition, mapPosition;
    public GameObject saveStageWarnningPopup, loadStageWarnningPopup, loadStagePopup, updateListPopup;

    public Dropdown selectModeDropDown, stageType, currentChapter;

    public Text mapCount, stagePositionText, mapSize;

    public ObstacleOptionPopup obstacleOptionPopup;
    public DragItem dragItem;
    public ErrorMessagePopup errorPopup;
    public GameDataUI gameDataUI;

    public CryptoMNG cryptoMNG;
    public NetworkMNG networkMNG;
    public MapDataMNG mapdataMNG;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetUIOnOff();

        HideStagePosition();

        OnClickLoadCancelButton();

        OnChangeSelectModeDropDown();

        dragItem.gameObject.SetActive(false);
    }

    public void SetUIOnOff()
    {
        isUIOn = !isUIOn;

        container.SetActive(isUIOn);

        if (isUIOn)
        {
            uiOnOffButton.transform.Find("Text").GetComponent<Text>().text = "UI ON";

        }
        else
        {
            uiOnOffButton.transform.Find("Text").GetComponent<Text>().text = "UI OFF";
        }
    }


    public void OnChangeSelectModeDropDown()
    {
        MapManager.Instance.ChangeSelectMode(selectModeDropDown.value);
        mapEditManager.OnChangeSelectModeDropDown((Enums.MAP_SELECT_MODE)selectModeDropDown.value);
        railEditManager.OnChangeSelectModeDropDown((Enums.MAP_SELECT_MODE)selectModeDropDown.value);
    }

    public void ShowStagePosition()
    {
        stagePosition.SetActive(true);
    }

    public void HideStagePosition()
    {
        stagePosition.SetActive(false);
    }

    public void MapPostionActive()
    {
        if (mapPosition.activeSelf)
            mapPosition.SetActive(false);
        else
            mapPosition.SetActive(true);
    }

    public void SetStagePosition(Vector2 stagePositionValue, Vector2 uiPositionValue)
    {
        stagePosition.transform.position = uiPositionValue;
        stagePositionText.text = "x:" + stagePositionValue.x + "\ny:" + stagePositionValue.y;
    }
    /// <summary>
    /// 맵 정보(좌표와 크기)의 텍스트를 갱신 시키는 함수
    /// </summary>
    /// <param name="mapPositionValue"></param>
    /// <param name="map"></param>
    public void SetMapPositionText(Vector2 mapPositionValue, Map map)
    {
        mapPositionX.text = mapPositionValue.x.ToString();
        mapPositionY.text = mapPositionValue.y.ToString();
        mapSize.text = "Width :  " + map.width + "\nHeight : " + map.height;
    }

    public void OnClickLoadStageButton()
    {
        loadStageWarnningPopup.SetActive(true);
    }

    public void OnClickLoadCancelButton()
    {
        loadStageWarnningPopup.SetActive(false);
    }

    public void ShowSavePopup()
    {
        saveStageWarnningPopup.SetActive(true);
    }

    public void LoadJsonForAndroid(string Decrjson)
    {
        string jsonData = cryptoMNG.DecrStage(Decrjson);
        StageInfo.data = JsonConvert.DeserializeObject<StageData>(jsonData);

        if (string.IsNullOrEmpty(jsonData))
        {
            errorPopup.SetMessage("맵의 정보가 잘못되어있습니다.");
            return;
        }
        Debug.Log("Load - \n" + jsonData);

        MapManager.Instance.CreateLoadedMap(StageInfo.data);

        mapEditManager.popups.mission.SetLoadedData(StageInfo.data);

        moveText.text = StageInfo.data.moveCount.ToString();
        jumpText.text = StageInfo.data.fenceCount.ToString();

        obstacleOptionPopup.jellyTerm.text = "0";
        obstacleOptionPopup.jellyCount.text = "0";

        bgDropdown.value = StageInfo.data.bgNumber;

        bgmDropdown.value = StageInfo.data.bgmNumber;
        SoundManager.Instance.SetBGM(bgmDropdown.value);

        starGauge.isOn = StageInfo.data.showStarGauge == 1;
        startingMove.isOn = StageInfo.data.isMoveAtStart == 1;
        moveBuff.isOn = StageInfo.data.usePossibleMoveBuff == 1;
        jumpBuff.isOn = StageInfo.data.usePossibleJumpBuff == 1;

        starPercent.text = StageInfo.data.starPercent.ToString();
        stageNumber.text = StageInfo.data.stageNumber.ToString();
        for (int i = 0; i < StageInfo.data.obstacles.Count; ++i)
        {
            if ((StageInfo.data.obstacles[i].type == (int)Enums.OBSTACLE_TYPE.JELLY) && StageInfo.data.obstacles[i].options.Length != 0)
            {
                obstacleOptionPopup.jellyTerm.text = StageInfo.data.obstacles[i].options[0].ToString();
                obstacleOptionPopup.jellyCount.text = StageInfo.data.obstacles[i].options[1].ToString();
            }
        }
        selectModeDropDown.value = 0;
        mapEditManager.OnChangeSelectModeDropDown(0);
    }
}
