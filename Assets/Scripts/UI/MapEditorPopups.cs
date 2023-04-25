using UnityEngine;

public class MapEditorPopups : MonoBehaviour
{
    public MissionPopup mission;
    public NextMapPopup nextMap;

    private void Start()
    {
        HideMissionPopup();
        HideNextMapPopup();
    }

    public void ShowMissionPopup()
    {
        mission.gameObject.SetActive(true);
    }

    public void HideMissionPopup()
    {
        mission.gameObject.SetActive(false);
    }

    public void ShowNextMapPopup()
    {
        nextMap.gameObject.SetActive(true);
    }
    public void HideNextMapPopup()
    {
        nextMap.gameObject.SetActive(false);
    }
}
