using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkMNG : MonoBehaviour
{
    private static NetworkMNG _instance;
    public static NetworkMNG instance
    {
        get
        {
            return _instance;
        }
    }

    public string ServerMapDataURL { get => _serverMapDataURL; set => _serverMapDataURL = value; }


    private static string _serverURL = "";

    private static string _releaseServerURL = "http://puzzle-sugar-flavor.com:80/sugarmonster/public_html/index.php/";
    private static string _devServerURL = "http://114.108.130.129:80/sugarmonster/public_html/index.php/";



    private static string _serverMapDataURL = "";

    private static string _releaseServerMapDataURL = "http://puzzle-sugar-flavor.com:80/sugarmonster/public_html/map_data/";
    private static string _devServerMapDataURL = "http://114.108.130.129:80/sugarmonster/public_html/map_data/";



    public string gachaRateUrl = "https://puzzle-sugar-flavor.com/sugarmonster_admin/public_html/index.php/admin/gacha_rate";
    public string gachaSpecialRateUrl = "https://puzzle-sugar-flavor.com/sugarmonster_admin/public_html/index.php/admin/gacha_special_rate";

    public string gachaRankingRateUrl = "https://puzzle-sugar-flavor.com/sugarmonster_admin/public_html/index.php/admin/gacha_ranking_rate";

    public string leagueRewardUrl = "https://puzzle-sugar-flavor.com/sugarmonster_admin/public_html/index.php/admin/league_reward";


    public string termsUrl = "https://puzzle-sugar-flavor.com/terms/terms_of_use.html";

    public string termsViewKrUrl = "http://puzzle-sugar-flavor.com/sugarmonster_admin/public_html/index.php/admin/a_terms_view_kr";
    public string termsViewEnUrl = "http://puzzle-sugar-flavor.com/sugarmonster_admin/public_html/index.php/admin/a_terms_view_en";

    public string termsInfoViewKrUrl = "http://puzzle-sugar-flavor.com/sugarmonster_admin/public_html/index.php/admin/a_terms_view_kr/info";
    public string termsInfoViewEnUrl = "http://puzzle-sugar-flavor.com/sugarmonster_admin/public_html/index.php/admin/a_terms_view_en/info";

    public static string resourceBundleBaseUrl = "";
    public static string rankingBundleBaseUrl = "";

#if UNITY_EDITOR || UNITY_ANDROID
    public static string resourceBundleReleaseUrl = "http://puzzle-sugar-flavor.com/sugarmonster/public_html/app_assets/android/";
    public static string resourceBundleDevUrl = "http://114.108.130.129/sugarmonster/public_html/app_assets/android/";
#else
    public static string resourceBundleReleaseUrl   = "http://puzzle-sugar-flavor.com/sugarmonster/public_html/app_assets/ios/";
    public static string resourceBundleDevUrl       = "http://114.108.130.129/sugarmonster/public_html/app_assets/ios/";
#endif
    public static string rankingBundleReleaseUrl = "http://puzzle-sugar-flavor.com/sugarmonster/public_html/app_assets/ranking/";
    public static string rankingBundleDevUrl = "http://114.108.130.129/sugarmonster/public_html/app_assets/ranking/";

    public delegate void NetworkResultCallback(string result);
    public delegate void ImageCallback(Texture texture);

    public const string NETWORK_ERROR = "networkError";

    private void Awake()
    {
        _instance = this;
        SettingUrlPass();
        UIManager.Instance.mapdataMNG.SetVisibleLoading(true);
        UIManager.Instance.mapdataMNG.MapEditorDownload();
    }

    public static void SettingUrlPass()
    {
        if (MapDataMNG.iSDev)
        {
            _serverURL = _devServerURL;
            _serverMapDataURL = _devServerMapDataURL;
            resourceBundleBaseUrl = resourceBundleDevUrl;
            rankingBundleBaseUrl = rankingBundleDevUrl;
        }
        else
        {
            _serverURL = _releaseServerURL;
            _serverMapDataURL = _releaseServerMapDataURL;

            resourceBundleBaseUrl = resourceBundleReleaseUrl;
            rankingBundleBaseUrl = rankingBundleReleaseUrl;
        }
    }

    public void StartNetworking(bool networkingCheck, string url, List<IMultipartFormSection> form, NetworkResultCallback networkResultCallback)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable && networkingCheck)
        {
            UIManager.Instance.errorPopup.SetMessage("통신시작 과정에서 통신 실패.");
        }
        else
        {
            StartCoroutine(PostNetworking(url, form, networkResultCallback));
        }
    }

    IEnumerator PostNetworking(string url, List<IMultipartFormSection> form, NetworkResultCallback networkResultCallback)
    {
        UnityWebRequest webRequest = UnityWebRequest.Post(_serverURL + url, form);
        webRequest.SetRequestHeader("Accept", "application/json");

        yield return webRequest.SendWebRequest();

        if (webRequest.error != null)
        {
            UIManager.Instance.errorPopup.SetMessage("보내는 과정에서 통신 실패.");
        }
        else
        {
            networkResultCallback?.Invoke(webRequest.downloadHandler.text);
        }
        webRequest.Dispose();
    }
    public void LoadMapList(string url, string folderName, string check) => StartCoroutine(LoadMapListFile(url, folderName, check));

    IEnumerator LoadMapListFile(string url, string folderName, string check)
    {

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.isDone && request.error == null)
        {
            string data = request.downloadHandler.text;
            UIManager.Instance.mapdataMNG.SetStageList(data, check);
        }
        else
        {
            UIManager.Instance.errorPopup.SetMessage("통신 오류");
        }
        request.Dispose();
    }



    public void LoadJson(string url, string folderName, string dataName) => StartCoroutine(LoadJsonFile(url, folderName, dataName));

    IEnumerator LoadJsonFile(string url, string folderName, string dataName)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        string dataAsJson = request.downloadHandler.text.ToString();

        if (request.isDone && request.error == null)
        {
            UIManager.Instance.LoadJsonForAndroid(dataAsJson);
        }
        else
        {
            UIManager.Instance.errorPopup.SetMessage("통신 오류 또는 해당 스테이지는 존재하지 않습니다.");
        }
        request.Dispose();
    }

    public void MapEditDown(string url) => StartCoroutine("MapEditFileDown", url);
    /// <summary>
    /// 맵 에디터 폴더에서 파일을 불러와 버전을 체크하고 다운로드 하는 함수
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    IEnumerator MapEditFileDown(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (!request.isDone && request.error != null)
        {
            UIManager.Instance.errorPopup.SetMessage("통신 오류가 발생하였습니다.");
        }
        else
        {
            string data = request.downloadHandler.text;

            List<string> filelist = UIManager.Instance.mapdataMNG.SetMapEditorFileList(data);
            string currentVersion = MapDataMNG.mapEditVersion;
            string[] latestVersion = null;
            foreach (string str in filelist)
            {
                UnityWebRequest downrequest = UnityWebRequest.Get(url + str);
                yield return downrequest.SendWebRequest();

                if (!downrequest.isDone && request.error != null)
                {
                    UIManager.Instance.errorPopup.SetMessage("통신 오류가 발생하였습니다.");
                }
                else
                {
                    if (str.Equals("Patch Note.txt"))
                    {
                        string patchNote = System.Text.Encoding.UTF8.GetString(downrequest.downloadHandler.data);
                        latestVersion = patchNote.Split('>');
                        UIManager.Instance.updateListPopup.transform.Find("Viewport").Find("Content").Find("UpdateList").GetComponent<Text>().text = patchNote;
                    }
                    if (latestVersion[0] != currentVersion)
                    {
                        File.WriteAllBytes(Application.dataPath + "/" + str, downrequest.downloadHandler.data);
                    }
                }
                downrequest.Dispose();
            }
            if (latestVersion[0] != currentVersion)
            {
                UIManager.Instance.mapdataMNG.SetVisibleLoading(false);
                UIManager.Instance.errorPopup.SetMessage("최신 버전이 있습니다.\n" + Application.dataPath + " 폴더에 \n" + latestVersion[0] + "\n버전이 다운로드 되었습니다. 알집을 풀고 새로운 버전을 이용해 주세요.");
                UIManager.Instance.updateListPopup.SetActive(true);
            }
            else
            {
                UIManager.Instance.mapdataMNG.SetVisibleLoading(false);
                UIManager.Instance.errorPopup.SetMessage("최신 버전입니다.");
                UIManager.Instance.updateListPopup.SetActive(true);
            }
        }
        request.Dispose();
    }
}