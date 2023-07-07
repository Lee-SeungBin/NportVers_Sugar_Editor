using UnityEngine;
using UnityEngine.UI;

public class ChurrosInfoPopup : MonoBehaviour
{
    public Toggle directionToggle;
    private void OnEnable()
    {
        //MapManager.Instance.specialMode.selectTile.box.boxDirection
    }
    public void OnClickAddChurros()
    {
        MapManager.Instance.CreateChurros();
    }

    public void OnClickDeleteChurros()
    {
        MapManager.Instance.DeleteChurros();
    }

    public void OnChangeDirection()
    {
        MapManager.Instance.ChangeDirectionChurros(directionToggle.isOn);
    }
}
