using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public MapEditManager mapEditManager;
    public RailEditManager railEditManager;

    public BGDropDown bgDropdown;
    public Dropdown bgmDropdown;

    public Toggle starGauge;
    public Toggle startingMove;
    public Toggle moveBuff;
    public Toggle jumpBuff;

    public InputField starPercent;

    public InputField moveText;
    public InputField jumpText;

    public Button uiOnOffButton;
    private bool isUIOn;

    public GameObject container;

    public Text stageFileName;
    public Text mapCount;

    public GameObject stagePosition;
    public Text stagePositionText;

    public Dropdown selectModeDropDown;

    public GameObject loadStageWarnningPopup;
    public ObstacleOptionPopup obstacleOptionPopup;


    public DragItem dragItem;

    public ErrorMessagePopup errorPopup;

    public GameDataUI gameDataUI;

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

        if(isUIOn)
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
        mapEditManager.OnChangeSelectModeDropDown((MapManager.SELECT_MODE)selectModeDropDown.value);
        railEditManager.OnChangeSelectModeDropDown((MapManager.SELECT_MODE)selectModeDropDown.value);
    }

    public void ShowStagePosition()
    {
        stagePosition.SetActive(true);
    }

    public void HideStagePosition()
    {
        stagePosition.SetActive(false);
    }

    public void SetStagePosition(Vector2 stagePositionValue, Vector2 uiPositionValue)
    {
        stagePosition.transform.position = uiPositionValue;
        stagePositionText.text = "x:" + stagePositionValue.x + "\ny:" + stagePositionValue.y;
    }

    public void OnClickLoadStageButton()
    {
        string input = " 야야야 ひらがな Киа ! @ # $ % ^ & * ' + = ` | ( ) [ ] { } : ; - _ - ＃ ＆ ＆ ＠ § ※ ○ ○ ◎ ◇ ◇ □ □ △ △ ▽ ▽ → ← ← ↑ ↓ ↔ 〓";
        //string regexp = @"/[\{\}\[\]\/?.,;:|\)*~`!^\-_+<>@\#$%&\\\=\(\'\"]";
        //input = Regex.Replace(input, regexp, "", RegexOptions.Singleline);

        string clenaName = new string(input.Select(c => char.IsLetterOrDigit(c) ? c : new char()).ToArray());
        clenaName = clenaName.Replace("\0","");

        string aaaaa = Normalize(input);
        //clenaName = clenaName.Trim();
        loadStageWarnningPopup.SetActive(true);
    }

    private string Normalize(string text)
    {
        return string.Join("",
            from ch in text
            where char.IsLetterOrDigit(ch) || char.IsWhiteSpace(ch)
            select ch);
    }

    public void OnClickLoadStartButton()
    {
        OnClickLoadCancelButton();

        stageFilePath = EditorUtility.OpenFilePanel("Overwrite with json", "", "json");

        if (stageFilePath.Length == 0) return;

        StartCoroutine(LoadJsonForAndroid());
    }

    public void OnClickLoadCancelButton()
    {
        loadStageWarnningPopup.SetActive(false);
    }

    private string stageFilePath;

    IEnumerator LoadJsonForAndroid()
    {
        string[] paths = stageFilePath.Split('/');
        stageFileName.text = paths[paths.Length - 1].ToString().Split('.')[0];

        mapEditManager.jsonFileMakerPopup.fileName.text = stageFileName.text;

        UnityWebRequest reader = UnityWebRequest.Get(stageFilePath);
        reader.SendWebRequest();

        while (!reader.isDone)
        {   
            yield return new WaitForFixedUpdate();
        }

        StageInfo.data = JsonUtility.FromJson<StageData>(reader.downloadHandler.text);

        MapManager.Instance.CreateLoadedMap(StageInfo.data);

        mapEditManager.popups.mission.SetLoadedData(StageInfo.data);

        moveText.text = StageInfo.data.moveCount.ToString();
        jumpText.text = StageInfo.data.fenceCount.ToString();


        bgDropdown.value = StageInfo.data.bgNumber;

        bgmDropdown.value = StageInfo.data.bgmNumber;
        SoundManager.Instance.SetBGM(bgmDropdown.value);

        starGauge.isOn = StageInfo.data.showStarGauge == 1;
        startingMove.isOn = StageInfo.data.isMoveAtStart == 1;
        moveBuff.isOn = StageInfo.data.usePossibleMoveBuff == 1;
        jumpBuff.isOn = StageInfo.data.usePossibleJumpBuff == 1;

        starPercent.text = StageInfo.data.starPercent.ToString();

        for(int i = 0; i < StageInfo.data.obstacles.Count; ++i)
        {
            if(StageInfo.data.obstacles[i].type == (int)Enums.OBSTACLE_TYPE.JELLY)
            {
                obstacleOptionPopup.jellyTerm.text = StageInfo.data.obstacles[i].options[0].ToString();
                obstacleOptionPopup.jellyCount.text = StageInfo.data.obstacles[i].options[1].ToString();
            }
        }
    }

    public void OnClickAllFixStartButton()
    {
        OnClickLoadCancelButton();

        if (stageFilePath_ == "")
        {
            stageFilePath_ = EditorUtility.OpenFilePanel("Overwrite with json", "", "json");

            if (stageFilePath_.Length == 0) return;
        }
        else
        {
            paths_ = stageFilePath_.Split('/');
            current_stage_num = int.Parse(paths_[paths_.Length - 2].Split('_')[1]);
            string current_stage_origin = current_stage_num.ToString();
            string current_stage_new = (current_stage_num + 1).ToString();

            if (current_stage_num == 86)
                return;

            stageFilePath_ = stageFilePath_.Replace("_" + current_stage_origin + "/", "_" + current_stage_new + "/");
            stageFilePath_ = stageFilePath_.Replace("_" + current_stage_origin + "_", "_" + current_stage_new + "_");
        }


        StartCoroutine(LoadAllJsonForAndroid());
    }

    string[] paths_;
    int current_stage_num = 1;
    public string stageFilePath_ = "";

    IEnumerator LoadAllJsonForAndroid()
    {
        yield return new WaitForSeconds(1.0f);

        paths_ = stageFilePath_.Split('/');
        stageFileName.text = paths_[paths_.Length - 1].ToString().Split('.')[0];

        mapEditManager.jsonFileMakerPopup.fileName.text = stageFileName.text;

        UnityWebRequest reader = UnityWebRequest.Get(stageFilePath_);
        reader.SendWebRequest();

        while (!reader.isDone)
        {
            yield return new WaitForFixedUpdate();
        }

        StageInfo.data = JsonUtility.FromJson<StageData>(reader.downloadHandler.text);

        MapManager.Instance.CreateLoadedMap(StageInfo.data);

        mapEditManager.popups.mission.SetLoadedData(StageInfo.data);

        moveText.text = StageInfo.data.moveCount.ToString();
        jumpText.text = StageInfo.data.fenceCount.ToString();


        bgDropdown.value = StageInfo.data.bgNumber;

        bgmDropdown.value = StageInfo.data.bgmNumber;
        SoundManager.Instance.SetBGM(bgmDropdown.value);

        starGauge.isOn = StageInfo.data.showStarGauge == 1;
        startingMove.isOn = StageInfo.data.isMoveAtStart == 1;
        moveBuff.isOn = StageInfo.data.usePossibleMoveBuff == 1;
        jumpBuff.isOn = StageInfo.data.usePossibleJumpBuff == 1;

        starPercent.text = StageInfo.data.starPercent.ToString();

        for (int i = 0; i < StageInfo.data.obstacles.Count; ++i)
        {
            if (StageInfo.data.obstacles[i].type == (int)Enums.OBSTACLE_TYPE.JELLY)
            {
                obstacleOptionPopup.jellyTerm.text = StageInfo.data.obstacles[i].options[0].ToString();
                obstacleOptionPopup.jellyCount.text = StageInfo.data.obstacles[i].options[1].ToString();
            }
        }

        mapEditManager.jsonFileMakerPopup.OnClickSave();
    }


}
