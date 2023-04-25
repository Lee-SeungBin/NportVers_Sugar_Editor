using UnityEngine;

public class TileSetVisiblePopup : MonoBehaviour
{
    [SerializeField]
    private TileSetVisibleToggle fence;
    [SerializeField]
    private TileSetVisibleToggle[] tiles;

    public void SetTileSetData(TileSet tileSet)
    {
        fence.isOn = !tileSet.isVisible;

        for (int i = 0; i < 4; ++i)
        {
            tiles[i].isOn = !tileSet.tiles[i].isVisible;
        }

        transform.position = Camera.main.WorldToScreenPoint(tileSet.transform.position);
    }
    public void SetWheelTileSetData(TileSet tileSet, float cameraSize)
    {
        if (this.gameObject.activeSelf)
            transform.position = Camera.main.WorldToScreenPoint(tileSet.transform.position);
        // 팝업의 크기를 조절합니다.
        float popupScale = 3.6f / cameraSize; // 카메라 크기에 따른 팝업 크기 비율 계산
        transform.localScale = new Vector3(popupScale, popupScale, 1f);

        // 타일 이미지 크기는 그대로 유지하면서 팝업 크기를 조절합니다.
        foreach (Transform tileTransform in transform)
        {
            if (tileTransform.name == "Tile")
            {
                foreach (Transform childTransform in tileTransform)
                {
                    childTransform.localScale = new Vector3(popupScale, popupScale, 1f);
                }
            }
        }

    }
    public void OnChangeTileSet()
    {
        if (MapManager.Instance.SetTileSetVisible(fence.isOn))
        {
            fence.isOn = !fence.isOn;
            for (int i = 0; i < 4; ++i)
            {
                tiles[i].isOn = fence.isOn;
            }
        }
    }

    public void OnChangeTile(int index)
    {
        if (MapManager.Instance.SetTileVisible(index, tiles[index].isOn))
        {
            tiles[index].isOn = !tiles[index].isOn;
        }
    }
}
