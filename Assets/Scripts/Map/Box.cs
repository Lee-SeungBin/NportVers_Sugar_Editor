using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public int fenceIndex;
    public int tileIndex;
    public int boxLayer;
    public int boxTypes;
    public int boxDirection;
    public List<int> boxTier;
    public List<int> boxGroup;
    public Sprite[] boxsprite;

    public Tile tile;
}
