using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using UnityEngine.UI;
using System;

public class JsonFileMaker : MonoBehaviour
{
    public InputField fileName;

    public MapManager mapManager;
    public UIManager uIManager;

    public Dropdown bgDropdown;
    public Dropdown bgmDropdown;

    public Toggle starGauge;
    public Toggle moveBuff;
    public Toggle fenceBuff;
    public Toggle startingMove;

    public InputField starPercent;

    public MissionPopup missionPopup;
    public InputField moveText;
    public InputField jumpText;

    public Text stageFileName;

    public void OnClickSave()
    {
        StageData data = new StageData();

        try
        {
            data.missions = missionPopup.GetUsingMissionDatas().ToArray();

            data.obstacles = new List<Obstacle>();
            
            data.bgNumber = bgDropdown.value;
            data.bgmNumber = bgmDropdown.value;
            data.moveCount = int.Parse(moveText.text);
            data.fenceCount = int.Parse(jumpText.text);
            data.showStarGauge = starGauge.isOn ? 1 : 0;
            data.starPercent = int.Parse(starPercent.text);
            data.isMoveAtStart = startingMove.isOn ? 1 : 0;
            data.usePossibleMoveBuff = moveBuff.isOn ? 1 : 0;
            data.usePossibleJumpBuff = fenceBuff.isOn ? 1 : 0;
        }
        catch (Exception)
        {
            UIManager.Instance.errorPopup.SetMessage("스테이지 정보를 확인해주세요.");
        }



        List<Map> maps = mapManager.Maps;
        int mapCount = maps.Count;

        MapData[] mapDatas = new MapData[mapCount];
        MapData mapData;

        TileSetData tileSet;
        List<TileSetData> tileSets;

        RailGroupData railGroupData;
        List<RailGroupData> railGroupDatas;
        List<RailGroup> railGroups;

       

        Charactor charactor;
        int tileCount;
        int i, w, h, r, ri;

        bool isRail = false;
        bool isJelly = false;
        bool isFrogSoup = false;
        bool isBox = false;

        for (i = 0; i < mapCount; ++i)
        {
            mapData = new MapData();
            mapData.x = maps[i].transform.position.x;
            mapData.y = maps[i].transform.position.y;
            mapData.centerX = maps[i].container.transform.localPosition.x;
            mapData.centerY = maps[i].container.transform.localPosition.y;
            mapData.width = maps[i].width;
            mapData.height = maps[i].height;

            tileCount = 0;
            tileSets = new List<TileSetData>();
            for (w = 0; w < maps[i].width; ++w)
            {
                for (h = 0; h < maps[i].height; ++h)
                {
                    charactor = maps[i].tileSets[w][h].character;

                    tileSet = new TileSetData();
                    tileSet.fenceVisibleState = maps[i].tileSets[w][h].isVisible == true ? 1 : 0;
                    if(charactor != null)
                    {
                        tileSet.code = charactor.name;//.Replace("0","100");
                        tileSet.startStar = charactor.isStar ? 1 : 0;
                        tileSet.position = charactor.tileIndex;
                        tileSet.iceStep = charactor.iceStep;
                        tileSet.heightDirection = charactor.isHeightDirection ? 1 : 0;
                        tileSet.userFence = charactor.isUser ? 1 : 0;
                    }
                    else
                    {
                        tileSet.code = "x";
                        tileSet.startStar = 0;
                        tileSet.position = 0;
                        tileSet.iceStep = 0;
                        tileSet.heightDirection = 0;
                        tileSet.userFence = 0;
                    }

                    tileSet.woodenFences = maps[i].tileSets[w][h].GetWoodenFencesForJsonSaving();
                    tileSet.isVisibleTiles = maps[i].tileSets[w][h].GetTileVisibleState();
                    tileSets.Add(tileSet);

                    ++tileCount;
                }
            }
            mapData.tileSetDatas = tileSets;


            railGroupDatas = new List<RailGroupData>();
            railGroups = maps[i].railManager.railGroups;
            List<int> rails;

            int visibleFenceCount;

            try
            {
                for (r = 0; r < railGroups.Count; ++r)
                {
                    visibleFenceCount = 0;

                    railGroupData = new RailGroupData();
                    railGroupData.straightMode = railGroups[r].railType == RailGroup.RAIL_TYPE.STRIGHT ? 1 : 0;

                    rails = new List<int>();
                    for (ri = 0; ri < railGroups[r].tileSets.Count; ++ri)
                    {
                        if(railGroups[r].tileSets[ri].isVisible)
                        {
                            ++visibleFenceCount;
                        }

                        rails.Add(railGroups[r].tileSets[ri].tileSetIndex);
                    }

                    if(railGroupData.straightMode == 1)
                    {
                        if(rails.Count == visibleFenceCount)
                        {
                            throw new Exception("레일 그룹 번호 " + r + "의 모든 울타리가 보이는 상태입니다. 레일 그룹안의 울타리 중 1개 이상을 없애주세요.");
                        }
                    }

                    railGroupData.rails = rails;

                    railGroupDatas.Add(railGroupData);
                }
            }
            catch(Exception e)
            {
                UIManager.Instance.errorPopup.SetMessage(e.Message);
            }
            mapData.railGroupDatas = railGroupDatas;
            if (mapData.railGroupDatas.Count > 0)
            {
                isRail = true;
            }

            mapData.jellyDatas = GetJellyDatas(maps[i]);
            if (mapData.jellyDatas.Count > 0)
            {
                isJelly = true;
            }

            mapData.frogSoupDatas = GetFrogSoupDatas(maps[i]);
            if(mapData.frogSoupDatas.Count > 0)
            {
                isFrogSoup = true;
            }
            
            mapData.boxDatas = GetBoxData(maps[i]);
            if (mapData.boxDatas.Count > 0)
            {
                isBox = true;
            }

            mapData.nextStageDatas = maps[i].nextStageDatas;


            mapDatas[i] = mapData;
        }

        int jellyTerm = int.Parse(uIManager.mapEditManager.obstacleOptionPopup.jellyTerm.text);
        int jellyCount = int.Parse(uIManager.mapEditManager.obstacleOptionPopup.jellyCount.text);

        if ( jellyTerm > 0 && jellyCount > 0 )
        {
            data.obstacles.Add(new Obstacle
            {
                type = (int)Enums.OBSTACLE_TYPE.JELLY,
                options = new int[] { jellyTerm, jellyCount }
            });
        }

        if (isRail)
        {
            data.obstacles.Add(new Obstacle
            {
                type = (int)Enums.OBSTACLE_TYPE.RAIL,
                options = new int[]{ }
            });
        }

        if(isFrogSoup)
        {
            data.obstacles.Add(new Obstacle
            {
                type = (int)Enums.OBSTACLE_TYPE.FROG_SOUP,
                options = new int[] { }
            });
        }

        if(isBox)
        {
            data.obstacles.Add(new Obstacle
            {
                type = (int)Enums.OBSTACLE_TYPE.BOX,
                options = new int[] { }
            });
        }
        
        data.mapDatas = mapDatas;

        string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(data);
        
        File.WriteAllText(Application.dataPath + "/" + fileName.text + ".json", jsonData.ToString());
        //File.WriteAllText(uIManager.stageFilePath_, jsonData.ToString());

        stageFileName.text = fileName.text;

        gameObject.SetActive(false);

        //uIManager.OnClickAllFixStartButton();
    }

