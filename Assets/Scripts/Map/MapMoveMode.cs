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
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 prevPosition = Camera.main.ScreenToWorldPoint(prevMousePosition);
        Vector2 translation = mousePosition - prevPosition;

        if (selectMapContainer != null)
        {
            Vector3 newPosition = selectMapContainer.transform.localPosition + new Vector3(translation.x, translation.y, 0f);
            newPosition.x = Mathf.Round(newPosition.x * 100f) / 100f;
            newPosition.y = Mathf.Round(newPosition.y * 100f) / 100f;
            selectMapContainer.transform.localPosition = newPosition;

            prevMousePosition = Input.mousePosition;
            UIManager.Instance.SetMapPositionText(selectMapContainer.transform.localPosition);

        }
        else if (selectMap != null)
        {
            if (prevMousePosition == Input.mousePosition)
                return;
            Vector3 newPosition = selectMap.transform.localPosition + new Vector3(translation.x, translation.y, 0f);
            newPosition.x = Mathf.Round(newPosition.x * 100f) / 100f;
            newPosition.y = Mathf.Round(newPosition.y * 100f) / 100f;
            selectMap.transform.localPosition = newPosition;

            prevMousePosition = Input.mousePosition;
            UIManager.Instance.SetStagePosition(selectMap.transform.position, Input.mousePosition);
        }
    }
}
