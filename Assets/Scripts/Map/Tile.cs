using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int tileIndex;

    public Charactor character
    {
        get
        {
            if(GetComponentInParent<TileSet>().character != null && GetComponentInParent<TileSet>().character.tileIndex == tileIndex)
            {
                return GetComponentInParent<TileSet>().character;
            }

            return null;
        }
    }

    public Jelly jelly;
    public FrogSoup frogSoup;
    public Box box;

    private bool _isVisible;
    public bool isVisible
    {
        get => _isVisible;
        set
        {
            _isVisible = value;
            GetComponent<SpriteRenderer>().color = value ? Color.white : new Color(1, 1, 1, 0.3f);
        }
    }
}
