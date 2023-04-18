using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpecialMode : MonoBehaviour
{
    [SerializeField]
    private Jelly jellyPrefab;
    //private List<Jelly> activeJellys = new List<Jelly>();
    //private List<Jelly> inactiveJellys = new List<Jelly>();

    [SerializeField]
    private FrogSoup frogSoupPrefab;
    //private List<FrogSoup> activeFrogSoups = new List<FrogSoup>();
    //private List<FrogSoup> inactiveFrogSoups = new List<FrogSoup>();

    [SerializeField]
    private Box boxPrefab;
    //private List<Box> activeBoxs = new List<Box>();
    //private List<Box> inactiveBoxs = new List<Box>();

    [SerializeField]
    private WoodenFence woodenFencePrefab;
    private List<WoodenFence> activeWoodenFences = new List<WoodenFence>();
    private List<WoodenFence> inactiveWoodenFences = new List<WoodenFence>();

    [SerializeField]
    private Vine vinePrefab;
    //private List<Vine> activeVines = new List<Vine>();
    //private List<Vine> inactiveVines = new List<Vine>();

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
        else if (Input.GetMouseButton(1))
        {
            var specialList = UIManager.Instance.mapEditManager.specialList.GetComponent<SpecialList>();
            specialList.SelectEmtpy();
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
            case SpecialList.SPECIAL_TYPE.JELLY:
                OnClickJelly();
                break;
            case SpecialList.SPECIAL_TYPE.FROG_SOUP:
                OnClickFrogSoup();
                break;
            case SpecialList.SPECIAL_TYPE.BOX:
                OnClickBox();
                break;
            case SpecialList.SPECIAL_TYPE.WOODEN_FENCE:
                OnClickWoodenFence();
                break;
            case SpecialList.SPECIAL_TYPE.VINE:
                OnClickVine();
                break;
        }
    }
    private void OnMouseUpForSetSpecial()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0, 1 << 10);

        foreach (RaycastHit2D hit in hits)
        {
            Tile tile = hit.collider?.GetComponent<Tile>();
            if (tile != null && tile == selectTile && selectTile.box != null && selectTile.box.boxTypes == 2)
            {
                UIManager.Instance.mapEditManager.ShowSandWichInfoPopup(selectTile.box);
                break;
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
        jelly.transform.position = tile.transform.position;

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
        if (selectTile == null)
            return;

        if (selectTile.frogSoup != null)
            RemoveFrogSoup(selectTile.frogSoup);
        else
            if (selectTile.isVisible)
                SetFrogSoup(selectTile.transform.parent, selectTile);
    }

    public void SetFrogSoup(Transform parent, Tile tile)
    {
        if (tile.box != null) return;

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
        if (selectTile == null)
            return;

        if (selectTile.box != null && selectTile.box.boxTypes != 2)
            RemoveBox(selectTile.box);
        else
            if (selectTile.isVisible && selectTile.box == null)
                SetBox(selectTile.transform.parent, selectTile);
    }

    public void SetBox(Transform parent, Tile tile)
    {
        if (tile.character != null || 
            tile.jelly != null || 
            tile.frogSoup != null) return;

        Box box = Instantiate(boxPrefab, parent);
        box.transform.position = tile.transform.position;

        var specialList = UIManager.Instance.mapEditManager.specialList.GetComponent<SpecialList>();
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

        ChangeSpriteBox(box, box.boxLayer, box.boxTypes);

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

    public void ChangeSpriteBox(Box box,int layer,int types)
    {
        Sprite[] getboxsprite = UIManager.Instance.mapEditManager.specialList.GetComponent<SpecialList>().specialSprites;
        if (types == 0)
        {
            switch (layer)
            {
                case 1:
                    box.GetComponentInChildren<SpriteRenderer>().sprite = getboxsprite[2];
                    break;
                case 3:
                    box.GetComponentInChildren<SpriteRenderer>().sprite = getboxsprite[4];
                    break;
                case 5:
                    box.GetComponentInChildren<SpriteRenderer>().sprite = getboxsprite[5];
                    break;
            }
        }
        else if (types == 1)
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
        if(selectTile.box.boxLayer != 0)
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

    public void ActiveAlphaLayerOfBox(Box box ,bool isActive)
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
            hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, 1 << 9);

            if (hit.collider == null) return;

            TileSet tileSet = hit.collider.transform.GetComponent<TileSet>();
            if (tileSet != null)
            {
                tileSet.woodenFenceColliders.SetActive(!tileSet.woodenFenceColliders.activeSelf);
            }
        }
    }

    public void SetWoodenFence(TileSet tileSet, int woodenFenceIndex)
    {
        WoodenFence woodenFence;
        if (inactiveWoodenFences.Count > 0)
        {
            woodenFence = inactiveWoodenFences[0];
            inactiveWoodenFences.RemoveAt(0);
        }
        else
        {
            woodenFence = Instantiate(woodenFencePrefab);
        }

        activeWoodenFences.Add(woodenFence);

        tileSet.woodenFenceShadows[woodenFenceIndex].image.enabled = false;

        WoodenFence shadow = tileSet.woodenFenceShadows[woodenFenceIndex];

        tileSet.woodenFences[woodenFenceIndex] = woodenFence;
        woodenFence.gameObject.SetActive(true);
        woodenFence.transform.SetParent(tileSet.transform);
        woodenFence.transform.localScale = shadow.transform.localScale;
        woodenFence.transform.position = shadow.transform.position;
    }

    public void RemoveWoodenFence(TileSet tileSet, int woodenFenceIndex)
    {
        WoodenFence woodenFence = tileSet.woodenFences[woodenFenceIndex];
        tileSet.woodenFences[woodenFenceIndex] = null;
        tileSet.woodenFenceShadows[woodenFenceIndex].image.enabled = true;

        woodenFence.gameObject.SetActive(false);
        activeWoodenFences.Remove(woodenFence.GetComponent<WoodenFence>());
        inactiveWoodenFences.Add(woodenFence.GetComponent<WoodenFence>());
    }
    #endregion

    #region Vine
    private void OnClickVine()
    {
        Ray ray = new Ray(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, 1 << 9);

        if (hit.collider == null) return;

        TileSet tileSet = hit.collider.GetComponent<TileSet>();
        if (tileSet == null) return;

        if (tileSet.isVisible)
        {
            UIManager.Instance.errorPopup.SetMessage("타일이 전부 비어있어야 설치 가능합니다.");
            return;
        }

        if (tileSet.vine != null)
        {
            RemoveVine(tileSet.vine);
        }
        else
        {
            SetVine(tileSet.transform, tileSet);
        }


    }

    public void SetVine(Transform parent,TileSet tileSet)
    {
        if (tileSet.vine != null) return;

        Vine vine = Instantiate(vinePrefab, parent);

        vine.transform.localScale = Vector3.one;
        vine.transform.position = tileSet.transform.position;

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
