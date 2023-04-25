using UnityEngine;
using UnityEngine.UI;

public class LoadStageList : MonoBehaviour
{
    public Text loadListText;
    /// <summary>
    /// 로드 팝업에서 리스트 버튼을 통해 스테이지 로드
    /// </summary>
    public void OnClickLoadButton()
    {
        string mapUrl = NetworkMNG.instance.ServerMapDataURL;
        string mapType = UIManager.Instance.mapdataMNG.currentMapType.captionText.text;
        string StageNumber = loadListText.text;

        string mapFolder = mapType + "/";
        string mapName = "stage_" + StageNumber + "_1.json";

        mapUrl += mapFolder + mapName;

        NetworkMNG.instance.LoadJson(mapUrl, mapFolder, mapName);

        UIManager.Instance.loadStagePopup.SetActive(false);
        UIManager.Instance.mapdataMNG.CloseStageList();
    }
}
