using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpecialMode : MonoBehaviour
{
    [SerializeField]
    private Jelly jellyPrefab;
    private List<Jelly> activeJellys = new List<Jelly>();
    private List<Jelly> inactiveJellys = new List<Jelly>();

    [SerializeField]
    private FrogSoup frogSoupPrefab;
    private List<FrogSoup> activeFrogSoups = new List<FrogSoup>();
    private List<FrogSoup> inactiveFrogSoups = new List<FrogSoup>();

    [SerializeField]
    private Box boxPrefab;
    private List<Box> activeBoxs = new List<Box>();
    private List<Box> inactiveBoxs = new List<Box>();

    [SerializeField]
    private WoodenFence woodenFencePrefab;
    private List<WoodenFence> activeWoodenFences = new List<WoodenFence>();
    private List<WoodenFence> inactiveWoodenFences = new List<WoodenFence>();

    public void TouchControll()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnMouseDown();
        }
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject() == true) return;

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
    }

    #region Jelly
    private void OnClickJelly()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0, 1 << 10);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null) continue;
         
            Tile tile = hit.collider.transform.GetComponent<Tile>();

            if (tile.jelly != null)
            {
                RemoveJelly(tile.jelly);
            }
            else
            {
                SetJelly(tile.transform.parent, tile);
            }
        }
    }

    public void SetJelly(Transform parent, Tile tile)
    {
        if (tile.box != null) return;

        Jelly jelly;

        if(inactiveJellys.Count > 0)
        {
            jelly = inactiveJellys[0];
            inactiveJellys.RemoveAt(0);
        }
        else
        {
            jelly = Instantiate(jellyPrefab);
        }

        activeJellys.Add(jelly);

        jelly.gameObject.SetActive(true);
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
        activeJellys.Remove(jelly);
        inactiveJellys.Add(jelly);
        jelly.tile.jelly = null;
        jelly.tile = null;
        jelly.gameObject.SetActive(false);

        jelly.GetComponentInParent<Map>().RemoveJelly(jelly);
    }
    #endregion

    #region FrogSoup
    private void OnClickFrogSoup()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0, 1 << 10);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null) continue;

            Tile tile = hit.collider.transform.GetComponent<Tile>();

            if (tile.frogSoup != null)
            {
                RemoveFrogSoup(tile.frogSoup);
            }
            else
            {
                SetFrogSoup(tile.transform.parent, tile);
            }
        }
    }

    public void SetFrogSoup(Transform parent, Tile tile)
    {
        if (tile.box != null) return;

        FrogSoup frogSoup;

        if (inactiveFrogSoups.Count > 0)
        {
            frogSoup = inactiveFrogSoups[0];
            inactiveFrogSoups.RemoveAt(0);
        }
        else
        {
            frogSoup = Instantiate(frogSoupPrefab);
        }

        if (frogSoup != null)
        {
            activeFrogSoups.Add(frogSoup);

            frogSoup.gameObject.SetActive(true);
            frogSoup.transform.SetParent(parent);
            frogSoup.transform.localScale = Vector3.one;
            frogSoup.transform.position = tile.transform.position;

            frogSoup.fenceIndex = tile.GetComponentInParent<TileSet>().tileSetIndex;
            frogSoup.tileIndex = tile.tileIndex;

            frogSoup.tile = tile;
            tile.frogSoup = frogSoup;

            tile.transform.parent.GetComponent<TileSet>().map.SetFrogSoup(frogSoup);
        }

    }

    public void RemoveFrogSoup(FrogSoup frogSoup)
    {
        activeFrogSoups.Remove(frogSoup);
        inactiveFrogSoups.Add(frogSoup);
        frogSoup.tile.frogSoup = null;
        frogSoup.tile = null;
        frogSoup.gameObject.SetActive(false);

        frogSoup.GetComponentInParent<Map>().RemoveFrogSoup(frogSoup);
    }
    #endregion

    #region Box
    private void OnClickBox()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0, 1 << 10);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null) continue;

            Tile tile = hit.collider.transform.GetComponent<Tile>();

            if (tile.box != null)
            {
                RemoveBox(tile.box);
            }
            else
            {
                SetBox(tile.transform.parent, tile);
            }
        }
    }

    public void SetBox(Transform parent, Tile tile)
    {
        if (tile.character != null) return;
        if (tile.jelly != null) return;
        if (tile.frogSoup != null) return;

        Box box;

        if (inactiveBoxs.Count > 0)
        {
            box = inactiveBoxs[0];
            inactiveBoxs.RemoveAt(0);
        }
        else
        {
            box = Instantiate(boxPrefab);
        }

        activeBoxs.Add(box);

        box.gameObject.SetActive(true);
        box.transform.SetParent(parent);
        box.transform.localScale = Vector3.one;
        box.transform.position = tile.transform.position;

        box.fenceIndex = tile.GetComponentInParent<TileSet>().tileSetIndex;
        box.tileIndex = tile.tileIndex;

        box.tile = tile;
        tile.box = box;

        tile.transform.parent.GetComponent<TileSet>().map.SetBox(box);
    }

    public void RemoveBox(Box box)
    {
        activeBoxs.Remove(box);
        inactiveBoxs.Add(box);
        box.tile.box = null;
        box.tile = null;
        box.gameObject.SetActive(false);

        box.GetComponentInParent<Map>().RemoveBox(box);
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
                if (tileSet.woodenFenceColliders.activeSelf == false)
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
}
