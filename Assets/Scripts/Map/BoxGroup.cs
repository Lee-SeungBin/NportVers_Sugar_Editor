using System.Collections.Generic;
using UnityEngine;

public class BoxGroup : MonoBehaviour
{
    private BoxManager boxManager;

    public int Direction;
    public List<int> fenceindex;
    public List<int> tileindex;


    public void Init(BoxManager boxManager)
    {
        this.boxManager = boxManager;

        fenceindex = new List<int>();
        tileindex = new List<int>();
    }

    public void Push(Tile tile, int fenceidx, int tileidx)
    {
        tile.boxGroup = this;

        fenceindex.Add(fenceidx);
        tileindex.Add(tileidx);
    }

    public void Pop(Tile tile)
    {
        tile.boxGroup = null;
        if (fenceindex.Count > 0)
        {
            fenceindex.RemoveAt(fenceindex.Count - 1);
        }

        if (tileindex.Count > 0)
        {
            tileindex.RemoveAt(tileindex.Count - 1);
        }
    }

    public int LastFenceIndex()
    {
        return fenceindex[fenceindex.Count - 1];
    }

    public int LastTileIndex()
    {
        return tileindex[tileindex.Count - 1];
    }
}
