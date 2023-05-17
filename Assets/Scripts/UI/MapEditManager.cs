using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditManager : MonoBehaviour
{
    public GameObject characterList, specialList;
    public GameObject createMapPopup, modifyMapPopup;
    public Button createMapButton, modifyMapButton, stageLoadButton, jsonSaveButton;
    public Button obstacleButton, versionupdateButton, resetButton;
    public Dropdown modifyMapDropDown, Maptype, sandwichChangeDropDown;
    public InputField Width, Height;
    public ObstacleOptionPopup obstacleOptionPopup;
    public CharacterInfoPopup characterInfoPopup;
    public TileSetVisiblePopup tileSetVisiblePopup;
    public SandWichInfoPopup sandwichInfoPopup;
    public SandWichChangePopup sandwichChangePopup;
    public MapEditorPopups popups;

    private void HidePopups()
    {
        HideCreateMapPopup();
        HideCharactorInfoPopup();
        HideModifyMapPopup();
        HideSandWichInfoPopup();
        HideSandWichChangePopup();
        UIManager.Instance.railEditManager.SetActiveRailSelectPopup(false, null);
    }

    private void Start()
    {
        HidePopups();

        SetVisibleTileSetPopup(false);
        SetVisibleObstacleOptionPopup(false);

        MapManager.Instance.onChangeMaps += OnChangeModifyMapDropDown;
    }
    /// <summary>
    /// 게임 오브젝트를 끄고 키는 함수 (배열 형태)
    /// </summary>
    /// <param name="gameobj"></param>
    /// <param name="active"></param>
    public void ActiveManager(GameObject[] gameobj, bool[] active)
    {
        for (int i = 0; i < gameobj.Length; i++)
        {
            gameobj[i].SetActive(active[i]);
        }
    }
    public void OnChangeSelectModeDropDown(Enums.MAP_SELECT_MODE selectMode)
    {
        gameObject.SetActive(true);

        specialList.GetComponent<SpecialList>().SelectEmtpy();
        // 맵 모드가 바뀔때마다 UI 끄고 켜기
        GameObject[] objArray = {
            characterList, // 0몬스터 1장애물 2로드버튼 3세이브버튼
            specialList,  // 4맵 수정버튼 5맵 생성버튼 6장애물 옵션버튼
            stageLoadButton.gameObject, // 7버전 업데이트 버튼 8리셋 버튼
            jsonSaveButton.gameObject,
            modifyMapButton.gameObject,
            createMapButton.gameObject,
            obstacleButton.gameObject,
            versionupdateButton.gameObject,
            resetButton.gameObject };

        bool[] activeArray = { false, false, true, true, true, true, true, true, true };

        switch (selectMode)
        {
            case Enums.MAP_SELECT_MODE.MONSTER_SET:
                objArray[0].SetActive(true);
                activeArray[0] = true;
                break;
            case Enums.MAP_SELECT_MODE.SPECIAL_SET:
                objArray[1].SetActive(true);
                activeArray[1] = true;
                break;
        }

        ActiveManager(objArray, activeArray);

        SetVisibleTileSetPopup(false);
        HidePopups();
        HideWoodenFenceColliders();
        characterList.GetComponent<CharacterList>().OnClickRemoveCharacterInList();
        MapManager.Instance.railMode.selectTileSet = null;
    }

    public void HideWoodenFenceColliders()
    {
        MapManager.Instance.HideWoodenFence();
    }
    public void ShowCreateMapPopup()
    {
        createMapPopup.SetActive(true);

        createMapPopup.transform.Find("MapNumber").GetComponent<Text>().text = (MapManager.Instance.Maps.Count + 1).ToString();

        Width.text = "";
        Height.text = "";
    }

    public void HideCreateMapPopup()
    {
        createMapPopup.SetActive(false);
    }

    public void OnClickCreateMap()
    {
        if (string.IsNullOrEmpty(Width.text) ||
            string.IsNullOrEmpty(Height.text) ||
            int.Parse(Width.text) == 0 ||
            int.Parse(Height.text) == 0)
        {
            UIManager.Instance.errorPopup.SetMessage("맵의 크기는 1이상 이여야 합니다.");
            return;
        }
        int w = int.Parse(Width.text);
        int h = int.Parse(Height.text);
        MapManager.Instance.CreateMap(w, h);
        HideCreateMapPopup();
    }

    public void SetVisibleTileSetPopup(bool isActive)
    {
        tileSetVisiblePopup.gameObject.SetActive(isActive);

        if (isActive == false)
        {
            MapManager.Instance.SetNullToSelectTileSet();
        }
    }

    public void SetTileSetData(TileSet tileSet)
    {
        tileSetVisiblePopup.SetTileSetData(tileSet);
    }

    public void OnChangeSelectMapNumberOfModifyMapPopup()
    {
        MapManager.Instance.SelectMap(modifyMapDropDown.value);
    }

    public void ShowModifyMapPopup()
    {
        modifyMapPopup.SetActive(true);
        if (MapManager.Instance.Maps.Count > 0)
        {
            modifyMapPopup.transform.Find("Width").GetComponent<InputField>().text = MapManager.Instance.currentMap.width.ToString();
            modifyMapPopup.transform.Find("Height").GetComponent<InputField>().text = MapManager.Instance.currentMap.height.ToString();
        }
    }

    public void DeleteMap()
    {
        MapManager.Instance.DeleteMap(modifyMapDropDown.value);
        modifyMapPopup.transform.Find("Width").GetComponent<InputField>().text = "";
        modifyMapPopup.transform.Find("Height").GetComponent<InputField>().text = "";
    }

    public void HideModifyMapPopup()
    {
        modifyMapPopup.SetActive(false);
    }

    private void OnChangeModifyMapDropDown()
    {
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        Dropdown.OptionData option;

        int len = MapManager.Instance.Maps.Count + 1;
        for (int i = 1; i < len; ++i)
        {
            option = new Dropdown.OptionData();
            option.text = i.ToString();
            options.Add(option);
        }

        modifyMapDropDown.options = options;
    }

    public void OnClickModifyMap()
    {
        Width.text = modifyMapPopup.transform.Find("Width").Find("Text").GetComponent<Text>().text;
        Height.text = modifyMapPopup.transform.Find("Height").Find("Text").GetComponent<Text>().text;

        if (string.IsNullOrEmpty(Width.text) ||
            string.IsNullOrEmpty(Height.text) ||
            int.Parse(Width.text) == 0 ||
            int.Parse(Height.text) == 0)
        {
            UIManager.Instance.errorPopup.SetMessage("맵의 크기는 1이상 이여야 합니다.");
            return;
        }
        int w = int.Parse(Width.text);
        int h = int.Parse(Height.text);

        MapManager.Instance.ModifyMap(modifyMapDropDown.value, w, h);
        HideModifyMapPopup();
    }
    /// <summary>
    /// Input Field의 값의 따라 맵의 좌표를 변경 하는 함수
    /// </summary>
    public void InputCoordinateMap()
    {
        if (MapManager.Instance.Maps.Count == 0 || MapManager.Instance.isCreatingMap)
        {
            return;
        }

        if (float.TryParse(UIManager.Instance.mapPositionX.text, out float floatValueX) && float.TryParse(UIManager.Instance.mapPositionY.text, out float floatValueY))
        {
            Vector2 newPosition = MapManager.Instance.currentMap.container.transform.localPosition;
            newPosition.x = floatValueX;
            newPosition.y = floatValueY;
            MapManager.Instance.currentMap.container.transform.localPosition = newPosition;
            UIManager.Instance.SetMapPositionText(newPosition, MapManager.Instance.currentMap);
        }
    }

    public void OnClickCenterMap()
    {
        if (MapManager.Instance.Maps.Count > 0)
        {
            Vector2 newPosition = MapManager.Instance.currentMap.container.transform.localPosition;
            newPosition = MapManager.Instance.currentMap.MotifyCoordinateMap();
            UIManager.Instance.SetMapPositionText(newPosition, MapManager.Instance.currentMap);
        }
        else
        {
            UIManager.Instance.errorPopup.SetMessage("맵이 없습니다.");
        }
    }

    public void OnClickReset() // 모두 초기화
    {
        for (int i = MapManager.Instance.Maps.Count - 1; i >= 0; i--) // 맵 지우기
        {
            MapManager.Instance.DeleteMap(i);
        }
        // 미션 초기화
        var missionPopup = UIManager.Instance.mapEditManager.popups.mission;
        missionPopup.creamScoreText.text = "0";
        missionPopup.strawberryScoreText.text = "0";
        missionPopup.chocolateScoreText.text = "0";
        missionPopup.eggScoreText.text = "0";
        missionPopup.breadScoreText.text = "0";
        missionPopup.jellyText.text = "0";
        missionPopup.breakIceText.text = "0";
        missionPopup.frogSoup.isOn = false;

        // 게임 데이터 UI 초기화
        var gameDataUI = UIManager.Instance;
        gameDataUI.starPercent.text = "0";
        gameDataUI.moveBuff.isOn = true;
        gameDataUI.jumpBuff.isOn = true;
        gameDataUI.starGauge.isOn = true;
        gameDataUI.startingMove.isOn = true;
        gameDataUI.moveText.text = "0";
        gameDataUI.jumpText.text = "0";
        gameDataUI.stageNumber.text = "0";
        gameDataUI.stageType.value = 0;
        gameDataUI.currentChapter.value = 0;
        gameDataUI.bgDropdown.value = 0;
        gameDataUI.bgmDropdown.value = 0;
        gameDataUI.mapPositionX.text = "0";
        gameDataUI.mapPositionY.text = "0";
        gameDataUI.mapSize.text = "Width :  0\nHeight : 0";
        Maptype.ClearOptions();
        Maptype.GetComponentInChildren<Text>().text = "";

        // 팝업 초기화
        obstacleOptionPopup.jellyCount.text = "0";
        obstacleOptionPopup.jellyTerm.text = "0";
        UIManager.Instance.selectModeDropDown.value = 0;
        OnChangeSelectModeDropDown(0);

        Width.text = "";
        Height.text = "";
    }
    public void ShowCharactorInfoPopup(Charactor character)
    {
        characterInfoPopup.Show(character);
    }

    public void HideCharactorInfoPopup()
    {
        characterInfoPopup.Hide();
    }

    public void SetVisibleObstacleOptionPopup(bool isActive)
    {
        obstacleOptionPopup.gameObject.SetActive(isActive);
    }

    public void ShowSandWichInfoPopup(Box box)
    {
        sandwichInfoPopup.Show(box);
    }

    public void HideSandWichInfoPopup()
    {
        sandwichInfoPopup.Hide();
    }

    public void ShowSandWichChangePopup(Box box)
    {
        sandwichChangePopup.Show(box);
    }

    public void HideSandWichChangePopup()
    {
        sandwichChangePopup.Hide();
    }
    public void OnClickSaveButton()
    {
        if (!MapDataMNG.iSDev)
        {
            popups.transform.Find("StageSaveWarnningPopup").Find("DevText").GetComponent<Text>().text = "현재 본 서버 입니다.";
        }
        else
        {
            popups.transform.Find("StageSaveWarnningPopup").Find("DevText").GetComponent<Text>().text = "현재 테스트 서버 입니다.";
        }
    }

    public void CheckSaveMapType()
    {
        UIManager.Instance.currentChapter.gameObject.SetActive(UIManager.Instance.stageType.captionText.text == "mushroom");
    }
}
