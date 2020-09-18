using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapMoveMode : MonoBehaviour
{
    private GameObject selectMap;
    private GameObject selectMapContainer;
    private Vector3 prevMousePosition;

    private float doubleTapTimer = -1;

    // Update is called once per frame
    public void TouchControll()
    {
        if (Input.GetMouseButtonDown(0))
        {
           OnMouseDownForMapMoving();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (EventSystem.current.IsPointerOverGameObject() == true) return;

            SetNullSelectMap();
        }
        else if (Input.GetMouseButton(0))
        {
            OnMouseMoveForMapMoving();
        }
    }


    public void SelectMap(GameObject map)
    {
        selectMap = map;
    }

    public void SetNullSelectMap()
    {
        if(selectMapContainer != null)
        {
            selectMapContainer = null;
        }
        else if (selectMap != null)
        {
            selectMap = null;

            UIManager.Instance.HideStagePosition();
        }
    }

    private void OnMouseDownForMapMoving()
    {
        if (EventSystem.current.IsPointerOverGameObject() == true) return;

        RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), Mathf.Infinity, 1 << 9);

        if (hit.collider != null)
        {
            if (hit.transform.tag.Equals("TileSet"))
            {
                selectMapContainer = hit.transform.parent.gameObject;

                prevMousePosition = Input.mousePosition;

                return;
            }
        }

        hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), Mathf.Infinity, 1 << 8);

        if (hit.collider != null)
        {
            if (hit.transform.tag.Equals("Map"))
            {
                selectMap = hit.transform.parent.gameObject;

                if (doubleTapTimer == -1)
                {
                    prevMousePosition = Input.mousePosition;

                    UIManager.Instance.ShowStagePosition();

                    doubleTapTimer = 0;

                    StartCoroutine(DoubleTapTimeChecker());

                }
                else
                {
                    MapManager.Instance.mainCamera.transform.position = new Vector3(
                        selectMap.transform.position.x
                        , selectMap.transform.position.y
                        , MapManager.Instance.mainCamera.transform.position.z);
                }
            }
        }
    }

    private IEnumerator DoubleTapTimeChecker()
    {
        while(doubleTapTimer > -1)
        {
            yield return new WaitForFixedUpdate();

            doubleTapTimer += Time.fixedDeltaTime;

            if(doubleTapTimer > 1f)
            {
                doubleTapTimer = -1;
                StopCoroutine(DoubleTapTimeChecker());
            }
        }
    }

    private void OnMouseMoveForMapMoving()
    {
        if(selectMapContainer != null)
        {
            selectMapContainer.transform.Translate((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)Camera.main.ScreenToWorldPoint(prevMousePosition));
            prevMousePosition = Input.mousePosition;

        }
        else if (selectMap != null)
        {
            if (prevMousePosition == Input.mousePosition) return;

            selectMap.transform.Translate((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)Camera.main.ScreenToWorldPoint(prevMousePosition));
            prevMousePosition = Input.mousePosition;
            UIManager.Instance.SetStagePosition(selectMap.transform.position, Input.mousePosition);
        }
    }

}
