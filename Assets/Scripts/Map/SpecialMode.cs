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
        else if (Input.GetMouseButtonUp(0))
        {
            if (EventSystem.current.IsPointerOverGameObject() == true) return;

            OnMouseUpForSetSpecial();
        }
        else if(Input.GetMouseButton(1))
        {
            UIManager.Instance.mapEditManager.specialList.GetComponent<SpecialList>().SelectEmtpy();
            MapManager.Instance.HideWoodenFence();
        }
    }
    public void SetNullToSelectTile()
    {
        selectTile = null;
    }

    private void OnMouseDownForSetSpecial()
    {
        if (EventSystem.current.IsPointerOverGameObject() == true) return;

        SetNullToSelectTile();

        UIManager.Instance.mapEditManager.HideSandWichInfoPopup();
        UIManager.Instance.mapEditManager.HideSandWichChangePopup();
        
        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0, 1 << 10);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null) continue;

            selectTile = hit.collider.transform.GetComponent<Tile>();
        }


        if (UIManager.Instance.dragItem.specialType == SpecialList.SPECIAL_TYPE.JELLY)
        {
            OnClickJelly();
        }
        else if (UIManager.Instance.dragItem.specialType == SpecialList.SPECIAL_TYPE.FROG_SOUP)
        {
            OnClickFrogSoup();
        }
        else if (UIManager.Instance.dragItem.specialType == SpecialList.SPECIAL_TYPE.BOX)
        {
            OnClickBox();
        }
        else if (UIManager.Instance.dragItem.specialType == SpecialList.SPECIAL_TYPE.WOODEN_FENCE)
        {
            OnClickWoodenFence();
        }
        else if (UIManager.Instance.dragItem.specialType == SpecialList.SPECIAL_TYPE.VINE)
        {
            OnClickVine();
        }
    }
    private void OnMouseUpForSetSpecial()
    {
        if (EventSystem.current.IsPointerOverGameObject() == true) return;

        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0, 1 << 10);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                if (hit.transform.tag.Equals("Tile"))
                {
                    if (selectTile == hit.collider.transform.GetComponent<Tile>())
                    {
                        if (selectTile.box != null && selectTile.box.boxTypes == 2)
                        {
                            UIManager.Instance.mapEditManager.ShowSandWichInfoPopup(selectTile.box);

                        }

                        break;
                    }
                }
            }
        }
    }

    #region Jelly
    private void OnClickJelly()
    {
        //RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0, 1 << 10);

        //foreach (RaycastHit2D hit in hits)
        //{
        //    if (hit.collider == null) continue;

        //    Tile tile = hit.collider.transform.GetComponent<Tile>();

        //    if (tile.jelly != null)
        //    {
        //        RemoveJelly(tile.jelly);
        //    }
        //    else
        //    {
        //        if(tile.isVisible)
        //            SetJelly(tile.transform.parent, tile);
        //    }
        //}
        if (selectTile == null) return;


        if (selectTile.jelly != null)
        {
            RemoveJelly(selectTile.jelly);
        }
        else
        {
            if (selectTile.isVisible)
                SetJelly(selectTile.transform.parent, selectTile);
        }
    }

    public void SetJelly(Transform parent, Tile tile)
    {
        if (tile.box != null) return;

        Jelly jelly;

        //if(inactiveJellys.Count > 0)
        //{
        //    jelly = inactiveJellys[0];
        //    inactiveJellys.RemoveAt(0);
        //}
        //else
        //{
            jelly = Instantiate(jellyPrefab);
        //}

        //activeJellys.Add(jelly);

        //jelly.gameObject.SetActive(true);
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
        //activeJellys.Remove(jelly);
        //inactiveJellys.Add(jelly);
        jelly.tile.jelly = null;
        jelly.tile = null;
        //jelly.gameObject.SetActive(false);

        Destroy(jelly.gameObject);

        jelly.GetComponentInParent<Map>().RemoveJelly(jelly);
    }
    #endregion

    #region FrogSoup
    private void OnClickFrogSoup()
    {
        //RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0, 1 << 10);

        //foreach (RaycastHit2D hit in hits)
        //{
        //    if (hit.collider == null) continue;

        //    Tile tile = hit.collider.transform.GetComponent<Tile>();

        //    if (tile.frogSoup != null)
        //    {
        //        RemoveFrogSoup(tile.frogSoup);
        //    }
        //    else
        //    {
        //        if(tile.isVisible)
        //            SetFrogSoup(tile.transform.parent, tile);
        //    }
        //}
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

        FrogSoup frogSoup;

        //if (inactiveFrogSoups.Count > 0)
        //{
        //    frogSoup = inactiveFrogSoups[0];
        //    inactiveFrogSoups.RemoveAt(0);
        //}
        //else
        //{
            frogSoup = Instantiate(frogSoupPrefab);
        //}

        if (frogSoup != null)
        {
            //activeFrogSoups.Add(frogSoup);

            //frogSoup.gameObject.SetActive(true);
            frogSoup.transform.SetParent(parent);
            frogSoup.transform.localScale = Vector3.one;
            frogSoup.transform.position = tile.transform.position;
            frogSoup.transform.Translate(Vector3.up * (0.119f)); // 위치가 살짝 틀어져서 맞추기 위함

            frogSoup.fenceIndex = tile.GetComponentInParent<TileSet>().tileSetIndex;
            frogSoup.tileIndex = tile.tileIndex;

            frogSoup.tile = tile;
            tile.frogSoup = frogSoup;

            tile.transform.parent.GetComponent<TileSet>().map.SetFrogSoup(frogSoup);
        }

    }

    public void RemoveFrogSoup(FrogSoup frogSoup)
    {
        //activeFrogSoups.Remove(frogSoup);
        //inactiveFrogSoups.Add(frogSoup);
        frogSoup.tile.frogSoup = null;
        frogSoup.tile = null;
        //frogSoup.gameObject.SetActive(false);

        Destroy(frogSoup.gameObject);

        frogSoup.GetComponentInParent<Map>().RemoveFrogSoup(frogSoup);
    }
    #endregion

    #region Box
    private void OnClickBox()
    {
        //RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0, 1 << 10);

        //foreach (RaycastHit2D hit in hits)
        //{
        //    if (hit.collider == null) continue;

        //    Tile tile = hit.collider.transform.GetComponent<Tile>();

        //    if (tile.box != null && tile.box.boxTypes != 2)
        //    {
        //        RemoveBox(tile.box);
        //    }
        //    else
        //    {
        //        if (tile.isVisible && tile.box == null)
        //            SetBox(tile.transform.parent, tile);
        //    }
        //}

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
        if (tile.character != null) return;
        if (tile.jelly != null) return;
        if (tile.frogSoup != null) return;

        Box box;

        //if (inactiveBoxs.Count > 0)
        //{
        //    box = inactiveBoxs[0];
        //    inactiveBoxs.RemoveAt(0);
        //}
        //else
        //{
        //if (UIManager.Instance.mapEditManager.specialList.GetComponent<SpecialList>().boxtype == 2)
        //    box = Instantiate(SandWichPrefab);
        //else
            box = Instantiate(boxPrefab);
        //}

        //activeBoxs.Add(box);

        //box.gameObject.SetActive(true);
        box.transform.SetParent(parent);
        box.transform.localScale = Vector3.one;
        box.transform.position = tile.transform.position;

        box.fenceIndex = tile.GetComponentInParent<TileSet>().tileSetIndex;
        box.tileIndex = tile.tileIndex;

        box.boxLayer = UIManager.Instance.mapEditManager.specialList.GetComponent<SpecialList>().boxlayer; // 박스 레이어 가져오기
        box.boxTypes = UIManager.Instance.mapEditManager.specialList.GetComponent<SpecialList>().boxtype; // 박스 타입 가져오기

        if(box.boxTypes == 2)
        {
            box.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = box.boxsprite[5];
            box.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
            UIManager.Instance.mapEditManager.ShowSandWichInfoPopup(box);
            UIManager.Instance.mapEditManager.specialList.GetComponent<SpecialList>().SelectEmtpy();
        }

        ChangeSpriteBox(box, box.boxLayer, box.boxTypes);

        box.tile = tile;
        tile.box = box;

        tile.transform.parent.GetComponent<TileSet>().map.SetBox(box);
    }

    public void RemoveBox(Box box)
    {
        //activeBoxs.Remove(box);
        //inactiveBoxs.Add(box);
        box.tile.box = null;
        box.tile = null;
        //box.gameObject.SetActive(false);

        box.GetComponentInParent<Map>().RemoveBox(box);

        Destroy(box.gameObject);
    }

    public void ChangeSpriteBox(Box box,int layer,int types)
    {
        Sprite[] getboxsprite = UIManager.Instance.mapEditManager.specialList.GetComponent<SpecialList>().specialSprites;

        if (types == 0)
        {
            if (layer == 1)
                box.GetComponentInChildren<SpriteRenderer>().sprite = getboxsprite[2];
            else if (layer == 3)
                box.GetComponentInChildren<SpriteRenderer>().sprite = getboxsprite[4];
            else if (layer == 5)
                box.GetComponentInChildren<SpriteRenderer>().sprite = getboxsprite[5];
        }
        else if (types == 1) // 햄버거 로드
        {
            box.GetComponentInChildren<SpriteRenderer>().sprite = getboxsprite[8];
        }
        else if (types == 2)
            LoadTasteOfBox(box);
    }
    public void LoadTasteOfBox(Box box)
    {
        GameObject alphalayer = box.transform.GetChild(0).gameObject;
        box.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = box.boxsprite[5];
        box.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
        for (int i = 0; i < box.boxLayer; i++)
        {
            GameObject layer = Instantiate(box.transform.GetChild(0).gameObject);

            layer.transform.SetParent(box.transform);
            layer.transform.position = box.transform.position;
            layer.transform.Translate(Vector3.up * (0.135f * (i + 1)));
            layer.GetComponent<SpriteRenderer>().sprite = box.boxsprite[box.boxTier[i]];
            layer.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

            alphalayer.transform.position = box.transform.position;
            alphalayer.transform.Translate(Vector3.up * (0.135f * (i + 1)));
            alphalayer.GetComponent<SpriteRenderer>().sortingOrder++;
        }
        alphalayer.transform.position = box.transform.position;
        alphalayer.transform.Translate(Vector3.up * (0.135f * (box.boxLayer + 1)));

        ActiveAlphaLayerOfBox(box, false);
    }
    public void CreateTasteOfBox()
    {
        GameObject layer;

        int currentTaste = UIManager.Instance.mapEditManager.sandwichInfoPopup.TasteStepDropdown.value;
        //selectTile.box.transform.GetChild(selectTile.box.boxLayer).gameObject.SetActive(true);

        //샌드위치 레이어 생성
        layer = Instantiate(selectTile.box.transform.GetChild(0).gameObject);

        layer.transform.SetParent(selectTile.box.transform);
        layer.transform.position = selectTile.box.transform.position;
        layer.transform.Translate(Vector3.up * (0.135f * (selectTile.box.boxLayer + 1)));
        layer.GetComponent<SpriteRenderer>().sprite = selectTile.box.boxsprite[currentTaste];
        layer.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

        //selectTile.box.transform.GetChild(selectTile.box.boxLayer).gameObject.SetActive(true);
        //selectTile.box.transform.GetChild(selectTile.box.boxLayer).GetComponent<SpriteRenderer>().sprite = selectTile.box.boxsprite[currentTaste];
        //selectTile.box.transform.GetChild(selectTile.box.boxLayer).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

        selectTile.box.boxTier.Add(currentTaste);
        selectTile.box.boxLayer++;

        GameObject alphalayer = selectTile.box.transform.GetChild(0).gameObject;

        alphalayer.transform.position = selectTile.box.transform.position;
        alphalayer.transform.Translate(Vector3.up * (0.135f * (selectTile.box.boxLayer + 1)));
        alphalayer.GetComponent<SpriteRenderer>().sortingOrder++;

        UIManager.Instance.mapEditManager.sandwichInfoPopup.SetData(selectTile.box);

        //if (selectTile.box.boxLayer == 5)
        //    return;
        //else
        //{
        //    selectTile.box.transform.GetChild(selectTile.box.boxLayer).gameObject.SetActive(true);
        //    selectTile.box.transform.GetChild(selectTile.box.boxLayer).GetComponent<SpriteRenderer>().sprite = selectTile.box.boxsprite[5];
        //    selectTile.box.transform.GetChild(selectTile.box.boxLayer).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
        //}
    }

    public void ChangeTasteOfBox(int SelectLayer)
    {
        int currentTaste = UIManager.Instance.mapEditManager.sandwichChangeDropDown.value;

        selectTile.box.boxTier[SelectLayer] = currentTaste;
        selectTile.box.transform.GetChild(SelectLayer + 1).GetComponent<SpriteRenderer>().sprite = selectTile.box.boxsprite[currentTaste];
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

            GameObject alphalayer = selectTile.box.transform.GetChild(0).gameObject;
            GameObject deletelayer = selectTile.box.transform.GetChild(selectTile.box.boxLayer).gameObject;

            alphalayer.transform.position = deletelayer.transform.position;
            Destroy(deletelayer);
            alphalayer.GetComponent<SpriteRenderer>().sortingOrder--;
            selectTile.box.boxLayer--;

            UIManager.Instance.mapEditManager.sandwichInfoPopup.SetData(selectTile.box);
        }
    }

    public void DeleteTasteLayerOfBox(int SelectLayer)
    {
        if(selectTile.box.boxLayer != 0)
        {
            for (int i = SelectLayer + 1; i <= selectTile.box.boxLayer; i++) // 샌드위치 레이어 재구성
            {
                GameObject layer = selectTile.box.transform.GetChild(i).gameObject;
                GameObject alphalayer = selectTile.box.transform.GetChild(0).gameObject;
                if (i == selectTile.box.boxLayer) // 마지막 레이어일때 맨위만 파괴
                {
                    Destroy(layer);
                }
                else // 선택된 층부터 위에 층꺼를 하나씩 땡겨옴
                {
                    layer.GetComponent<SpriteRenderer>().sprite = selectTile.box.transform.GetChild(i + 1).GetComponent<SpriteRenderer>().sprite;
                }
                alphalayer.transform.position = layer.transform.position; // 파괴된 위치(맨위)에 투명 레이어 이동
                alphalayer.GetComponent<SpriteRenderer>().sortingOrder = layer.GetComponent<SpriteRenderer>().sortingOrder;
            }

            selectTile.box.boxTier.RemoveAt(SelectLayer);
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

        TileSet tileSet;

        if (hit.collider != null)
        {
            GameObject shadowFence = hit.collider.gameObject;
            int index = int.Parse(shadowFence.name.Split('_')[1]);
            tileSet = shadowFence.transform.parent.parent.GetComponent<TileSet>();

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

            tileSet = hit.collider.transform.GetComponent<TileSet>();
            if (tileSet != null)
            {
                if (!tileSet.woodenFenceColliders.activeSelf)
                {
                    tileSet.woodenFenceColliders.SetActive(true);
                }
                else
                {
                    tileSet.woodenFenceColliders.SetActive(false);
                }
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
        
        TileSet tileSet;

        if (hit.collider == null)
            return;
        else
        {
            tileSet = hit.collider.transform.GetComponent<TileSet>();

            if (tileSet.isVisible)
            {
                UIManager.Instance.errorPopup.SetMessage("타일이 전부 비어있어야 설치 가능합니다.");
                return;
            }
            else
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
        }

    }

    public void SetVine(Transform parent,TileSet tileSet)
    {
        if (tileSet.vine != null)
            return;

        Vine vine;

        //if (inactiveVines.Count > 0)
        //{
        //    vine = inactiveVines[0];
        //    inactiveVines.RemoveAt(0);
        //}
        //else
        //{
            vine = Instantiate(vinePrefab);
        //}

        //activeVines.Add(vine);

        //vine.gameObject.SetActive(true);
        vine.transform.SetParent(parent);
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
        //activeVines.Remove(vine);
        //inactiveVines.Add(vine);
        vine.tileset.vine = null;
        vine.tileset = null;
        //vine.gameObject.SetActive(false);

        Destroy(vine.gameObject);

        vine.GetComponentInParent<Map>().RemoveVine(vine);
    }
    #endregion
}
