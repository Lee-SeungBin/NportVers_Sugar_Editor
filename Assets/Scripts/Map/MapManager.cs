using System;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;
    public static MapManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public bool isCreatingMap = false;

    public Camera mainCamera;

    [Header("- Prefabs")]
    public GameObject tileSet;
    public Map map;
    public List<GameObject> CRCharactors;
    public List<GameObject> STCharactors;
    public List<GameObject> CHCharactors;
    public List<GameObject> EGCharactors;
    public List<GameObject> BRCharactors;
    public List<GameObject> SPCharactors;

    public float scale = 0;
    public const float minSize = 3.6f;
    private const float maxSize = 14.4f;
    public List<Map> Maps { get; private set; }

    public Action onChangeMaps;

    private Enums.MAP_SELECT_MODE selectModeType;

    private bool isTouchScreen;
    private Vector2 prevMousePosition;
    private Vector2 moveMousePosition;

    private MapMoveMode mapMoveMode;
    private SelectTileSetMode selectTileSetMode;
    private MonsterSetMode monsterSetMode;
    [HideInInspector]
    public RailMode railMode;
    [HideInInspector]
    public SpecialMode specialMode;

    private void Awake()
    {
        _instance = this;

        Maps = new List<Map>();

        mapMoveMode = GetComponent<MapMoveMode>();
        selectTileSetMode = GetComponent<SelectTileSetMode>();
        monsterSetMode = GetComponent<MonsterSetMode>();
        railMode = GetComponent<RailMode>();
        specialMode = GetComponent<SpecialMode>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            isTouchScreen = true;
            prevMousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(2))
        {
            if (isTouchScreen)
            {
                moveMousePosition = prevMousePosition - (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mainCamera.gameObject.transform.position += new Vector3(moveMousePosition.x, moveMousePosition.y);
                prevMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else if (Input.GetMouseButtonUp(2))
        {
            mapMoveMode.SetNullSelectMap();
        }
        else
        {
            switch (selectModeType)
            {
                case Enums.MAP_SELECT_MODE.MAP_MOVE:
                    mapMoveMode.TouchControll();
                    break;
                case Enums.MAP_SELECT_MODE.SELECT_TILESET:
                    selectTileSetMode.TouchControll();
                    break;
                case Enums.MAP_SELECT_MODE.MONSTER_SET:
                    monsterSetMode.TouchControll();
                    break;
                case Enums.MAP_SELECT_MODE.RAIL_SET:
                    railMode.TouchControll();
                    break;
                case Enums.MAP_SELECT_MODE.SPECIAL_SET:
                    specialMode.TouchControll();
                    break;
            }
            if (!UIManager.Instance.loadStagePopup.activeSelf &&
                !UIManager.Instance.updateListPopup.activeSelf)
            {
                Vector2 wheelInput2 = Input.mouseScrollDelta;
                if (wheelInput2.y != 0)
                {
                    scale -= (wheelInput2.y * 0.1f);
                    scale = Mathf.Clamp(scale, 0, 1);
                    SetCameraScale();
                }
            }
        }
    }

    private void SetCameraScale()
    {
        mainCamera.orthographicSize = minSize + (maxSize * scale);
        UIManager.Instance.mapEditManager.tileSetVisiblePopup.SetWheelTileSetData(selectTileSetMode.selectTileSet, mainCamera.orthographicSize);
    }

    public void CreateMap(int w, int h)
    {
        isCreatingMap = true;

        Map tempMap = Instantiate(map);
        tempMap.CreateMap(this, w, h, Maps.Count);
        Maps.Add(tempMap);

        MapcountManager.Instance.Init(Maps.Count, Maps.Count);

        CameraSetting(Maps[Maps.Count - 1].transform.position);

        isCreatingMap = false;

        onChangeMaps?.Invoke();
    }

    private void CameraSetting(Vector3 newPos)
    {
        Vector3 cameraPos = newPos;
        cameraPos.z = MapManager.Instance.mainCamera.transform.position.z;

        MapManager.Instance.mainCamera.transform.position = cameraPos;
    }
    public void ChangeSelectMode(int value)
    {
        selectModeType = (Enums.MAP_SELECT_MODE)value;

        selectTileSetMode.SetNullToSelectTileSet();

        mapMoveMode.SetNullSelectMap();

        SetActiveRailGroups(selectModeType == Enums.MAP_SELECT_MODE.RAIL_SET);
    }

    public bool SetTileSetVisible(bool isActive)
    {
        return selectTileSetMode.SetTileSetVisible(isActive);
    }

    public bool SetTileVisible(int index, bool isActive)
    {
        return selectTileSetMode.SetTileVisible(index, isActive);
    }

    public void SetNullToSelectTileSet()
    {
        selectTileSetMode.SetNullToSelectTileSet();
    }

    public void SelectMap(int index)
    {
        mapMoveMode.SelectMap(Maps[index].gameObject);
    }

    public void DeleteMap(int index)
    {
        if (Maps.Count == 0)
        {
            UIManager.Instance.errorPopup.SetMessage("맵이 없습니다.");
            return;
        }

        int mapIndex = index;

        Map map = Maps[mapIndex];
        Destroy(map.gameObject);

        Maps.RemoveAt(mapIndex);

        int len = Maps.Count;
        if (len != 0)
        {
            Vector3 posSort;
            for (int i = 0; i < len; ++i)
            {
                Maps[i].index = i;

                if (i >= mapIndex)
                {
                    posSort = Maps[i].transform.position;
                    posSort.x = i * 50;

                    Maps[i].transform.position = posSort;
                }
            }
            if (mapIndex == len)
                mapIndex -= 1;

            CameraSetting(Maps[mapIndex].transform.position);
        }

        if (Maps.Count > 0)
            MapcountManager.Instance.Init(Maps.Count, mapIndex + 1);
        else
            MapcountManager.Instance.Init(Maps.Count, mapIndex);

        onChangeMaps?.Invoke();
    }

    public void ModifyMap(int index, int w, int h)
    {
        if (Maps.Count > 0)
            Maps[index].ModifyMap(this, w, h, index);
        else
            UIManager.Instance.errorPopup.SetMessage("수정할 맵이 없습니다.");
    }

    public int GetTotalIceCount()
    {
        int count = 0;
        int len = Maps.Count;
        for (int i = 0; i < len; ++i)
        {
            count += Maps[i].GetTotalIceCount();
        }

        return count;
    }

    public void ChangeCharactor(Enums.CHARACTOR_TASTY_TYPE tastyType, int characterIndex)
    {
        List<GameObject> charactors;
        switch (tastyType)
        {
            case Enums.CHARACTOR_TASTY_TYPE.CR:
                charactors = CRCharactors;
                break;
            case Enums.CHARACTOR_TASTY_TYPE.ST:
                charactors = STCharactors;
                break;
            case Enums.CHARACTOR_TASTY_TYPE.CH:
                charactors = CHCharactors;
                break;
            case Enums.CHARACTOR_TASTY_TYPE.EG:
                charactors = EGCharactors;
                break;
            case Enums.CHARACTOR_TASTY_TYPE.BR:
                charactors = BRCharactors;
                break;
            case Enums.CHARACTOR_TASTY_TYPE.SP:
                charactors = SPCharactors;
                break;
            default:
                monsterSetMode.ChangeCharactor(null);
                return;
        }

        if (characterIndex >= 0 && characterIndex < charactors.Count)
        {
            monsterSetMode.ChangeCharactor(charactors[characterIndex]);
        }
    }




    public void DeleteCharacterOnSelectFence()
    {
        monsterSetMode.DeleteCharacterOnSelectFence();
    }

    public void ChangeIceStepOfCharactor(int step)
    {
        monsterSetMode.ChangeIceStepOfCharactor(step);
    }

    public void ChangeStarOfCharactor(bool isStar)
    {
        monsterSetMode.ChangeStarOfCharactor(isStar);
    }
    public void ChangeDirectionOfCharactor(bool isRightDirection)
    {
        monsterSetMode.ChangeDirectionOfCharactor(isRightDirection);
    }
    public void CreateTasteOfBox()
    {
        specialMode.CreateTasteOfBox();
    }
    public void ChangeTasteOfBox(int SelectTier)
    {
        specialMode.ChangeTasteOfBox(SelectTier);
    }
    public void DeleteTasteOfBox()
    {
        specialMode.DeleteTasteOfBox();
    }
    public void DeleteTasteLayerOfBox(int SelectTier)
    {
        specialMode.DeleteTasteLayerOfBox(SelectTier);
    }
    public void DestroyTasteOfBox()
    {
        specialMode.DestroyTasteOfBox();
    }
    public void CreateChurros()
    {
        specialMode.CreateChurros();
    }
    public void DeleteChurros()
    {
        specialMode.DeleteChurros();
    }
    public void ChangeDirectionChurros(bool isRightDirection)
    {
        specialMode.ChangeDirectionChurros(isRightDirection);
    }
    public bool IsAbleToChangeToUserFence()
    {
        int count = 0;
        for (int i = Maps.Count - 1; i > -1; --i)
        {
            count += Maps[i].GetUserFenceCount();
        }

        if (count == 5)
        {
            return false;
        }

        return true;
    }

    public void ChangeUserCharacterState(bool isUser)
    {
        monsterSetMode.ChangeUserCharacterState(isUser);
    }

    public void SetActiveRailGroups(bool isActive)
    {
        for (int i = Maps.Count - 1; i > -1; --i)
        {
            Maps[i].railManager.SetActiveRailGroups(isActive);
        }
    }

    public void CreateLoadedMap(StageData stageData)
    {
        if (Maps.Count > 0)
        {
            for (int j = Maps.Count - 1; j > -1; --j)
            {
                if (Maps[j].railManager.railGroups != null)
                {
                    for (int i = 0; i < Maps[j].railManager.railGroups.Count; i++)
                    {
                        Destroy(Maps[j].railManager.railGroups[i].gameObject);
                    }
                }
                DeleteMap(j);
            }

            Maps = new List<Map>();
        }

        if (Maps.Count > 0)
        {
            for (int j = Maps.Count - 1; j > -1; --j)
            {
                if (Maps[j].boxManager.boxGroups != null)
                {
                    for (int i = 0; i < Maps[j].boxManager.boxGroups.Count; i++)
                    {
                        Destroy(Maps[j].boxManager.boxGroups[i].gameObject);
                    }
                }
                DeleteMap(j);
            }

            Maps = new List<Map>();
        }


        for (int i = 0; i < stageData.mapDatas.Length; ++i)
        {
            Map tempMap = Instantiate(map);
            tempMap.SetLoadMap(this, stageData.mapDatas[i], i);
            Maps.Add(tempMap);

            onChangeMaps?.Invoke();
        }

        MapcountManager.Instance.Init(Maps.Count);

        if (Maps.Count > 0)
        {
            CameraSetting(Maps[0].transform.position);
        }
        else
        {
            CameraSetting(Vector3.zero);
        }
    }

    public GameObject GetCharacter(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            UIManager.Instance.errorPopup.SetMessage("몬스터를 로드하지 못했습니다. 저장된 몬스터가 빈칸 입니다.");

            return null;
        }

        Dictionary<string, List<GameObject>> characterDictionary = new Dictionary<string, List<GameObject>>() // 딕셔너리로 저장
    {
        { "ST", STCharactors },
        { "CH", CHCharactors },
        { "CR", CRCharactors },
        { "EG", EGCharactors },
        { "BR", BRCharactors },
        { "SP", SPCharactors }
    };

        string type = code.Substring(0, 2);

        if (characterDictionary.ContainsKey(type))
        {
            foreach (GameObject character in characterDictionary[type])
            {
                if (character.name == code)
                {
                    return character;
                }
            }
        }

        return null;
    }

    public void SetCameraPosition(Vector3 position)
    {
        mainCamera.gameObject.transform.position = position;
    }

    public void ChangeTheme(int index)
    {
        for (int m = Maps.Count - 1; m > -1; --m)
        {
            for (int t = Maps[m].tileSets.Count - 1; t > -1; --t)
            {
                for (int t2 = Maps[m].tileSets[t].Count - 1; t2 > -1; --t2)
                {
                    Maps[m].tileSets[t][t2].SetTheme();
                }
            }
        }
    }

    public Map currentMap
    {
        get
        {
            return Maps[MapcountManager.Instance.mapCurrentNumber - 1];
        }
    }

    public void HideWoodenFence()
    {
        for (int i = Maps.Count - 1; i > -1; --i)
        {
            Maps[i].HideUserFence();
        }
    }
    /// <summary>
    /// 맵 크기가 딕셔너리 Key값에 있는 경우 Value값을 정하기 위한 함수
    /// </summary>
    public void SetType()
    {
        currentMap.type = UIManager.Instance.mapEditManager.Maptype.value;
        Vector2 newPosition = currentMap.container.transform.localPosition;
        newPosition = currentMap.MotifyCoordinateMap();
        UIManager.Instance.SetMapPositionText(newPosition, currentMap);
    }
}
