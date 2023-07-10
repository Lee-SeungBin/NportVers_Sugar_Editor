using UnityEngine;

public class Tile : MonoBehaviour
{
    public int tileIndex;
    public int tileW;
    public int tileH;

    private BoxGroup _boxGroup;
    [HideInInspector]
    public BoxGroup boxGroup
    {
        get
        {
            return _boxGroup;
        }

        set
        {
            _boxGroup = value;
        }
    }

    public Charactor character
    {
        get
        {
            if (GetComponentInParent<TileSet>().character != null && GetComponentInParent<TileSet>().character.tileIndex == tileIndex)
            {
                return GetComponentInParent<TileSet>().character;
            }

            return null;
        }
    }

    public int fenceIndex
    {
        get
        {
            return GetComponentInParent<TileSet>().tileSetIndex;
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

    public void SetTileText(int w, int h)
    {
        tileW = w;
        tileH = h;
    }
}
