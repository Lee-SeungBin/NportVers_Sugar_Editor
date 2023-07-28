using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    private const float TileSetW = 1.16f;
    private const float TileSetH = 0.66f;


    public int index;
    public int type;
    public GameObject bg;
    public GameObject container;

    public int width;
    public int height;

    public List<List<TileSet>> tileSets;
    public List<List<Tile>> Tiles { get; protected set; }
    public RailManager railManager;
    public BoxManager boxManager;

    public List<Jelly> jellys = new List<Jelly>();
    public List<FrogSoup> frogSoups = new List<FrogSoup>();
    public List<Box> boxs = new List<Box>();
    public List<Vine> vines = new List<Vine>();

    public List<NextStageData> nextStageDatas;

    private Dictionary<string, List<Vector2>> mapCoords = new Dictionary<string, List<Vector2>>() // 맵 정보 딕셔너리 리스트
{
    { "3x3", new List<Vector2>() { new Vector2(0f, 1.9f) } },
    { "4x3", new List<Vector2>() { new Vector2(0.58f, 1.4f) } },
    { "4x4", new List<Vector2>() { new Vector2(0f, 1.9f) } },
    { "4x5", new List<Vector2>() { new Vector2(-0.58f, 2f) } },
    { "4x7", new List<Vector2>() { new Vector2(-1.8f, 2.5f) } },
    { "5x5", new List<Vector2>() { new Vector2(0f, 2.2f), new Vector2(0f, 2.4f) } },
    { "5x6", new List<Vector2>() { new Vector2(0f, 2.2f), new Vector2(-0.58f, 2.6f), new Vector2(-1f, 2.85f), new Vector2(-1f, 2.2f) } },
    { "5x7", new List<Vector2>() { new Vector2(-1.1f, 2.9f) } },
    { "6x4", new List<Vector2>() { new Vector2(1.16f, 2.2f) } },
    { "6x5", new List<Vector2>() { new Vector2(0.58f, 2.6f) } },
    { "6x6", new List<Vector2>() { new Vector2(0f, 2.9f), new Vector2(0f, 3.2f), new Vector2(0f, 3.5f) } },
    { "6x7", new List<Vector2>() { new Vector2(-0.6f, 3.3f) } },
    { "7x4", new List<Vector2>() { new Vector2(1.74f, 3f) } },
    { "7x5", new List<Vector2>() { new Vector2(2f, 3.5f), new Vector2(1.15f, 2.88f) } },
    { "7x6", new List<Vector2>() { new Vector2(1f, 2.9f), new Vector2(0.6f, 3.2f), new Vector2(0f, 3.3f) } },
    { "7x7", new List<Vector2>() { new Vector2(0f, 3.5f), new Vector2(0f, 3.6f) } },
    { "7x8", new List<Vector2>() { new Vector2(-0.55f, 3.5f) } },
    { "8x6", new List<Vector2>() { new Vector2(1.1f, 3.5f) } },
    { "8x7", new List<Vector2>() { new Vector2(0.6f, 3.8f) } },
    { "8x8", new List<Vector2>() { new Vector2(0f, 4.2f) } },
    { "9x9", new List<Vector2>() { new Vector2(0f, 4.8f) } }
    };

    private void Awake()
    {
        railManager = GetComponent<RailManager>();
        boxManager = GetComponent<BoxManager>();
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
        Tiles = new List<List<Tile>>();
        int tileWIndex, tileWIndex2, tileIndex;
        int w, h;

        TileSet tileSet;

        transform.SetParent(mapManager.transform);

        SetBGSize(width, height);

        int count = 0;
        for (w = 0; w < width; ++w)
        {
            tileSets.Add(new List<TileSet>());

            Tiles.Add(new List<Tile>());
            Tiles.Add(new List<Tile>());

            tileWIndex = w * 2;
            tileWIndex2 = tileWIndex + 1;
            tileIndex = 0;
            for (h = 0; h < height; ++h)
            {
                tileSet = Instantiate(mapManager.tileSet).GetComponent<TileSet>();
                tileSet.map = this;
                tileSet.tileSetIndex = count;
                tileSet.transform.SetParent(container.transform);
                tileSet.transform.localPosition = new Vector2((h - w) * TileSetW, ((w + h) * -TileSetH));
                tileSet.SetTheme();

                tileSets[w].Add(tileSet);

                if (tileSet.tiles[1] != null)
                {
                    Tiles[tileWIndex].Add(tileSet.tiles[1]);
                    tileSet.tiles[1].SetTileText(tileWIndex, tileIndex);
                }

                if (tileSet.tiles[0] != null)
                {
                    Tiles[tileWIndex2].Add(tileSet.tiles[0]);
                    tileSet.tiles[0].SetTileText(tileWIndex2, tileIndex);
                }

                ++tileIndex;

                if (tileSet.tiles[2] != null)
                {
                    Tiles[tileWIndex].Add(tileSet.tiles[2]);
                    tileSet.tiles[2].SetTileText(tileWIndex, tileIndex);
                }

                if (tileSet.tiles[3] != null)
                {
                    Tiles[tileWIndex2].Add(tileSet.tiles[3]);
                    tileSet.tiles[3].SetTileText(tileWIndex2, tileIndex);
                }

                ++tileIndex;
                ++count;
            }
        }

        container.transform.localPosition = MotifyCoordinateMap();

        Vector3 newPos = Vector3.zero;

        newPos.x = mapManager.Maps.Count * 50;

        transform.position = newPos;

        nextStageDatas = new List<NextStageData>();

        UIManager.Instance.SetMapPositionText(container.transform.localPosition, this);
    }

    public void ModifyMap(MapManager mapManager, int modifyWidth, int modifyHeight, int index)
    {
        int w, h;

        this.index = index;

        TileSet tileSet;

        SetBGSize(modifyWidth, modifyHeight);

        SpecialMode specialMode = MapManager.Instance.specialMode;
        if (width > modifyWidth)
        {
            for (w = tileSets.Count - 1; w > modifyWidth - 1; --w)
            {
                if (tileSets.Count > modifyWidth)
                {
                    for (h = tileSets[w].Count - 1; h > -1; --h)
                    {
                        for (int t = 0; t < 4; ++t)
                        {
                            if (tileSets[w][h].tiles[t].jelly != null)
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
                        if (tileSets[w][h].vine != null)
                        {
                            specialMode.RemoveVine(tileSets[w][h].vine);
                        }
                        if (tileSets[w][h].railGroup != null)
                        {
                            tileSets[w][h].railGroup.DeleteRailGroup();
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
            for (w = width; w < modifyWidth; ++w)
            {
                tileSets.Add(new List<TileSet>());

                for (h = 0; h < modifyHeight; ++h)
                {
                    tileSet = Instantiate(mapManager.tileSet).GetComponent<TileSet>();
                    tileSet.map = this;
                    tileSet.transform.SetParent(container.transform);
                    tileSets[w].Add(tileSet);
                }
            }
        }

        if (height > modifyHeight)
        {
            for (w = tileSets.Count - 1; w > -1; --w)
            {
                if (tileSets[w].Count > modifyHeight)
                {
                    for (h = tileSets[w].Count - 1; h > modifyHeight - 1; --h)
                    {
                        if (tileSets[w][h].vine != null)
                        {
                            specialMode.RemoveVine(tileSets[w][h].vine);
                        }
                        if (tileSets[w][h].railGroup != null)
                        {
                            tileSets[w][h].railGroup.DeleteRailGroup();
                        }
                        for (int t = 0; t < 4; ++t)
                        {
                            if (tileSets[w][h].tiles[t].jelly != null)
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
                }
            }
        }
        else if (height < modifyHeight)
        {
            for (w = 0; w < modifyWidth; ++w)
            {
                for (h = tileSets[w].Count; h < modifyHeight; ++h)
                {
                    tileSet = Instantiate(mapManager.tileSet).GetComponent<TileSet>();
                    tileSet.map = this;
                    tileSet.transform.SetParent(container.transform);
                    tileSets[w].Add(tileSet);
                }
            }
        }

        width = modifyWidth;
        height = modifyHeight;
        int tileWIndex, tileWIndex2, tileIndex;
        Tiles.Clear();

        int count = 0;
        for (w = 0; w < width; ++w)
        {
            Tiles.Add(new List<Tile>());
            Tiles.Add(new List<Tile>());

            tileWIndex = w * 2;
            tileWIndex2 = tileWIndex + 1;
            tileIndex = 0;

            for (h = 0; h < height; ++h)
            {
                tileSets[w][h].GetComponent<TileSet>().tileSetIndex = count;
                tileSets[w][h].transform.localPosition = new Vector2((h - w) * TileSetW, ((w + h) * -TileSetH));
                tileSets[w][h].SetTheme();

                tileSet = tileSets[w][h];

                if (tileSet.tiles[1] != null)
                {
                    Tiles[tileWIndex].Add(tileSet.tiles[1]);
                    tileSet.tiles[1].SetTileText(tileWIndex, tileIndex);
                }

                if (tileSet.tiles[0] != null)
                {
                    Tiles[tileWIndex2].Add(tileSet.tiles[0]);
                    tileSet.tiles[0].SetTileText(tileWIndex2, tileIndex);
                }

                ++tileIndex;

                if (tileSet.tiles[2] != null)
                {
                    Tiles[tileWIndex].Add(tileSet.tiles[2]);
                    tileSet.tiles[2].SetTileText(tileWIndex, tileIndex);
                }

                if (tileSet.tiles[3] != null)
                {
                    Tiles[tileWIndex2].Add(tileSet.tiles[3]);
                    tileSet.tiles[3].SetTileText(tileWIndex2, tileIndex);
                }

                ++tileIndex;
                ++count;
            }
        }
        UIManager.Instance.mapEditManager.Maptype.value = 0;
        this.type = 0;
        container.transform.localPosition = MotifyCoordinateMap();
        UIManager.Instance.SetMapPositionText(container.transform.localPosition, this);
    }
    /// <summary>
    /// 맵 크기에 따라 중앙 좌표를 정해줌
    /// </summary>
    /// <returns> Vector2 중앙 좌표 </returns>
    public Vector2 MotifyCoordinateMap()
    {
        string mapsize = width.ToString() + "x" + height.ToString();
        if (mapCoords.ContainsKey(mapsize)) // Key값인지 검색
        {
            List<Vector2> coords = mapCoords[mapsize];
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            Dropdown.OptionData option;
            // Value가 여러개인 경우 드롭다운 옵션 설정
            int len = coords.Count + 1;
            for (int i = 1; i < len; ++i)
            {
                option = new Dropdown.OptionData();
                string coordString = coords[i - 1].ToString();

                string[] coordArray = coordString.Trim('(', ')', ' ').Split(',');

                string xLabel = "X: ";
                string yLabel = "Y: ";

                string formattedString = xLabel + coordArray[0] + ", " + yLabel + coordArray[1];
                // 보기 이쁘게 문자열 잘라줌
                option.text = formattedString;
                options.Add(option);
            }

            UIManager.Instance.mapEditManager.Maptype.options = options;
            return coords[type];
        }
        else // 딕셔너리에 있는 맵 크기가 아닐경우
        {
            UIManager.Instance.mapEditManager.Maptype.options.Clear();
            UIManager.Instance.mapEditManager.Maptype.GetComponentInChildren<Text>().text = "";
            return new Vector2((width - height) * (TileSetW * 0.5f), (((width + height) * 0.5f - 1.2f) * TileSetH));
        }
    }
    public void SetLoadMap(MapManager mapManager, MapData data, int mapIndex)
    {
        SpecialList specialList = UIManager.Instance.mapEditManager.specialList.GetComponent<SpecialList>();
        specialList.boxtype = 0;
        this.width = data.width;
        this.height = data.height;
        this.index = mapIndex;

        tileSets = new List<List<TileSet>>();
        Tiles = new List<List<Tile>>();
        int tileWIndex, tileWIndex2, tileIndex;
        int w, h;

        TileSet tileSet;

        List<TileSetData> tileSetDatas = data.tileSetDatas;
        transform.SetParent(mapManager.transform);

        SetBGSize(width, height);


        int count = 0;
        for (w = 0; w < width; ++w)
        {
            tileSets.Add(new List<TileSet>());
            Tiles.Add(new List<Tile>());
            Tiles.Add(new List<Tile>());
            tileWIndex = w * 2;
            tileWIndex2 = tileWIndex + 1;
            tileIndex = 0;
            for (h = 0; h < height; ++h)
            {
                tileSet = Instantiate(mapManager.tileSet).GetComponent<TileSet>();
                tileSet.map = this;
                tileSet.tileSetIndex = count;
                tileSet.transform.SetParent(container.transform);
                tileSet.transform.localPosition = new Vector2((h - w) * TileSetW, ((w + h) * -TileSetH));
                tileSet.isVisible = tileSetDatas[count].fenceVisibleState == 1;

                bool[] woodenFences = tileSetDatas[count].woodenFences;
                if (woodenFences != null)
                {
                    for (int wf = 0; wf < 12; ++wf)
                    {
                        if (woodenFences[wf])
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


                if (tileSetDatas[count].code != "x" && !string.IsNullOrEmpty(tileSetDatas[count].code))
                {
                    switch (tileSetDatas[count].code) // 예전 맵은 스페셜 몬스터 코드가 달랐음
                    {
                        case "SP0001":
                            tileSetDatas[count].code = "SP1001";
                            break;
                        case "SP0002":
                            tileSetDatas[count].code = "SP1002";
                            break;
                        case "SP0003":
                            tileSetDatas[count].code = "SP1003";
                            break;
                        case "SP0004":
                            tileSetDatas[count].code = "SP1004";
                            break;
                        default:
                            break;
                    }
                    tileSet.character = Instantiate(mapManager.GetCharacter(tileSetDatas[count].code)).GetComponent<Charactor>();
                    tileSet.character.name = tileSetDatas[count].code;
                    tileSet.character.tileIndex = tileSetDatas[count].position;
                    tileSet.character.transform.SetParent(tileSet.transform);
                    tileSet.character.transform.localPosition = tileSet.tiles[tileSet.character.tileIndex].transform.localPosition;
                    tileSet.character.iceStep = tileSetDatas[count].iceStep;
                    tileSet.character.isStar = tileSetDatas[count].startStar == 1;
                    tileSet.character.isHeightDirection = tileSetDatas[count].heightDirection == "1";
                    tileSet.character.isUser = tileSetDatas[count].userFence == 1;
                }

                tileSet.SetTheme();

                tileSets[w].Add(tileSet);

                if (tileSet.tiles[1] != null)
                {
                    Tiles[tileWIndex].Add(tileSet.tiles[1]);
                    tileSet.tiles[1].SetTileText(tileWIndex, tileIndex);
                }

                if (tileSet.tiles[0] != null)
                {
                    Tiles[tileWIndex2].Add(tileSet.tiles[0]);
                    tileSet.tiles[0].SetTileText(tileWIndex2, tileIndex);
                }

                ++tileIndex;

                if (tileSet.tiles[2] != null)
                {
                    Tiles[tileWIndex].Add(tileSet.tiles[2]);
                    tileSet.tiles[2].SetTileText(tileWIndex, tileIndex);
                }

                if (tileSet.tiles[3] != null)
                {
                    Tiles[tileWIndex2].Add(tileSet.tiles[3]);
                    tileSet.tiles[3].SetTileText(tileWIndex2, tileIndex);
                }

                ++tileIndex;
                ++count;
            }
        }


        container.transform.localPosition = new Vector3(data.centerX, data.centerY, 0);
        UIManager.Instance.SetMapPositionText(container.transform.localPosition, this);
        transform.localPosition = MotifyCoordinateMap();
        transform.localPosition = new Vector2(data.x, data.y);

        railManager.SetLoadedData(tileSets, data);
        boxManager.SetLoadedData(tileSets, data);
        MapManager.Instance.specialMode.SetMagicHat(tileSets, data);
        Tile tile;
        for (w = 0; w < data.jellyDatas.Count; ++w)
        {
            tile = tileSets[data.jellyDatas[w].fenceIndex / height][data.jellyDatas[w].fenceIndex % height].tiles[int.Parse(data.jellyDatas[w].tileIndex)].GetComponent<Tile>();
            MapManager.Instance.specialMode.SetJelly(tile.transform.parent, tile);
        }

        for (w = 0; w < data.frogSoupDatas.Count; ++w)
        {
            tile = tileSets[data.frogSoupDatas[w].fenceIndex / height][data.frogSoupDatas[w].fenceIndex % height].tiles[int.Parse(data.frogSoupDatas[w].tileIndex)].GetComponent<Tile>();
            MapManager.Instance.specialMode.SetFrogSoup(tile.transform.parent, tile);
        }

        for (w = 0; w < data.boxDatas.Count; ++w)
        {
            tile = tileSets[int.Parse(data.boxDatas[w].fenceIndex) / height][int.Parse(data.boxDatas[w].fenceIndex) % height].tiles[int.Parse(data.boxDatas[w].tileIndex)].GetComponent<Tile>();
            specialList.boxlayer = int.Parse(data.boxDatas[w].boxLayer);
            if (data.boxDatas[w].boxTypes != null)
                specialList.boxtype = int.Parse(data.boxDatas[w].boxTypes);
            if (specialList.boxtype == 4 || specialList.boxtype == 3)
                continue;
            else
                MapManager.Instance.specialMode.SetBox(tile.transform.parent, tile);
            //if (data.boxDatas[w].boxTypes != null)
            //    tile.box.boxTypes = int.Parse(data.boxDatas[w].boxTypes); // 박스 타입 불러오기
            //if (data.boxDatas[w].boxLayer != null)
            //    tile.box.boxLayer = int.Parse(data.boxDatas[w].boxLayer); // 박스 레이어 불러오기
            //if (data.boxDatas[w].boxTier != null)
            //tile.box.boxTier = data.boxDatas[w].boxTier.ConvertAll(i => int.Parse(i)); // 샌드위치 층 불러오기
            //if (specialList.boxtype != 4)
            //    MapManager.Instance.specialMode.ChangeBox(tile.box, tile.box.boxLayer, tile.box.boxTypes);
        }

        for (w = 0; w < data.vineDatas.Count; ++w)
        {
            tileSet = tileSets[int.Parse(data.vineDatas[w].fenceIndex) / height][int.Parse(data.vineDatas[w].fenceIndex) % height].GetComponent<TileSet>();
            MapManager.Instance.specialMode.SetVine(tileSet.transform, tileSet);

            tileSet.vine.layer = int.Parse(data.vineDatas[w].layer);
        }

        nextStageDatas = data.nextStageDatas;
    }

    public bool CheckMapSize(int w, int h)
    {
        return (w >= 0 && w < width * 2 && h >= 0 && h < height * 2) && GetTileWH(w, h).isVisible;
    }

    public Tile GetTileWH(int w, int h)
    {
        if (w < 0 || w >= width * 2 || h < 0 || h >= height * 2)
        {
            return null;
        }
        return Tiles[w][h];
    }

    public TileSet GetTileSet(int fenceindex)
    {
        int w = fenceindex / height;
        int h = fenceindex % height;

        if (w < 0 || w >= width || h < 0 || h >= height)
        {
            return null;
        }

        return tileSets[w][h];
    }

    public TileSet GetTileSet(int w, int h)
    {
        if (w < 0 || w >= width || h < 0 || h >= height)
        {
            return null;
        }
        return tileSets[w][h];
    }

    public bool IsExistWoodenFence(int fromTileW, int fromTileH, int toTileW, int toTileH, bool isEmptyMoveActive = false)
    {
        if (fromTileW < 0 || fromTileW >= Tiles.Count || fromTileH < 0 || fromTileH >= Tiles[fromTileW].Count
            || toTileW < 0 || toTileW >= Tiles.Count || toTileH < 0 || toTileH >= Tiles[toTileW].Count)
        {
            return true;
        }

        Tile fromTile = Tiles[fromTileW][fromTileH];
        Tile toTile = Tiles[toTileW][toTileH];

        if (fromTile == null && toTile == null)
        {
            return false;
        }

        TileSet fromTileSet = null;
        int fromTileIndex = 0;
        TileSet toTileSet = null;
        int toTileIndex = 0;

        int VectorW = toTileW - fromTileW;
        int VectorH = toTileH - fromTileH;

        if (fromTile != null)
        {
            fromTileSet = fromTile.tileSet;
            fromTileIndex = fromTile.tileIndex;
        }

        if (toTile != null)
        {
            toTileSet = toTile.tileSet;
            toTileIndex = toTile.tileIndex;
        }

        return IsExistWoodenFence(fromTileSet, fromTileIndex, toTileSet, toTileIndex, VectorW, VectorH, isEmptyMoveActive);
    }

    public bool IsExistWoodenFence(TileSet fromTileSet, int fromTileIndex, TileSet toTileSet, int toTileIndex, int VectorW = 0, int VectorH = 0, bool isEmptyMoveActive = false)
    {
        bool isExist = false;

        if (isEmptyMoveActive)
        {
            if (fromTileSet == null && toTileSet == null)
                return isExist;

            if (fromTileSet != null)
            {

                if (fromTileIndex == 0)
                {
                    if (VectorH < 0)
                    {
                        isExist = (fromTileSet?.woodenFences[5] != null) || (toTileSet?.woodenFences[10] != null);
                    }
                    else if (VectorH > 0)
                    {
                        isExist = fromTileSet?.woodenFences[3] != null;
                    }

                    if (VectorW < 0)
                    {
                        isExist = fromTileSet?.woodenFences[0] != null;
                    }
                    else if (VectorW > 0)
                    {
                        isExist = (fromTileSet?.woodenFences[4] != null) || (toTileSet?.woodenFences[7] != null);
                    }
                }
                else if (fromTileIndex == 1)
                {
                    if (VectorH < 0)
                    {
                        isExist = (fromTileSet?.woodenFences[6] != null) || (toTileSet?.woodenFences[9] != null);
                    }
                    else if (VectorH > 0)
                    {
                        isExist = fromTileSet?.woodenFences[1] != null;
                    }

                    if (VectorW < 0)
                    {
                        isExist = (fromTileSet?.woodenFences[7] != null) || (toTileSet?.woodenFences[4] != null);
                    }
                    else if (VectorW > 0)
                    {
                        isExist = fromTileSet?.woodenFences[0] != null;
                    }
                }
                else if (fromTileIndex == 2)
                {
                    if (VectorH < 0)
                    {
                        isExist = fromTileSet?.woodenFences[1] != null;
                    }
                    else if (VectorH > 0)
                    {
                        isExist = (fromTileSet?.woodenFences[9] != null) || (toTileSet?.woodenFences[6] != null);
                    }

                    if (VectorW < 0)
                    {
                        isExist = (fromTileSet?.woodenFences[8] != null) || (toTileSet?.woodenFences[11] != null);
                    }
                    else if (VectorW > 0)
                    {
                        isExist = fromTileSet?.woodenFences[2] != null;
                    }
                }
                else
                {
                    if (VectorH < 0)
                    {
                        isExist = fromTileSet?.woodenFences[3] != null;
                    }
                    else if (VectorH > 0)
                    {
                        isExist = (fromTileSet?.woodenFences[10] != null) || (toTileSet?.woodenFences[5] != null);
                    }

                    if (VectorW < 0)
                    {
                        isExist = fromTileSet?.woodenFences[2] != null;
                    }
                    else if (VectorW > 0)
                    {
                        isExist = (fromTileSet?.woodenFences[11] != null) || (toTileSet?.woodenFences[8] != null);
                    }
                }
            }
            else //fromTileSet == null && toTileSet != null
            {
                if (toTileIndex == 0)
                {
                    if (VectorH > 0) // 왼쪽이 빈 타일
                    {
                        isExist = toTileSet?.woodenFences[5] != null;
                    }
                    else if (VectorH < 0) // 오른쪽이 빈 타일
                    {
                        isExist = toTileSet?.woodenFences[3] != null;
                    }

                    if (VectorW < 0) // 아래쪽이 빈 타일
                    {
                        isExist = toTileSet?.woodenFences[4] != null;
                    }
                    else if (VectorW > 0) // 위쪽이 빈 타일
                    {
                        isExist = toTileSet?.woodenFences[0] != null;
                    }
                }
                else if (toTileIndex == 1)
                {
                    if (VectorH > 0)
                    {
                        isExist = toTileSet?.woodenFences[6] != null;
                    }
                    else if (VectorH < 0)
                    {
                        isExist = toTileSet?.woodenFences[1] != null;
                    }

                    if (VectorW > 0)
                    {
                        isExist = toTileSet?.woodenFences[7] != null;
                    }
                    else if (VectorW < 0)
                    {
                        isExist = toTileSet?.woodenFences[0] != null;
                    }
                }
                else if (toTileIndex == 2)
                {
                    if (VectorH < 0)
                    {
                        isExist = toTileSet?.woodenFences[9] != null;
                    }
                    else if (VectorH > 0)
                    {
                        isExist = toTileSet?.woodenFences[1] != null;
                    }

                    if (VectorW > 0)
                    {
                        isExist = toTileSet?.woodenFences[8] != null;
                    }
                    else if (VectorW < 0)
                    {
                        isExist = toTileSet?.woodenFences[2] != null;
                    }
                }
                else if (toTileIndex == 3)
                {
                    if (VectorH < 0)
                    {
                        isExist = toTileSet?.woodenFences[10] != null;
                    }
                    else if (VectorH > 0)
                    {
                        isExist = toTileSet?.woodenFences[3] != null;
                    }

                    if (VectorW < 0)
                    {
                        isExist = toTileSet?.woodenFences[11] != null;
                    }
                    else if (VectorW > 0)
                    {
                        isExist = toTileSet?.woodenFences[2] != null;
                    }
                }
            }
        }
        else
        {
            if (fromTileIndex == 0)
            {
                if (fromTileSet == toTileSet)
                {
                    if (toTileIndex == 1)
                    {
                        isExist = fromTileSet.woodenFences[0] != null;
                    }
                    else if (toTileIndex == 3)
                    {
                        isExist = fromTileSet.woodenFences[3] != null;
                    }
                }
                else
                {
                    if (toTileIndex == 3)
                    {
                        isExist = (fromTileSet != null && fromTileSet.woodenFences[5] != null)
                            || (toTileSet != null && toTileSet.woodenFences[10] != null);
                    }
                    else if (toTileIndex == 1)
                    {
                        isExist = (fromTileSet != null && fromTileSet.woodenFences[4] != null)
                            || (toTileSet != null && toTileSet.woodenFences[7] != null);
                    }
                }
            }
            else if (fromTileIndex == 1)
            {
                if (fromTileSet == toTileSet)
                {
                    if (toTileIndex == 2)
                    {
                        isExist = fromTileSet.woodenFences[1] != null;
                    }
                    else if (toTileIndex == 0)
                    {
                        isExist = fromTileSet.woodenFences[0] != null;
                    }
                }
                else
                {
                    if (toTileIndex == 2)
                    {
                        isExist = (fromTileSet != null && fromTileSet.woodenFences[6] != null)
                            || (toTileSet != null && toTileSet.woodenFences[9] != null);
                    }
                    else if (toTileIndex == 0)
                    {
                        isExist = (fromTileSet != null && fromTileSet.woodenFences[7] != null)
                            || (toTileSet != null && toTileSet.woodenFences[4] != null);
                    }
                }
            }
            else if (fromTileIndex == 2)
            {
                if (fromTileSet == toTileSet)
                {
                    if (toTileIndex == 1)
                    {
                        isExist = fromTileSet.woodenFences[1] != null;
                    }
                    else if (toTileIndex == 3)
                    {
                        isExist = fromTileSet.woodenFences[2] != null;
                    }
                }
                else
                {
                    if (toTileIndex == 3)
                    {
                        isExist = (fromTileSet != null && fromTileSet.woodenFences[8] != null)
                            || (toTileSet != null && toTileSet.woodenFences[11] != null);
                    }
                    else if (toTileIndex == 1)
                    {
                        isExist = (fromTileSet != null && fromTileSet.woodenFences[9] != null)
                            || (toTileSet != null && toTileSet.woodenFences[6] != null);
                    }
                }
            }
            else
            {
                if (fromTileSet == toTileSet)
                {
                    if (toTileIndex == 0)
                    {
                        isExist = fromTileSet.woodenFences[3] != null;
                    }
                    else if (toTileIndex == 2)
                    {
                        isExist = fromTileSet.woodenFences[2] != null;
                    }
                }
                else
                {

                    if (toTileIndex == 0)
                    {
                        isExist = (fromTileSet != null && fromTileSet.woodenFences[10] != null)
                            || (toTileSet != null && toTileSet.woodenFences[5] != null);
                    }
                    else if (toTileIndex == 2)
                    {
                        isExist = (fromTileSet?.woodenFences[11] != null) || (toTileSet?.woodenFences[8] != null);
                    }

                }
            }
        }

        return isExist;
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

    public void HideUserFence()
    {
        for (int w = 0; w < width; ++w)
        {
            for (int h = 0; h < height; ++h)
            {
                tileSets[w][h].GetComponent<TileSet>().woodenFenceColliders.SetActive(false);
            }
        }
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
    public void SetVine(Vine vine)
    {
        vines.Add(vine);
    }

    public void RemoveVine(Vine vine)
    {
        vines.Remove(vine);
    }

    public Tile GetTile(int fenceIndex, int tileIndex)
    {
        int w = fenceIndex / height;
        int h = fenceIndex % height;
        return tileSets[w][h].tiles[tileIndex].GetComponent<Tile>();
    }
}
