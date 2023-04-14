using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailGroup : MonoBehaviour
{
    private RailManager railManager;

    public List<TileSet> tileSets;
    public List<RailMoveNumber> railMoveNumbers;

    private Color color;

    public enum RAIL_TYPE
    {
        ROTATION = 0,
        STRIGHT = 1
    }
    [HideInInspector]
    public RAIL_TYPE railType;

    private void Awake()
    {
        color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 0.8f);
    }


    public void Init(RailManager railManager)
    {

        this.railManager = railManager;

        tileSets = new List<TileSet>();
        railMoveNumbers = new List<RailMoveNumber>();
    }

    public void PushTileSet(TileSet tileSet)
    {
        for(int i = 0; i < tileSets.Count; ++i)
        {
            if(tileSets[i] == tileSet)
            {
                return;
            }
        }

        tileSets.Add(tileSet);
        //tileSet.isVisible = true;
        tileSet.railGroup = this;
        tileSet.SetLineColor(Color.red);
        tileSet.SetActiveStateRailLine(true);


        if (railMoveNumbers.Count > 0)
        {
            railMoveNumbers[railMoveNumbers.Count - 1].Unselect();
        }

        RailMoveNumber railMoveNumber = Instantiate(railManager.railMoveNumberPrefab);
        railMoveNumber.SetNumber(tileSets.Count);
        railMoveNumber.transform.SetParent(tileSet.transform);
        railMoveNumber.transform.position = new Vector2(tileSet.transform.position.x, tileSet.transform.position.y);
        railMoveNumbers.Add(railMoveNumber);
    }

    public void RemoveTileSet(TileSet tileSet)
    {
        for(int i = tileSets.Count - 1; i >-1; --i)
        {
            if(tileSets[i] == tileSet)
            {
                tileSets.RemoveAt(i);
                tileSet.railGroup = null;
                tileSet.SetActiveStateRailLine(false);

                railMoveNumbers.RemoveAt(i);

                break;
            }
        }
    }

    public void SetActiveGroupLine(bool isActive)
    {
        if(isActive)
        {
            for (int i = tileSets.Count - 1; i > -1; --i)
            {
                tileSets[i].SetLineColor(color);
                tileSets[i].SetActiveStateRailLine(true);

                railMoveNumbers[i].gameObject.SetActive(true);
            }
        }
        else
        {
            for (int i = tileSets.Count - 1; i > -1; --i)
            {
                tileSets[i].SetActiveStateRailLine(false);

                railMoveNumbers[i].gameObject.SetActive(false);
            }
        }
    }

    public List<int> GetTileMoveNumbers(TileSet tileSet)
    {
        List<int> moveNumbers = new List<int>();

        for (int i = 0; i < tileSets.Count; ++i)
        {
            if (tileSets[i] == tileSet)
            {
                moveNumbers.Add(i);
            }
        }

        return moveNumbers;
    }

    public void DeleteRailGroup() // 버튼 액션
    {
        for (int i = tileSets.Count - 1; i > -1; --i)
        {
            tileSets[i].railGroup = null;
            tileSets[i].SetActiveStateRailLine(false);
            tileSets.RemoveAt(i);

            Destroy(railMoveNumbers[i].gameObject);
            railMoveNumbers.RemoveAt(i);
        }
        railManager.DeleteRailGroup(this);
    }

    public void SelectGroup()
    {
        for (int i = tileSets.Count - 1; i > -1; --i)
        {
            tileSets[i].SetLineColor(Color.red);
            //tileSets[i].SetActiveStateRailLine(true);
        }

        railMoveNumbers[railMoveNumbers.Count - 1].Select();
        MapManager.Instance.railMode.SelectGroup(tileSets[tileSets.Count - 1]);
    }

    public TileSet GetLastTileSet()
    {
        return tileSets[tileSets.Count - 1];
    }

    public void UnselectGroup()
    {
        for (int i = tileSets.Count - 1; i > -1; --i)
        {
            tileSets[i].SetLineColor(color);
        }

        railMoveNumbers[railMoveNumbers.Count - 1].Unselect();
        MapManager.Instance.railMode.UnselectGroup(tileSets[tileSets.Count - 1]);
    }
}
