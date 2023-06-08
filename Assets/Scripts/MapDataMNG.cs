using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MapDataMNG : MonoBehaviour
{
    public static MapDataMNG Instance { get; private set; }

    public GameObject loadingPopup, versionUpPopup;

    public GameObject stageList, content;
    public Text currentVersionText, currentTotalStageText, DevButtonText, DevText, editVersionText;
    public Text updateStageList_normal_Text, updateStageList_ranking_Text, updateStageList_tutorial_Text, updateStageList_mushroom_Text, updateStageVersionText;
    public Dropdown currentMapType, currentChapter;
    public ScrollRect sr;

    public static bool iSDev = false;
    public static string mapEditVersion { get; private set; } = "v1.1.1";
    private void Awake()
    {
        editVersionText.text = "에디터 버전 : " + mapEditVersion;
    }
    public int mapDataVersion
    {
        get
        {
            return PlayerPrefs.GetInt("MapDataVersion", 0);
        }

        set
        {
            PlayerPrefs.SetInt("MapDataVersion", value);
        }
    }

    /// <summary>
    /// 버전, 맵 타입에 따른 스테이지 리스트를 가져옴
    /// </summary>
    public void DownloadStageData()

    {
        NetworkMNG.instance.StartNetworking(true, "user_info/map_check", null, (result) =>
        {

            JObject jo = JObject.Parse(result);

            currentVersionText.text = jo["latest_version"].ToString();
            ShowStageList("Load");
        });

        SetVisibleLoading(false);
    }
    /// <summary>
    /// 맵을 로드하는 함수
    /// </summary>
    public void MapDataDownload()
    {
        string mapUrl = NetworkMNG.instance.ServerMapDataURL;
        string mapType = currentMapType.captionText.text;
        string StageNumber = UIManager.Instance.loadStageNumber.text;
        if (string.IsNullOrEmpty(StageNumber))
        {
            UIManager.Instance.errorPopup.SetMessage("올바른 스테이지를 입력해주세요.");
            SetVisibleLoading(false);
            return;
        }

        string mapFolder = mapType.ToString() + "/";
        string mapName = "";
        if (currentChapter.gameObject.activeSelf)
        {
            mapName = "stage_" + StageNumber + "_" + (currentChapter.value + 1).ToString() + ".json";
        }
        else
        {
            mapName = "stage_" + StageNumber + "_1.json";
        }



        mapUrl += mapFolder + mapName;

        NetworkMNG.instance.LoadJson(mapUrl, mapFolder, mapName);

        SetVisibleLoading(false);
    }
    /// <summary>
    /// 맵을 저장하는 함수
    /// </summary>
    public void MapDataSave()
    {
        JsonFileMaker jsonFileMaker = UIManager.Instance.mapEditManager.jsonSaveButton.GetComponent<JsonFileMaker>();
        string jsonData = jsonFileMaker.SaveJsonForAndroid();
        if (jsonData == null)
        {
            SetVisibleLoading(false);
            return;
        }
        int chapter = UIManager.Instance.currentChapter.gameObject.activeSelf ? UIManager.Instance.currentChapter.value + 1 : 1;
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();

        form.Add(new MultipartFormDataSection("stage_number", jsonFileMaker.stageNumber.text));
        form.Add(new MultipartFormDataSection("map_data", jsonData));
        form.Add(new MultipartFormDataSection("mode", UIManager.Instance.stageType.value.ToString()));
        form.Add(new MultipartFormDataSection("chapter", chapter.ToString()));
        NetworkMNG.instance.StartNetworking(true, "map_edit/map_upload", form, (result) =>
        {
            //Debug.Log(JObject.Parse(result));
            JObject jo = JObject.Parse(result);
            Debug.Log(result);
            string result_code = jo["result"].ToString();
            string stagenumber = jo["stage_number"].ToString();
            string chapter = jo["chapter"].ToString();
            string mode = jo["mode"].ToString();
            if (result_code == "r8001") // 업로드 성공
            {
                string message = "맵 업로드에 성공했습니다. 해당 맵은 " + UIManager.Instance.stageType.captionText.text + "타입";
                if (UIManager.Instance.stageType.value == 3)
                {
                    message += " " + UIManager.Instance.currentChapter.captionText.text;
                }
                message += "의 " + jsonFileMaker.stageNumber.text + "스테이지 입니다. 버전 업데이트를 꼭 눌러주세요.";
                UIManager.Instance.errorPopup.SetMessage(message);
            }
            else if (result_code == "r8002") // 업로드 실패
            {
                UIManager.Instance.errorPopup.SetMessage("통신 실패, 다시 시도해주세요.");
            }
            else
            {
                UIManager.Instance.errorPopup.SetMessage("통신 실패, 다시 시도해주세요.");
            }
        });

        SetVisibleLoading(false);
    }
    /// <summary>
    /// 맵 데이터를 전부 가져오는 함수
    /// </summary>
    public void ShowStageList(string check)
    {
        string mapUrl = NetworkMNG.instance.ServerMapDataURL;
        string mapType = currentMapType.captionText.text;

        currentChapter.gameObject.SetActive(mapType == "mushroom");

        string mapFolder = mapType.ToString() + "/";

        mapUrl += mapFolder;

        NetworkMNG.instance.LoadMapList(mapUrl, mapFolder, check);

        SetVisibleLoading(false);
    }
    /// <summary>
    /// 가져온 맵 데이터를 html 파싱하는 함수
    /// </summary>
    /// <param name="htmldata"> html 데이터 </param>
    /// <param name="check"> 저장인지 로드인지 인스펙터 창에서 정의 </param>
    public void SetStageList(string htmldata, string check)
    {
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(htmldata);
        int totalMapCnt = 0;
        bool flag = false;

        // tr 태그를 찾아서 반복문 실행
        foreach (HtmlNode tr in doc.DocumentNode.SelectNodes("//tr"))
        {
            // td 태그가 4개 미만이라면 다음 반복문으로 이동
            if (tr.ChildNodes.Count < 4) continue;

            // td 태그 중 첫 번째 태그 안의 값을 가져옴
            HtmlNode td = tr.ChildNodes[1];
            string fileName = td.InnerText.Trim();

            // 파일명에 "stage_" 문자열이 포함되어 있지 않다면 다음 반복문으로 이동
            if (!fileName.Contains("stage_")) continue;

            // td 태그 중 세 번째 태그 안의 값을 가져옴 (마지막 수정 날짜)
            td = tr.ChildNodes[2];
            string lastModified = td.InnerText.Trim();

            // 파일 크기를 가져오려면 td 태그 중 네 번째 태그 안의 값을 사용할 수 있음
            // td = tr.ChildNodes[3];
            // string fileSize = td.InnerText.Trim();

            // 파일명에서 스테이지 번호를 추출
            string[] splitName = fileName.Split('_');
            string stageNumber = splitName[1];
            string fileExtension = ".json";
            string chapter = splitName[2].Replace(fileExtension, "");

            if (check == "Load")
            {
                if (currentChapter.gameObject.activeSelf && int.Parse(chapter) == currentChapter.value + 1)
                {
                    GameObject newList = Instantiate(stageList, content.transform);
                    newList.GetComponent<LoadStageList>().loadListText.text = stageNumber;
                    totalMapCnt++;
                    currentTotalStageText.text = totalMapCnt.ToString();
                }
                else if (!currentChapter.gameObject.activeSelf)
                {
                    GameObject newList = Instantiate(stageList, content.transform);
                    newList.GetComponent<LoadStageList>().loadListText.text = stageNumber;
                    totalMapCnt++;
                    currentTotalStageText.text = totalMapCnt.ToString();
                }
            }
            if (check == "Save")
            {
                if (UIManager.Instance.mapEditManager.jsonSaveButton.GetComponent<JsonFileMaker>().stageNumber.text == stageNumber)
                {
                    Debug.Log(chapter);
                    if (UIManager.Instance.currentChapter.gameObject.activeSelf && (UIManager.Instance.currentChapter.value + 1) == int.Parse(chapter))
                    {
                        UIManager.Instance.saveStageWarnningPopup.SetActive(true);
                        UIManager.Instance.saveStageWarnningPopup.transform.Find("duplicateStage").GetComponent<Text>().text = stageNumber;
                        UIManager.Instance.saveStageWarnningPopup.transform.Find("lastModified").GetComponent<Text>().text = lastModified;
                        flag = true;
                        break;
                    }
                    else if (!UIManager.Instance.currentChapter.gameObject.activeSelf)
                    {
                        UIManager.Instance.saveStageWarnningPopup.SetActive(true);
                        UIManager.Instance.saveStageWarnningPopup.transform.Find("duplicateStage").GetComponent<Text>().text = stageNumber;
                        UIManager.Instance.saveStageWarnningPopup.transform.Find("lastModified").GetComponent<Text>().text = lastModified;
                        flag = true;
                        break;
                    }
                }
            }
        }
        if (!flag && check == "Save")
            MapDataSave();
    }
    /// <summary>
    /// 저장된 맵들의 버전을 업데이트 하는 함수
    /// </summary>
    public void VersionUpdate()
    {
        NetworkMNG.instance.StartNetworking(true, "map_edit/map_version_up", null, (result) =>
        {
            //Debug.Log(JObject.Parse(result));
            JObject jo = JObject.Parse(result);

            string result_code = jo["result"].ToString();
            string version = jo["version"].ToString();
            string normalStage = jo["stage_normal"].ToString();
            string rankingStage = jo["stage_ranking"].ToString();
            string tutorialStage = jo["stage_tutorial"].ToString();
            string mushroomStage = jo["stage_mushroom"].ToString();

            if (result_code == "r8011") // 업로드 성공
            {
                SetVersionUpPopup(true);
                updateStageVersionText.text = (int.Parse(version) - 1).ToString() + "   ---->   " + version;
                updateStageList_normal_Text.text = normalStage;
                updateStageList_ranking_Text.text = rankingStage;
                updateStageList_tutorial_Text.text = tutorialStage;
                updateStageList_mushroom_Text.text = mushroomStage;
            }
            else if (result_code == "r8012") // 업로드 실패
            {
                UIManager.Instance.errorPopup.SetMessage("통신 실패, 다시 시도해주세요.");
            }
            else if (result_code == "r8013") // 변경 사항 없음
            {
                UIManager.Instance.errorPopup.SetMessage("업데이트할 맵이 없습니다.");
            }
            else
            {
                UIManager.Instance.errorPopup.SetMessage("통신 실패, 다시 시도해주세요.");
            }
        });

        SetVisibleLoading(false);
    }
    /// <summary>
    /// 맵 에디터 다운로드 실행 함수
    /// </summary>
    public void MapEditorDownload()
    {
        string mapeditUrl = NetworkMNG.instance.ServerMapDataURL + "map_editer/";
        if (iSDev)
            mapeditUrl = "http://puzzle-sugar-flavor.com:80/sugarmonster/public_html/map_data/" + "map_editer/";
        NetworkMNG.instance.MapEditDown(mapeditUrl);
    }
    /// <summary>
    /// html 파싱을 통하여 맵 에디터 폴더에 있는 파일 이름을 리스트에 넣고 반환하는 함수
    /// </summary>
    /// <param name="htmldata"> html 데이터 </param>
    /// <returns> 파일 이름 리스트 </returns>
    public List<string> SetMapEditorFileList(string htmldata)
    {
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(htmldata);

        List<string> fileList = new List<string>();
        foreach (HtmlNode tr in doc.DocumentNode.SelectNodes("//table/tr[position() > 3]"))
        {
            if (tr.ChildNodes.Count < 4) continue;

            HtmlNode td = tr.ChildNodes[1];
            string fileName = td.InnerText.Trim();
            fileList.Add(fileName); // 파일 이름을 리스트에 추가
        }
        return fileList;
    }
    public void SetVisibleLoading(bool isActive)
    {
        loadingPopup.SetActive(isActive);
    }
    public void SetVersionUpPopup(bool isActive)
    {
        versionUpPopup.SetActive(isActive);
    }
    public void CloseUpdateStageList()
    {
        updateStageList_normal_Text.text = "";
        updateStageList_ranking_Text.text = "";
        updateStageList_tutorial_Text.text = "";
        updateStageList_mushroom_Text.text = "";
        updateStageVersionText.text = "";
    }
    public void CloseStageList()
    {
        for (int i = 0; i < content.transform.childCount; i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }
        sr.content.localPosition = new Vector3(0, 1, 0); // 스크롤바 초기화
    }
    public void OnClickChangeDev()
    {
        if (iSDev)
        {
            iSDev = false;
            DevButtonText.text = "현재 : 본 서버";
            NetworkMNG.SettingUrlPass();
        }
        else
        {
            iSDev = true;
            DevButtonText.text = "현재 : 테스트 서버";
            NetworkMNG.SettingUrlPass();
        }
    }
}