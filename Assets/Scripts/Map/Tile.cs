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
            if(GetComponentInParent<TileSet>().character != null)
            {
                if(GetComponentInParent<TileSet>().character.tileIndex == tileIndex)
                {
                    return GetComponentInParent<TileSet>().character;
                }
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
        get
        {
            return _isVisible;
        }

        set
        {
            _isVisible = value;
            if(value)
            {
                GetComponent<SpriteRenderer>().color = Color.white;
            }
            else
            {
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.3f);
            }
        }
    }
}
