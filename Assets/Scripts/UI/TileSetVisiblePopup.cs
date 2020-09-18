using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileSetVisiblePopup : MonoBehaviour
{
    [SerializeField]
    private TileSetVisibleToggle fence;
    [SerializeField]
    private TileSetVisibleToggle[] tiles;


    public void SetTileSetData(TileSet tileSet)
    {
        fence.isOn = !tileSet.gameObject.activeSelf;

        for(int i = 0; i < 4; ++i)
        {
            tiles[i].isOn = !tileSet.tiles[i].gameObject.activeSelf;
        }

        transform.position = Camera.main.WorldToScreenPoint(tileSet.transform.position);
    }

    public void OnChangeTileSet()
    {
        if(MapManager.Instance.SetTileSetVisible(fence.isOn))
        {
            fence.isOn = !fence.isOn;
        }
    }

    public void OnChangeTile(int index)
    {
        if(MapManager.Instance.SetTileVisible(index, tiles[index].isOn))
        {
            tiles[index].isOn = !tiles[index].isOn;
        }
    }
}
