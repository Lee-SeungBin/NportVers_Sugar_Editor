using UnityEngine;

public class TileSet : MonoBehaviour
{
    [HideInInspector]
    public Map map;

    public int tileSetIndex;

    public Tile[] tiles;

    public Charactor character;

    [SerializeField]
    public LineRenderer line;
    private RailGroup _railGroup;
    [HideInInspector]
    public RailGroup railGroup
    {
        get
        {
            return _railGroup;
        }

        set
        {
            _railGroup = value;

            SetTheme();
        }
    }
    public bool isVisibleTiles { get; private set; }

    public GameObject woodenFenceColliders;
    public WoodenFence[] woodenFenceShadows;
    public WoodenFence[] woodenFences = new WoodenFence[12];
    public Vine vine;

    public Sprite[] normalThemes;
    public Sprite[] myThemes;
    public Sprite[] railTheme;
    public Sprite[] myRailTheme;

    void Awake()
    {
        isVisible = true;
        isVisibleTiles = false;

        woodenFenceColliders.SetActive(false);

        SetActiveStateRailLine(false);
    }


    public void SetVisibleAllTilesForRailMode(bool isActive)
    {
        isVisibleTiles = isActive;
    }

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

            for (int i = 0; i < 4; ++i)
            {
                tiles[i].isVisible = value;
            }
        }
    }

    public void SetActiveStateRailLine(bool isActive)
    {
        line.gameObject.SetActive(isActive);
    }

    public void SetLineColor(Color color)
    {
        line.startColor = color;
        line.endColor = color;
    }

    public bool[] GetWoodenFencesForJsonSaving()
    {
        bool[] values = new bool[12];
        for (int i = 0; i < 12; ++i)
        {
            if (woodenFences[i] != null)
            {
                values[i] = true;
            }
            else
            {
                values[i] = false;
            }
        }
        return values;
    }

    public bool[] GetTileVisibleState()
    {
        bool[] values = new bool[4];
        for (int i = 0; i < 4; ++i)
        {
            values[i] = tiles[i].isVisible;
        }
        return values;
    }

    public bool GetFlavoneBox()
    {
        for (int i = 0; i < 4; i++)
        {
            if (tiles[i]?.box?.boxTypes == 3)
            {
                return tiles[i].box.GetComponent<RandomBox>().isflavone;
            }
        }
        return false;
    }

    public void SetTheme()
    {
        Sprite sp = normalThemes[UIManager.Instance.bgDropdown.value];

        if (railGroup != null)
        {
            sp = railTheme[UIManager.Instance.bgDropdown.value];
            if (character != null && character.isUser)
            {
                sp = myRailTheme[UIManager.Instance.bgDropdown.value];
            }
        }
        else
        {
            if (character != null && character.isUser)
            {
                sp = myThemes[UIManager.Instance.bgDropdown.value];
            }
        }

        for (int i = 0; i < 4; ++i)
        {
            tiles[i].GetComponent<SpriteRenderer>().sprite = sp;
        }
    }
    public int GetIndexOfFence(WoodenFence fence)
    {
        for (int i = 0; i < woodenFences.Length; i++)
        {
            if (woodenFences[i] == fence)
            {
                return i;
            }
        }
        return -1;
    }
}
