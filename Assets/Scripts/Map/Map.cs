using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    private const float TileSetW = 1.16f;
    private const float TileSetH = 0.66f;


    public int index;

    public GameObject bg;
    public GameObject container;

    public int width;
    public int height;

    public List<List<TileSet>> tileSets;
    [HideInInspector]
    public RailManager railManager;

    public List<Jelly> jellys = new List<Jelly>();
    public List<FrogSoup> frogSoups = new List<FrogSoup>();
    public List<Box> boxs = new List<Box>();


    public List<NextStageData> nextStageDatas;



    private void Awake()
    {
        railManager = GetComponent<RailManager>();
    }

    private void SetBGSize(int width, int height)
    {
        float wAddH = width + height;
        bg.transform.GetComponent<SpriteRenderer>().size = new Vector2(TileSetW * wAddH, TileSetH * wAddH);
        bg.GetComponent<BoxCollider2D>().size = bg.transform.GetComponent<SpriteRenderer>().size;
    }

    public void CreateMap(MapManager mapManager, int width, int height, int index)
    {
        this.width = width;
        this.height = height;
        this.index = index;

        tileSets = new List<List<TileSet>>();

        int w, h;

        TileSet tileSet;

        transform.SetParent(mapManager.transform);

        SetBGSize(width, height);

        int count = 0;
        for (w = 0; w < width; ++w)
        {
            tileSets.Add(new List<TileSet>());

            for (h = 0; h < height; ++h)
            {
                tileSet = Instantiate(mapManager.tileSet).GetComponent<TileSet>();
                tileSet.map = this;
                tileSet.tileSetIndex = count;
                tileSet.transform.SetParent(container.transform);
                tileSet.transform.localPosition = new Vector2((h - w) * TileSetW, ((w + h) * -TileSetH));
                tileSet.SetTheme();
                tileSets[w].Add(tileSet);
                ++count;
            }
        }

        container.transform.localPosition = new Vector2((width - height) * (TileSetW * 0.5f ), (((width + height) * 0.5f - 1.2f) * TileSetH));

        Vector3 newPos = Vector3.zero;

        newPos.x = mapManager.Maps.Count  * 50;

        transform.position = newPos;

        nextStageDatas = new List<NextStageData>();
    }


    public void ModifyMap(MapManager mapManager, int modifyWidth, int modifyHeight, int index)
    {
        int w, h;
        
        this.index = index;

        TileSet tileSet;

        SetBGSize(modifyWidth, modifyHeight);

        SpecialMode specialMode = GetComponent<SpecialMode>();

        if (width > modifyWidth)
        {
            if (tileSets.Count > modifyWidth)
            {
                for(w = tileSets.Count - 1; w > modifyWidth - 1; --w)
                {
                    for (h = tileSets[w].Count - 1; h > -1; --h)
                    {
                        for(int t = 0; t < 4; ++t)
                        {
                            if(tileSets[w][h].tiles[t].jelly != null)
                            {
                                specialMode.RemoveJelly(tileSets[w][h].tiles[t].jelly);
                            }
                            if (tileSets[w][h].tiles[t].frogSoup != null)
                            {
                                specialMode.RemoveFrogSoup(tileSets[w][h].tiles[t].frogSoup);
                            }
                            if (tileSets[w][h].tiles[t].box != null)
                            {
                                specialMode.RemoveBox(tileSets[w][h].tiles[t].box);
                            }
                        }
                        
                        Destroy(tileSets[w][h].gameObject);
                        tileSets[w].RemoveAt(h);
                    }

                    tileSets.RemoveAt(w);
                }
            }
        }
        else if (width < modifyWidth)
        {
            for(w = width; w < modifyWidth; ++w)
            {
                tileSets.Add(new List<TileSet>());

                for (h = 0; h < modifyHeight; ++h)
                {
                    tileSet = Instantiate(mapManager.tileSet).GetComponent< TileSet>();
                    tileSet.transform.SetParent(container.transform);
                    tileSets[w].Add(tileSet);
                }
            }
        }

        if (height > modifyHeight)
        {
            for (w = tileSets.Count - 1; w > -1; --w)
            {
                if(tileSets[w].Count > modifyHeight)
                {
                    for(h = tileSets[w].Count - 1; h > modifyHeight - 1; --h)
                    {
                        Destroy(tileSets[w][h].gameObject);
                        tileSets[w].RemoveAt(h);
                    }
                }
            }
        }
        else if (height < modifyHeight)
        {
            for (w = 0; w < modifyWidth; ++w)
            {
                for(h = tileSets[w].Count; h < modifyHeight; ++h)
                {
                    tileSet = Instantiate(mapManager.tileSet).GetComponent< TileSet>();
                    tileSet.transform.SetParent(container.transform);
                    tileSets[w].Add(tileSet);
                }
            }
        }

        width = modifyWidth;
        height = modifyHeight;

        int count = 0;
        for (w = 0; w < width; ++w)
        {
            for (h = 0; h < height; ++h)
            {
                tileSets[w][h].GetComponent<TileSet>().tileSetIndex = count;
                tileSets[w][h].transform.localPosition = new Vector2((h - w) * TileSetW, ((w + h) * -TileSetH));
                tileSets[w][h].SetTheme();
                ++count;
            }
        }


        container.transform.localPosition = new Vector2((width - height) * (TileSetW * 0.5f), (((width + height) * 0.5f - 1.2f) * TileSetH));


    }


    public void SetLoadMap(MapManager mapManager, MapData data, int mapIndex)
    {
        this.width = data.width;
        this.height = data.height;
        this.index = mapIndex;

        tileSets = new List<List<TileSet>>();

        int w, h;

        TileSet tileSet;

        List<TileSetData> tileSetDatas = data.tileSetDatas;

        transform.SetParent(mapManager.transform);

        SetBGSize(width, height);


        int count = 0;
        for (w = 0; w < width; ++w)
        {
            tileSets.Add(new List<TileSet>());

            for (h = 0; h < height; ++h)
            {
                tileSet = Instantiate(mapManager.tileSet).GetComponent<TileSet>();
                tileSet.map = this;
                tileSet.tileSetIndex = count;
                tileSet.transform.SetParent(container.transform);
                tileSet.transform.localPosition = new Vector2((h - w) * TileSetW, ((w + h) * -TileSetH));
                tileSet.isVisible = tileSetDatas[count].fenceVisibleState == 1;

                bool[] woodenFences = tileSetDatas[count].woodenFences;
                if(woodenFences != null)
                {
                    for (int wf = 0; wf < 12; ++wf)
                    {
                        if(woodenFences[wf])
                        {
                            MapManager.Instance.specialMode.SetWoodenFence(tileSet, wf);
                        }
                    }
                }

                bool[] isVisibleTiles = tileSetDatas[count].isVisibleTiles;
                if (isVisibleTiles != null)
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        tileSet.tiles[i].isVisible = isVisibleTiles[i];
                    }
                }


                if (tileSetDatas[count].code != "x")
                {
                    tileSet.character = Instantiate(mapManager.GetCharacter(tileSetDatas[count].code)).GetComponent<Charactor>();
                    tileSet.character.name = tileSetDatas[count].code;
                    tileSet.character.tileIndex = tileSetDatas[count].position;
                    tileSet.character.transform.SetParent(tileSet.transform);
                    tileSet.character.transform.localPosition = tileSet.tiles[tileSet.character.tileIndex].transform.localPosition;
                    tileSet.character.iceStep = tileSetDatas[count].iceStep;
                    tileSet.character.isStar = tileSetDatas[count].startStar == 1;
                    tileSet.character.isHeightDirection= tileSetDatas[count].heightDirection == 1;
                    tileSet.character.isUser = tileSetDatas[count].userFence == 1;
                }

                tileSet.SetTheme();

                tileSets[w].Add(tileSet);

                ++count;
            }
        }

        //container.transform.localPosition = new Vector2((width - height) * (TileSetW * 0.5f), (((width + height) * 0.5f - 1) * TileSetH));
        container.transform.localPosition = new Vector3(data.centerX, data.centerY, 0);
        //container.transform.localPosition = new Vector2(data.centerX, (((width + height) * 0.5f - 1.21f) * TileSetH));

        transform.localPosition = new Vector2(data.x, data.y);

        railManager.SetLoadedData(tileSets, data);

        Tile tile;
        for(w = 0; w < data.jellyDatas.Count; ++w)
        {
            tile = tileSets[data.jellyDatas[w].fenceIndex / height][data.jellyDatas[w].fenceIndex % height].tiles[data.jellyDatas[w].tileIndex].GetComponent<Tile>();
            MapManager.Instance.specialMode.SetJelly(tile.transform.parent, tile);
        }

        for (w = 0; w < data.frogSoupDatas.Count; ++w)
        {
            tile = tileSets[data.frogSoupDatas[w].fenceIndex / height][data.frogSoupDatas[w].fenceIndex % height].tiles[data.frogSoupDatas[w].tileIndex].GetComponent<Tile>();
            MapManager.Instance.specialMode.SetFrogSoup(tile.transform.parent, tile);
        }

        for (w = 0; w < data.boxDatas.Count; ++w)
        {
            tile = tileSets[data.boxDatas[w].fenceIndex / height][data.boxDatas[w].fenceIndex % height].tiles[data.boxDatas[w].tileIndex].GetComponent<Tile>();
            MapManager.Instance.specialMode.SetBox(tile.transform.parent, tile);
        }


        nextStageDatas = data.nextStageDatas;
    }



    public int GetTotalIceCount()
    {
        int w, h, iceCount = 0;
        for (w = 0; w < width; ++w)
        {
            for (h = 0; h < height; ++h)
            {
                if (tileSets[w][h].GetComponent<TileSet>().character != null)
                {
                    iceCount += tileSets[w][h].GetComponent<TileSet>().character.iceStep;
                }
            }
        }

        return iceCount;
    }

    public int GetUserFenceCount()
    {
        int w, h, count = 0;
        for (w = 0; w < width; ++w)
        {
            for (h = 0; h < height; ++h)
            {
                if (tileSets[w][h].GetComponent<TileSet>().character != null)
                {
                    if (tileSets[w][h].GetComponent<TileSet>().character.isUser)
                        ++count;
                }
            }
        }
        return count;
    }

    public void SetJelly(Jelly jelly)
    {
        jellys.Add(jelly);
    }

    public void RemoveJelly(Jelly jelly)
    {
        jellys.Remove(jelly);
    }

    public void SetFrogSoup(FrogSoup frogSoup)
    {
        frogSoups.Add(frogSoup);
    }

    public void RemoveFrogSoup(FrogSoup frogSoup)
    {
        frogSoups.Remove(frogSoup);
    }
    
    public void SetBox(Box box)
    {
        boxs.Add(box);
    }

    public void RemoveBox(Box box)
    {
        boxs.Remove(box);
    }

    public Tile GetTile(int fenceIndex, int tileIndex)
    {
        int w = fenceIndex / height;
        int h = fenceIndex % height;
        return tileSets[w][h].tiles[tileIndex].GetComponent<Tile>();
    }
}
