using UnityEngine;
using UnityEngine.EventSystems;

public class SpecialMode : MonoBehaviour
{
    /* 기존의 오브젝트 풀링 방식 사용 안함(장애물의 종류가 다양해지면 처리해야 하는게 많아지기 때문)
    * 예를 들어 샌드위치 같은 경우 생성하고 비활성화후 다른 박스 타입을 선택하면 레이어도 초기화 해야하고
    * 처리할게 많아지기 때문, 나중에 문제가 생기면 다시 오브젝트 풀링 방식 사용예정
    */
    [SerializeField]
    private Jelly jellyPrefab;

    [SerializeField]
    private FrogSoup frogSoupPrefab;

    [SerializeField]
    private Box boxPrefab;

    [SerializeField]
    private Box box3Prefab;

    [SerializeField]
    private Box box5Prefab;

    [SerializeField]
    private WoodenFence woodenFencePrefab;

    [SerializeField]
    private Vine vinePrefab;

    public Tile selectTile;
    public void TouchControll()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnMouseDownForSetSpecial();
        }
        else if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            OnMouseUpForSetSpecial();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            OnMouseDownDeleteSpecial();
            MapManager.Instance.HideWoodenFence();
        }
    }
    public void SetNullToSelectTile()
    {
        selectTile = null;
    }

    private void OnMouseDownForSetSpecial()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        SetNullToSelectTile();

        UIManager.Instance.mapEditManager.HideSandWichInfoPopup();
        UIManager.Instance.mapEditManager.HideSandWichChangePopup();

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0, 1 << 10);
        if (hit.collider != null)
            selectTile = hit.collider.transform.GetComponent<Tile>();

        switch (UIManager.Instance.dragItem.specialType)
        {
            case Enums.SPECIAL_TYPE.JELLY:
                OnClickJelly();
                break;
            case Enums.SPECIAL_TYPE.FROG_SOUP:
                OnClickFrogSoup();
                break;
            case Enums.SPECIAL_TYPE.BOX:
                OnClickBox();
                break;
            case Enums.SPECIAL_TYPE.WOODEN_FENCE:
                OnClickWoodenFence();
                break;
            case Enums.SPECIAL_TYPE.VINE:
                OnClickVine();
                break;
        }
    }
    private void OnMouseUpForSetSpecial()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0, 1 << 10 | 1 << 0);

        Tile tile = hit.collider?.GetComponent<Tile>();

        if (tile != null && tile == selectTile && selectTile.box != null && selectTile.box.boxTypes == 2)
        {
            UIManager.Instance.mapEditManager.ShowSandWichInfoPopup(selectTile.box);
        }
    }
    private void OnMouseDownDeleteSpecial()
    {
        var specialList = UIManager.Instance.mapEditManager.specialList.GetComponent<SpecialList>();
        UIManager.Instance.mapEditManager.HideSandWichInfoPopup();
        UIManager.Instance.mapEditManager.HideSandWichChangePopup();
        if (UIManager.Instance.dragItem.specialType != Enums.SPECIAL_TYPE.NONE)
        {
            specialList.SelectEmtpy();
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0, 1 << 0 | 1 << 10);
            if (hit.collider != null)
            {
                selectTile = hit.collider.transform.GetComponent<Tile>();
                TileSet selectTileSet = selectTile?.GetComponentInParent<TileSet>();

                if (hit.collider.transform.tag == "WoodenFence")
                {
                    TileSet woodTileSet = hit.collider.transform.gameObject.GetComponentInParent<TileSet>();
                    int index = woodTileSet.GetIndexOfFence(hit.collider.transform.GetComponent<WoodenFence>());
                    if (index == -1) return;
                    RemoveWoodenFence(woodTileSet, index);
                }
                else if (selectTile?.jelly != null)
                {
                    RemoveJelly(selectTile.jelly);
                }
                else if (selectTile?.box != null)
                {
                    RemoveBox(selectTile.box);
                }
                else if (selectTile?.frogSoup != null)
                {
                    RemoveFrogSoup(selectTile.frogSoup);
                }
                else if (selectTileSet?.vine != null)
                {
                    RemoveVine(selectTileSet.vine);
                }
            }
        }
    }
    #region Jelly
    private void OnClickJelly()
    {
        if (selectTile?.jelly != null)
        {
            RemoveJelly(selectTile.jelly);
        }
        else if (selectTile?.isVisible == true)
        {
            SetJelly(selectTile.transform.parent, selectTile);
        }
    }

    public void SetJelly(Transform parent, Tile tile)
    {
        if (tile.box != null) return;

        Jelly jelly = Instantiate(jellyPrefab);

        jelly.transform.SetParent(parent);
        jelly.transform.localScale = Vector3.one;
        jelly.transform.position = tile.transform.position + (Vector3.up * 0.119f);

        jelly.fenceIndex = tile.GetComponentInParent<TileSet>().tileSetIndex;
        jelly.tileIndex = tile.tileIndex;

        jelly.tile = tile;
        tile.jelly = jelly;

        tile.transform.parent.GetComponent<TileSet>().map.SetJelly(jelly);
    }

    public void RemoveJelly(Jelly jelly)
    {
        jelly.tile.jelly = null;
        jelly.tile = null;

        Destroy(jelly.gameObject);

        jelly.GetComponentInParent<Map>().RemoveJelly(jelly);
    }
    #endregion

    #region FrogSoup
    private void OnClickFrogSoup()
    {
        if (selectTile?.frogSoup != null)
        {
            RemoveFrogSoup(selectTile.frogSoup);
        }
        else if (selectTile?.isVisible == true)
        {
            SetFrogSoup(selectTile.transform.parent, selectTile);
        }
    }

    public void SetFrogSoup(Transform parent, Tile tile)
    {
        FrogSoup frogSoup = Instantiate(frogSoupPrefab, parent);
        frogSoup.transform.position = tile.transform.position + Vector3.up * 0.119f;

        frogSoup.fenceIndex = tile.GetComponentInParent<TileSet>().tileSetIndex;
        frogSoup.tileIndex = tile.tileIndex;

        frogSoup.tile = tile;
        tile.frogSoup = frogSoup;

        tile.transform.parent.GetComponent<TileSet>().map.SetFrogSoup(frogSoup);

    }

    public void RemoveFrogSoup(FrogSoup frogSoup)
    {
        frogSoup.tile.frogSoup = null;
        frogSoup.tile = null;

        Destroy(frogSoup.gameObject);

        frogSoup.GetComponentInParent<Map>().RemoveFrogSoup(frogSoup);
    }
    #endregion

    #region Box
    private void OnClickBox()
    {
        if (selectTile?.box != null)
        {
            RemoveBox(selectTile.box);
        }
        else if (selectTile?.isVisible == true)
        {
            SetBox(selectTile.transform.parent, selectTile);
        }
    }

    public void SetBox(Transform parent, Tile tile)
    {
        if (tile.character != null ||
            tile.jelly != null) return;

        Box box;

        var specialList = UIManager.Instance.mapEditManager.specialList.GetComponent<SpecialList>();
        if (specialList.boxlayer == 3 && specialList.boxtype == 0)
        {
            box = Instantiate(box3Prefab, parent);
        }
        else if (specialList.boxlayer == 5 && specialList.boxtype == 0)
        {
            box = Instantiate(box5Prefab, parent);
        }
        else
        {
            box = Instantiate(boxPrefab, parent);
        }
        box.transform.position = tile.transform.position + Vector3.up * 0.119f;
        box.fenceIndex = tile.GetComponentInParent<TileSet>().tileSetIndex;
        box.tileIndex = tile.tileIndex;
        box.boxLayer = specialList.boxlayer;
        box.boxTypes = specialList.boxtype;

        if (box.boxTypes == 2)
        {
            var spriteRenderer = box.transform.GetChild(0).GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = box.boxsprite[5];
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
            UIManager.Instance.mapEditManager.ShowSandWichInfoPopup(box);
            specialList.SelectEmtpy();
        }

        ChangeBox(box, box.boxLayer, box.boxTypes);

        tile.box = box;
        box.tile = tile;
        tile.transform.parent.GetComponent<TileSet>().map.SetBox(box);
    }

    public void RemoveBox(Box box)
    {
        box.tile.box = null;
        box.tile = null;

        box.GetComponentInParent<Map>().RemoveBox(box);

        Destroy(box.gameObject);
    }

    public void ChangeBox(Box box, int layer, int types)
    {
        Sprite[] getboxsprite = UIManager.Instance.mapEditManager.specialList.GetComponent<SpecialList>().specialSprites;
        //if (types == 0)
        //{
        //    switch (layer)
        //    {
        //        case 1:
        //            box.GetComponentInChildren<SpriteRenderer>().sprite = getboxsprite[2];
        //            break;
        //        case 3:
        //            box.GetComponentInChildren<SpriteRenderer>().sprite = getboxsprite[4];
        //            break;
        //        case 5:
        //            box.GetComponentInChildren<SpriteRenderer>().sprite = getboxsprite[5];
        //            break;
        //    }
        //}
        if (types == 1)
        {
            box.GetComponentInChildren<SpriteRenderer>().sprite = getboxsprite[8];
        }
        else if (types == 2)
        {
            LoadTasteOfBox(box);
        }
    }
    public void LoadTasteOfBox(Box box)
    {
        GameObject alphaTier = box.transform.GetChild(0).gameObject;
        alphaTier.GetComponent<SpriteRenderer>().sprite = box.boxsprite[5];
        alphaTier.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);

        for (int i = 0; i < box.boxLayer; i++)
        {
            GameObject Tier = Instantiate(alphaTier);
            Tier.transform.SetParent(box.transform);
            Tier.transform.position = box.transform.position + Vector3.up * (0.135f * (i + 1));
            Tier.GetComponent<SpriteRenderer>().sprite = box.boxsprite[box.boxTier[i]];
            Tier.GetComponent<SpriteRenderer>().color = Color.white;

            alphaTier.transform.position = Tier.transform.position;
            alphaTier.GetComponent<SpriteRenderer>().sortingOrder++;
        }

        alphaTier.transform.position = box.transform.position + Vector3.up * (0.135f * (box.boxLayer + 1));
        ActiveAlphaLayerOfBox(box, false);
    }

    public void CreateTasteOfBox()
    {
        int currentTaste = UIManager.Instance.mapEditManager.sandwichInfoPopup.TasteStepDropdown.value;

        GameObject Tier = Instantiate(selectTile.box.transform.GetChild(0).gameObject, selectTile.box.transform); // 샌드위치 층 생성
        Tier.transform.position = selectTile.box.transform.position + Vector3.up * (0.135f * (selectTile.box.boxLayer + 1));
        Tier.GetComponent<SpriteRenderer>().sprite = selectTile.box.boxsprite[currentTaste];
        Tier.GetComponent<SpriteRenderer>().color = Color.white;

        selectTile.box.boxTier.Add(currentTaste);
        selectTile.box.boxLayer++;

        GameObject alphaTier = selectTile.box.transform.GetChild(0).gameObject;
        alphaTier.transform.position = selectTile.box.transform.position + Vector3.up * (0.135f * (selectTile.box.boxLayer + 1));
        alphaTier.GetComponent<SpriteRenderer>().sortingOrder++;

        UIManager.Instance.mapEditManager.sandwichInfoPopup.SetData(selectTile.box);
    }
    public void ChangeTasteOfBox(int SelectTier)
    {
        int currentTaste = UIManager.Instance.mapEditManager.sandwichChangeDropDown.value;

        selectTile.box.boxTier[SelectTier] = currentTaste;
        selectTile.box.transform.GetChild(SelectTier + 1).GetComponent<SpriteRenderer>().sprite = selectTile.box.boxsprite[currentTaste];
        UIManager.Instance.mapEditManager.sandwichChangePopup.SetData(selectTile.box);
    }

    public void DeleteTasteOfBox()
    {
        if (selectTile.box.boxLayer == 0)
        {
            UIManager.Instance.errorPopup.SetMessage("샌드위치가 없습니다.");
            return;
        }
        else
        {
            if (selectTile.box.boxTier.Count != 0)
                selectTile.box.boxTier.RemoveAt(selectTile.box.boxTier.Count - 1);

            GameObject alphaTier = selectTile.box.transform.GetChild(0).gameObject;
            GameObject deleteTier = selectTile.box.transform.GetChild(selectTile.box.boxLayer).gameObject;

            alphaTier.transform.position = deleteTier.transform.position;
            Destroy(deleteTier);
            alphaTier.GetComponent<SpriteRenderer>().sortingOrder--;
            selectTile.box.boxLayer--;

            UIManager.Instance.mapEditManager.sandwichInfoPopup.SetData(selectTile.box);
        }
    }

    public void DeleteTasteLayerOfBox(int SelectTier)
    {
        if (selectTile.box.boxLayer != 0)
        {
            for (int i = SelectTier + 1; i <= selectTile.box.boxLayer; i++) // 샌드위치 레이어 재구성
            {
                GameObject Tier = selectTile.box.transform.GetChild(i).gameObject;
                GameObject alphaTier = selectTile.box.transform.GetChild(0).gameObject;
                if (i == selectTile.box.boxLayer) // 마지막 레이어일때 맨위만 파괴
                {
                    Destroy(Tier);
                }
                else // 선택된 층부터 위에 층꺼를 하나씩 땡겨옴
                {
                    Tier.GetComponent<SpriteRenderer>().sprite = selectTile.box.transform.GetChild(i + 1).GetComponent<SpriteRenderer>().sprite;
                }
                alphaTier.transform.position = Tier.transform.position; // 파괴된 위치(맨위)에 투명 레이어 이동
                alphaTier.GetComponent<SpriteRenderer>().sortingOrder = Tier.GetComponent<SpriteRenderer>().sortingOrder;
            }

            selectTile.box.boxTier.RemoveAt(SelectTier);
            selectTile.box.boxLayer--;

            UIManager.Instance.mapEditManager.sandwichChangePopup.SetData(selectTile.box);
        }
        else
        {
            UIManager.Instance.errorPopup.SetMessage("샌드위치가 없습니다.");
            return;
        }
    }

    public void ActiveAlphaLayerOfBox(Box box, bool isActive)
    {
        box.transform.GetChild(0).gameObject.SetActive(isActive);
    }

    public void DestroyTasteOfBox()
    {
        RemoveBox(selectTile.box);
        selectTile = null;
    }

    #endregion

    #region WoodenFence
    private void OnClickWoodenFence()
    {
        Ray ray = new Ray(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, 1 << 12);

        if (hit.collider != null)
        {
            GameObject shadowFence = hit.collider.gameObject;
            int index = int.Parse(shadowFence.name.Split('_')[1]);
            TileSet tileSet = shadowFence.transform.parent.parent.GetComponent<TileSet>();

            if (tileSet.woodenFences[index] == null)
            {
                SetWoodenFence(tileSet, index);
            }
            else
            {
                RemoveWoodenFence(tileSet, index);
            }
        }
        else
        {
            TileSet tileSet = selectTile?.GetComponentInParent<TileSet>();

            if (tileSet != null && tileSet.isVisible)
            {
                tileSet.woodenFenceColliders.SetActive(!tileSet.woodenFenceColliders.activeSelf);
            }
        }
    }

    public void SetWoodenFence(TileSet tileSet, int woodenFenceIndex)
    {
        WoodenFence woodenFence;
        woodenFence = Instantiate(woodenFencePrefab, tileSet.transform);

        tileSet.woodenFenceShadows[woodenFenceIndex].image.enabled = false;

        WoodenFence shadow = tileSet.woodenFenceShadows[woodenFenceIndex];

        tileSet.woodenFences[woodenFenceIndex] = woodenFence;

        woodenFence.transform.localScale = shadow.transform.localScale;
        woodenFence.transform.position = shadow.transform.position;
    }

    public void RemoveWoodenFence(TileSet tileSet, int woodenFenceIndex)
    {
        WoodenFence woodenFence = tileSet.woodenFences[woodenFenceIndex];
        tileSet.woodenFences[woodenFenceIndex] = null;
        tileSet.woodenFenceShadows[woodenFenceIndex].image.enabled = true;

        Destroy(woodenFence.gameObject);
    }
    #endregion

    #region Vine
    private void OnClickVine()
    {
        TileSet tileSet = selectTile?.GetComponentInParent<TileSet>();

        if (tileSet != null && !tileSet.isVisible)
        {
            if (tileSet.vine != null)
            {
                RemoveVine(tileSet.vine);
            }
            else
            {
                SetVine(tileSet.transform, tileSet);
            }
        }
        else if (selectTile != null)
        {
            UIManager.Instance.errorPopup.SetMessage("타일이 전부 비어있어야 설치 가능합니다.");
        }
    }

    public void SetVine(Transform parent, TileSet tileSet)
    {
        if (tileSet.vine != null) return;

        Vine vine = Instantiate(vinePrefab, parent);

        vine.transform.localScale = Vector3.one;
        vine.transform.position = tileSet.transform.position + Vector3.up * 0.223f;

        int getvinelayer = UIManager.Instance.mapEditManager.specialList.GetComponent<SpecialList>().vinelayer; // 덩굴 레이어 가져오기

        vine.fenceIndex = tileSet.GetComponentInParent<TileSet>().tileSetIndex;
        vine.layer = getvinelayer;

        vine.tileset = tileSet;
        tileSet.vine = vine;

        tileSet.transform.GetComponent<TileSet>().map.SetVine(vine);
    }

    public void RemoveVine(Vine vine)
    {
        vine.tileset.vine = null;
        vine.tileset = null;

        Destroy(vine.gameObject);

        vine.GetComponentInParent<Map>().RemoveVine(vine);
    }
    #endregion
}
