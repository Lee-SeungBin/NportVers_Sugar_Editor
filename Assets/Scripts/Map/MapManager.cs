using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    private SELECT_MODE selectModeType;
    public enum SELECT_MODE
    {
        MAP_MOVE,
        SELECT_TILESET,
        MONSTER_SET,
        RAIL_SET,
        SPECIAL_SET
    }

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
                prevMousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else if (Input.GetMouseButtonUp(2))
        {
            mapMoveMode.SetNullSelectMap();
        }
        else
        {
            if (selectModeType == SELECT_MODE.MAP_MOVE)
            {
                mapMoveMode.TouchControll();
            }
            else if (selectModeType == SELECT_MODE.SELECT_TILESET)
            {
                selectTileSetMode.TouchControll();
            }
            else if (selectModeType == SELECT_MODE.MONSTER_SET)
            {
                monsterSetMode.TouchControll();
            }
            else if (selectModeType == SELECT_MODE.RAIL_SET)
            {
                railMode.TouchControll();
            }
            else if (selectModeType == SELECT_MODE.SPECIAL_SET)
            {
                specialMode.TouchControll();
            }
            if (!UIManager.Instance.loadStagePopup.activeSelf)
            {
                Vector2 wheelInput2 = Input.mouseScrollDelta;
                if (wheelInput2.y > 0)
                {
                    // 휠을 밀어 돌렸을 때의 처리 ↑
                    scale -= (wheelInput2.y * 0.1f);
                    if (scale < 0)
                    {
                        scale = 0;
                    }

                    SetCameraScale();
                }
                else if (wheelInput2.y < 0)
                {

                    // 휠을 당겨 올렸을 때의 처리 ↓
                    scale -= (wheelInput2.y * 0.1f);
                    if (scale > 1)
                    {
                        scale = 1;
                    }

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
        Map tempMap = Instantiate(map);
        tempMap.CreateMap(this, w, h, Maps.Count);
        Maps.Add(tempMap);

        MapcountManager.Instance.Init(Maps.Count, Maps.Count);

        CameraSetting(Maps[Maps.Count-1].transform.position);

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
        selectModeType = (SELECT_MODE)value;

        selectTileSetMode.SetNullToSelectTileSet();

        mapMoveMode.SetNullSelectMap();

        SetActiveRailGroups(selectModeType == SELECT_MODE.RAIL_SET);
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
        if (len == 0)
        {

        }
        else
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

        if(Maps.Count > 0)
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

    public void ChangeCharactor(Charactor.TASTY_TYPE tastyType, int characterIndex)
    {
        if (tastyType == Charactor.TASTY_TYPE.NONE)
        {
            monsterSetMode.ChangeCharactor(null);
        }
        else if(tastyType == Charactor.TASTY_TYPE.ST)
        {
            monsterSetMode.ChangeCharactor(STCharactors[characterIndex]);
        }
        else if (tastyType == Charactor.TASTY_TYPE.CH)
        {
            monsterSetMode.ChangeCharactor(CHCharactors[characterIndex]);
        }
        else if (tastyType == Charactor.TASTY_TYPE.CR)
        {
            monsterSetMode.ChangeCharactor(CRCharactors[characterIndex]);
        }
        else if (tastyType == Charactor.TASTY_TYPE.EG)
        {
            monsterSetMode.ChangeCharactor(EGCharactors[characterIndex]);
        }
        else if (tastyType == Charactor.TASTY_TYPE.BR)
        {
            monsterSetMode.ChangeCharactor(BRCharactors[characterIndex]);
        }
        else if (tastyType == Charactor.TASTY_TYPE.SP)
        {
            monsterSetMode.ChangeCharactor(SPCharactors[characterIndex]);
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
    public void CrateTasteOfBox()
    {
        specialMode.CreateTasteOfBox();
    }
    public void ChangeTasteOfBox(int SelectLayer)
    {
        specialMode.ChangeTasteOfBox(SelectLayer);
    }
    public void DeleteTasteOfBox()
    {
        specialMode.DeleteTasteOfBox();
    }
    public void DeleteTasteLayerOfBox(int SelectLayer)
    {
        specialMode.DeleteTasteLayerOfBox(SelectLayer);
    }
    public void DestroyTasteOfBox()
    {
        specialMode.DestroyTasteOfBox();
    }
    public bool IsAbleToChangeToUserFence()
    {
        int count = 0;
        for (int i = Maps.Count - 1; i > -1; --i)
        {
            count += Maps[i].GetUserFenceCount();
        }

        if(count == 5)
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
        for(int i = Maps.Count - 1; i > -1; --i)
        {
            Maps[i].railManager.SetActiveRailGroups(isActive);
        }
    }


    public void CreateLoadedMap(StageData stageData)
    {
        if(Maps.Count > 0)
        {
            for(int j = Maps.Count - 1; j > -1; --j)
            {
                DeleteMap(j);
            }

            Maps = new List<Map>();
        }



        for(int i = 0; i < stageData.mapDatas.Length; ++i)
        {
            Map tempMap = Instantiate(map);
            tempMap.SetLoadMap(this, stageData.mapDatas[i], i);
            //tempMap.transform.localPosition = Vector3.zero;
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

        string type = code.Substring(0, 2);

        if(type == "ST")
        {
            for (int i = 0; i < STCharactors.Count; i++)
            {
                if (STCharactors[i].name == code)
                    return STCharactors[i];
            }
            return null;
        }
        else if (type == "CH")
        {
            for (int i = 0; i < CHCharactors.Count; i++)
            {
                if (CHCharactors[i].name == code)
                    return CHCharactors[i];
            }
            return null;

        }
        else if (type == "CR")
        {
            for (int i = 0; i < CRCharactors.Count; i++)
            {
                if (CRCharactors[i].name == code)
                    return CRCharactors[i];
            }
            return null;

        }
        else if (type == "EG")
        {
            for (int i = 0; i < EGCharactors.Count; i++)
            {
                if (EGCharactors[i].name == code)
                    return EGCharactors[i];
            }
            return null;
        }
        else if (type == "BR")
        {
            for (int i = 0; i < BRCharactors.Count; i++)
            {
                if (BRCharactors[i].name == code)
                    return BRCharactors[i];
            }
            return null;
        }
        else if (type == "SP")
        {
            for (int i = 0; i < SPCharactors.Count; i++)
            {
                if (SPCharactors[i].name == code)
                    return SPCharactors[i];
            }
            return null;
        }
        else
        {
            return null;
        }
    }

    public void SetCameraPosition(Vector3 position)
    {
        mainCamera.gameObject.transform.position = position;
    }

    public void ChangeTheme(int index)
    {
        for(int m = Maps.Count - 1; m > -1; --m)
        {
            for(int t = Maps[m].tileSets.Count - 1; t > -1; --t)
            {
                for(int t2 = Maps[m].tileSets[t].Count - 1; t2 > -1; --t2)
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


    public void OnInputFieldValueChanged(string value)
    {
        if (float.TryParse(value, out float floatValue))
        {
            Vector3 newPosition = currentMap.container.transform.position;
            newPosition.x = floatValue;
            currentMap.container.transform.position = newPosition;
        }

        Debug.Log(value);
    }
}
