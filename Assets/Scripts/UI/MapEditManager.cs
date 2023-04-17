using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditManager : MonoBehaviour
{
    public GameObject characterList;
    public GameObject specialList;

    public Button createMapButton;
    public GameObject createMapPopup;

    public Button modifyMapButton;
    public Dropdown modifyMapDropDown;
    public GameObject modifyMapPopup;
    public GameObject selectMaptypePopup;
    public Dropdown Maptype;

    public Button stageLoadButton;
    public Button jsonSaveButton;
    //public JsonFileMaker jsonFileMaker;
    public ObstacleOptionPopup obstacleOptionPopup;

    public InputField Width;
    public InputField Height;

    public CharacterInfoPopup characterInfoPopup;

    public TileSetVisiblePopup tileSetVisiblePopup;

    public SandWichInfoPopup sandwichInfoPopup;
    public SandWichChangePopup sandwichChangePopup;
    public Dropdown sandwichChangeDropDown;

    public Toggle centerSelectButton;


    public MapEditorPopups popups;

    public Button obstacleButton;
    public Button versionupdateButton;
    public Button resetButton;

    private void Start()
    {
        HideCreateMapPopup();
        HideCharactorInfoPopup();
        HideModifyMapPopup();
        HideSandWichInfoPopup();
        HideSandWichChangePopup();

        SetVisibleTileSetPopup(false);
        //SetVisibleJsonFileSavePopup(false);
        SetVisibleObstacleOptionPopup(false);


        MapManager.Instance.onChangeMaps += OnChangeModifyMapDropDown;
    }


    public void OnChangeSelectModeDropDown(MapManager.SELECT_MODE selectMode)
    {
        gameObject.SetActive(true);

        specialList.GetComponent<SpecialList>().SelectEmtpy();

        if (selectMode == MapManager.SELECT_MODE.MONSTER_SET)
        {
            characterList.SetActive(true);
            specialList.SetActive(false);
            stageLoadButton.gameObject.SetActive(false);
            jsonSaveButton.gameObject.SetActive(false);
            modifyMapButton.gameObject.SetActive(false);
            createMapButton.gameObject.SetActive(false);
            obstacleButton.gameObject.SetActive(false);
            versionupdateButton.gameObject.SetActive(false);
            resetButton.gameObject.SetActive(false);
        }
        else if (selectMode == MapManager.SELECT_MODE.SPECIAL_SET)
        {
            characterList.SetActive(false);
            specialList.SetActive(true);
            stageLoadButton.gameObject.SetActive(false);
            jsonSaveButton.gameObject.SetActive(false);
            modifyMapButton.gameObject.SetActive(false);
            createMapButton.gameObject.SetActive(false);
            obstacleButton.gameObject.SetActive(false);
            versionupdateButton.gameObject.SetActive(false);
            resetButton.gameObject.SetActive(false);
        }
        else
        {
            characterList.SetActive(false);
            specialList.SetActive(false);
            stageLoadButton.gameObject.SetActive(true);
            jsonSaveButton.gameObject.SetActive(true);
            modifyMapButton.gameObject.SetActive(true);
            createMapButton.gameObject.SetActive(true);
            obstacleButton.gameObject.SetActive(true);
            versionupdateButton.gameObject.SetActive(true);
            resetButton.gameObject.SetActive(true);
        }

        SetVisibleTileSetPopup(false);
        HideCharactorInfoPopup();
        HideSandWichInfoPopup();
        HideWoodenFenceColliders();
        HideSandWichChangePopup();
        characterList.GetComponent<CharacterList>().OnClickRemoveCharacterInList();
    }

    public void HideWoodenFenceColliders()
    {
        MapManager.Instance.HideWoodenFence();
    }
    public void ShowCreateMapPopup()
    {
        createMapPopup.SetActive(true);

        createMapPopup.transform.Find("MapNumber").GetComponent<Text>().text = (MapManager.Instance.Maps.Count + 1).ToString();
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

        if(isActive == false)
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
        else
        {
            modifyMapPopup.transform.Find("Width").GetComponent<InputField>().text = "";
            modifyMapPopup.transform.Find("Height").GetComponent<InputField>().text = "";
        }
    }

    public void DeleteMap()
    {
        MapManager.Instance.DeleteMap(modifyMapDropDown.value);
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
    public void InputCoordinateMap()
    {
        if (MapManager.Instance.Maps.Count > 0 && !MapManager.Instance.isCreatingMap)
        {
            if (float.TryParse(UIManager.Instance.mapPositionX.text, out float floatValueX) && float.TryParse(UIManager.Instance.mapPositionY.text, out float floatValueY))
            {
                Vector2 newPosition = MapManager.Instance.currentMap.container.transform.localPosition;
                newPosition.x = floatValueX;
                newPosition.y = floatValueY;
                MapManager.Instance.currentMap.container.transform.localPosition = newPosition;
                UIManager.Instance.SetMapPositionText(newPosition, MapManager.Instance.currentMap);
            }
        }
        else
        {
            return;
        }
    }

    public void OnClickCenterMap()
    {
        if(MapManager.Instance.Maps.Count > 0)
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
        for(int i = MapManager.Instance.Maps.Count - 1; i >= 0; i--) // 맵 지우기
        {
            MapManager.Instance.DeleteMap(i);
        }
        // 미션 초기화
        UIManager.Instance.stageNumber.text = "0";
        UIManager.Instance.mapEditManager.popups.mission.creamScoreText.text = "0";
        UIManager.Instance.mapEditManager.popups.mission.strawberryScoreText.text = "0";
        UIManager.Instance.mapEditManager.popups.mission.chocolateScoreText.text = "0";
        UIManager.Instance.mapEditManager.popups.mission.eggScoreText.text = "0";
        UIManager.Instance.mapEditManager.popups.mission.breadScoreText.text = "0";
        UIManager.Instance.mapEditManager.popups.mission.jellyText.text = "0";
        UIManager.Instance.mapEditManager.popups.mission.breakIceText.text = "0";
        UIManager.Instance.mapEditManager.popups.mission.frogSoup.isOn = false;

        // 게임 데이터 UI 초기화
        UIManager.Instance.starPercent.text = "0";
        UIManager.Instance.moveBuff.isOn = true;
        UIManager.Instance.jumpBuff.isOn = true;
        UIManager.Instance.starGauge.isOn = true;
        UIManager.Instance.startingMove.isOn = true;
        UIManager.Instance.moveText.text = "0";
        UIManager.Instance.jumpText.text = "0";

        UIManager.Instance.stageType.value = 0;
        UIManager.Instance.bgDropdown.value = 0;
        UIManager.Instance.bgmDropdown.value = 0;

        UIManager.Instance.mapPositionX.text = "0";
        UIManager.Instance.mapPositionY.text = "0";
        UIManager.Instance.mapSize.text = "Width :  0\nHeight : 0";

        // 팝업 초기화
        Width.text = "";
        Height.text = "";
        UIManager.Instance.mapEditManager.obstacleOptionPopup.jellyCount.text = "0";
        UIManager.Instance.mapEditManager.obstacleOptionPopup.jellyTerm.text = "0";
    }
    public void ShowCharactorInfoPopup(Charactor character)
    {
        characterInfoPopup.Show(character);
    }


    public void HideCharactorInfoPopup()
    {
        characterInfoPopup.Hide();
    }

    //public void SetVisibleJsonFileSavePopup(bool isActive)
    //{
    //    jsonFileMakerPopup.gameObject.SetActive(isActive);
    //}
    

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
}
