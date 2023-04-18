using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailManager : MonoBehaviour
{
    public RailGroup railGroupPrefab;

    public List<RailGroup> railGroups;

    public RailMoveNumber railMoveNumberPrefab;

    private void Awake()
    {
        railGroups = new List<RailGroup>();
    }

    public RailGroup CreateRailGroup()
    {
        RailGroup railGroup = Instantiate(railGroupPrefab).GetComponent<RailGroup>();
        railGroup.Init(this);
        railGroups.Add(railGroup);

        return railGroup;
    }

    public void DeleteRailGroup(RailGroup railGroup)
    {
        railGroups.Remove(railGroup);
        Destroy(railGroup.gameObject);
    }

    public void SetActiveRailGroups(bool isActive)
    {
        int i;

        if(isActive)
        {
            for(i = railGroups.Count - 1; i > -1; --i)
            {
                railGroups[i].SetActiveGroupLine(true);
            }
        }
        else
        {
            for (i = railGroups.Count - 1; i > -1; --i)
            {
                railGroups[i].UnselectGroup();
            }
        }
    }


    public void SetLoadedData(List<List<TileSet>> tileSets, MapData mapData)
    {
        for(int i = 0; i < mapData.railGroupDatas.Count; ++i)
        {
            RailGroup railGroup = Instantiate(railGroupPrefab).GetComponent<RailGroup>();
            railGroup.Init(this);

            railGroup.railType = (Enums.RAIL_TYPE)int.Parse(mapData.railGroupDatas[i].straightMode);

            for(int j = 0; j < mapData.railGroupDatas[i].rails.Count; ++j)
            {
                int fenceIndex = mapData.railGroupDatas[i].rails[j];
                railGroup.PushTileSet(tileSets[fenceIndex / mapData.height][fenceIndex % mapData.height]);
            }

            railGroup.SetActiveGroupLine(true);

            railGroups.Add(railGroup);

        }
    }
}