    private List<JellyData> GetJellyDatas(Map map)
    {
        JellyData jellyData;
        List<JellyData> jellyDatas = new List<JellyData>();

        List<Jelly> jellysOfMap = map.jellys;

        int jellyCount = jellysOfMap.Count;

        for(int i = 0; i < jellyCount; ++i)
        {
            jellyData = new JellyData();
            jellyData.fenceIndex = jellysOfMap[i].fenceIndex;
            jellyData.tileIndex = jellysOfMap[i].tileIndex;
            jellyDatas.Add(jellyData);
        }

        return jellyDatas;
    }

    private List<FrogSoupData> GetFrogSoupDatas(Map map)
    {
        FrogSoupData frogSoupData;
        List<FrogSoupData> frogSoupDatas = new List<FrogSoupData>();

        List<FrogSoup> frogSoupOfMap = map.frogSoups;

        int count = frogSoupOfMap.Count;

        for (int i = 0; i < count; ++i)
        {
            frogSoupData = new FrogSoupData();
            frogSoupData.fenceIndex = frogSoupOfMap[i].fenceIndex;
            frogSoupData.tileIndex = frogSoupOfMap[i].tileIndex;
            frogSoupDatas.Add(frogSoupData);
        }

        return frogSoupDatas;
    }
    
    private List<BoxData> GetBoxData(Map map)
    {
        BoxData boxData;
        List<BoxData> boxDatas = new List<BoxData>();

        List<Box> boxOfMap = map.boxs;

        int count = boxOfMap.Count;

        for (int i = 0; i < count; ++i)
        {
            boxData = new BoxData();
            boxData.fenceIndex = boxOfMap[i].fenceIndex;
            boxData.tileIndex = boxOfMap[i].tileIndex;
            boxDatas.Add(boxData);
        }

        return boxDatas;
    }

    public void OnClickCancel()
    {
        gameObject.SetActive(false);
    }

}

[Serializable]
public static class StageInfo
{
    public static StageData data;
}

[Serializable]
public class StageData
{
    public Mission[] missions;
    public List<Obstacle> obstacles;

    public int bgNumber;
    public int bgmNumber;

    public int moveCount;
    public int fenceCount;

    public int showStarGauge;
    public float starPercent;
    public int isMoveAtStart;

    public int usePossibleMoveBuff;
    public int usePossibleJumpBuff;

    public MapData[] mapDatas;
}


[Serializable]
public class Mission
{
     public int type;
     public int qty;
}

[Serializable]
public class Obstacle
{
    public int type;
    public int[] options;
}


[Serializable]
public class MapData
{
    public float x;
    public float y;
    public int width;
    public int height;
    public float centerX;
    public float centerY;
    public List<TileSetData> tileSetDatas;
    public List<RailGroupData> railGroupDatas;
    public List<JellyData> jellyDatas;
    public List<FrogSoupData> frogSoupDatas;
    public List<BoxData> boxDatas;
    public List<NextStageData> nextStageDatas;
}

[Serializable]
public class TileSetData
{
    public int fenceVisibleState;
    public string code;
    public int startStar;
    public int position;
    public int iceStep;
    public int heightDirection;
    public int userFence;
    public bool[] woodenFences;
    public bool[] isVisibleTiles;
}

[Serializable]
public class RailGroupData
{
    public int straightMode;
    public List<int> rails;
}

[Serializable]
public class JellyData
{
    public int fenceIndex;
    public int tileIndex;
}
[Serializable]
public class FrogSoupData
{
    public int fenceIndex;
    public int tileIndex;
}
[Serializable]
public class BoxData
{
    public int fenceIndex;
    public int tileIndex;
}
[Serializable]
public class NextStageData
{
    public int type;
    public int qty;
}