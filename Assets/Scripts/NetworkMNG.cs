using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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
}
