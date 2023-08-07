using System.Collections.Generic;
using UnityEngine;

public class BoxManager : MonoBehaviour
{
    public BoxGroup boxGroupPrefab;

    public List<BoxGroup> boxGroups = new List<BoxGroup>();

    public BoxGroup CreateBoxGroup()
    {
        BoxGroup boxGroup = Instantiate(boxGroupPrefab).GetComponent<BoxGroup>();
        boxGroup.Init(this);
        boxGroups.Add(boxGroup);

        return boxGroup;
    }

    public void DeleteBoxGroup(BoxGroup boxGroup)
    {
        boxGroups.Remove(boxGroup);
        Destroy(boxGroup.gameObject);
    }

    public void SetLoadedData(List<List<TileSet>> tileSets, MapData mapData)
    {
        if (mapData.boxGroupDatas == null)
            return;
        SpecialList specialList = UIManager.Instance.mapEditManager.specialList.GetComponent<SpecialList>();
        SpecialMode specialMode = MapManager.Instance.specialMode;
        specialList.boxtype = 4;
        for (int i = 0; i < mapData.boxGroupDatas.Count; i++)
        {
            BoxGroup boxGroup = Instantiate(boxGroupPrefab).GetComponent<BoxGroup>();
            boxGroup.Init(this);

            boxGroup.Direction = mapData.boxGroupDatas[i].direction;
            boxGroup.startidx = mapData.boxGroupDatas[i].tileIndex[0];
            for (int j = 0; j < mapData.boxGroupDatas[i].fenceIndex.Count; j++)
            {
                int tileIndex = mapData.boxGroupDatas[i].tileIndex[j];
                int fenceIndex = mapData.boxGroupDatas[i].fenceIndex[j];
                Tile currentTile = tileSets[fenceIndex / mapData.height][fenceIndex % mapData.height].tiles[tileIndex];
                specialMode.selectTile = currentTile;
                boxGroup.Push(currentTile, fenceIndex, tileIndex);
                if (boxGroup.Direction == 0)
                {
                    //Tile nextTile = MapManager.Instance.currentMap.GetTileWH(currentTile.tileW - 1, currentTile.tileH);
                    specialMode.SetChurros(currentTile.transform.parent, currentTile, boxGroup.Direction);
                }
                else if (boxGroup.Direction == 1)
                {
                    //Tile nextTile = MapManager.Instance.currentMap.GetTileWH(currentTile.tileW, currentTile.tileH - 1);

                    specialMode.SetChurros(currentTile.transform.parent, currentTile, boxGroup.Direction);
                }


            }

            boxGroups.Add(boxGroup);
        }

    }
}
