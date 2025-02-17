using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace ACEPlay.CrossPromotion
{
    public class CrossPromotionManager : MonoBehaviour
    {

        public static CrossPromotionManager instance;
        [System.Serializable]
        private class CrossInfo
        {
            /// <summary>
            /// iOS: AppID của Game đang được quảng cáo chéo.
            /// <br>Android: Package name của Game đang được quảng cáo chéo.</br>
            /// </summary>
            public string crossApp;
            /// <summary>
            /// index hiện tại của các content quảng cáo chéo cho crossApp
            /// </summary>
            public int indexContent;
            /// <summary>
            /// index hiện tại của Game được quảng cáo chéo
            /// </summary>
            public int indexCross;
            /// <summary>
            /// url của content tương ứng với indexContent
            /// </summary>
            public string contentUrl;
            /// <summary>
            /// Tổng số Game được quảng cáo chéo
            /// </summary>
            public int countCross;
            /// <summary>
            /// Tổng số content quảng cáo chéo cho crossApp
            /// </summary>
            public int countContent;
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Debug.LogError("Destroy Cross");
                Destroy(this.gameObject);
            }
        }

        public Texture2D appIcon;
        public Texture2D appBanner;
        Coroutine coroutineIcon;
        Coroutine coroutineBanner;

        public bool EnableCrossPromotion;

         public bool EnableVideoOnStart
        {
            get { return PlayerPrefs.GetInt("EnableVideoOnStart", 0) == 1; }
            set { PlayerPrefs.SetInt("EnableVideoOnStart", value ? 1 : 0); }
        }
        public bool EnableVideoOnEndgame
        {
            get { return PlayerPrefs.GetInt("EnableVideoOnEndgame", 0) == 1; }
            set { PlayerPrefs.SetInt("EnableVideoOnEndgame", value ? 1 : 0); }
        }
        public bool EnableIconOnMenu
        {
            get { return PlayerPrefs.GetInt("EnableIconOnMenu", 0) == 1; }
            set { PlayerPrefs.SetInt("EnableIconOnMenu", value ? 1 : 0); }
        }
        public bool EnableIconOnEndgame
        {
            get { return PlayerPrefs.GetInt("EnableIconOnMenu", 0) == 1; }
            set { PlayerPrefs.SetInt("EnableIconOnMenu", value ? 1 : 0); }
        }
        public bool EnableBannerOnSetting
        {
            get { return PlayerPrefs.GetInt("EnableBannerOnSetting", 0) == 1; }
            set { PlayerPrefs.SetInt("EnableBannerOnSetting", value ? 1 : 0); }
        }
        public bool EnableBannerOnEndGame
        {
            get { return PlayerPrefs.GetInt("EnableBannerOnEndGame", 0) == 1; }
            set { PlayerPrefs.SetInt("EnableBannerOnEndGame", value ? 1 : 0); }
        }

        public string iconConfigStr;
        public string videoConfigStr;
        public string bannerConfigStr;

        #region Icon
        /// <summary>
        /// Config Data của tất cả Icon quảng cáo chéo
        /// </summary>
        private Dictionary<string, string[]> CrossIcons = new Dictionary<string, string[]>();

        [SerializeField] private CrossInfo CurrentCrossIconInfo;

        public string GetIconURL()
        {
            return CurrentCrossIconInfo.contentUrl;
        }

        public string GetCurrentCrossAppByIcon()
        {
            return CurrentCrossIconInfo.crossApp;
        }

        /// <summary>
        /// Chuyển sang Icon quảng cáo chéo tiếp theo sau mỗi ngày. Nếu game cũ có nhiều Icon thì next sang Icon tiếp theo cho tới khi hết rồi mới next sang Icon của game tiếp theo. Hết tất cả thì quay vòng
        /// </summary>
        public void NextCrossIconByDay()
        {
            int indexContent = CurrentCrossIconInfo.indexContent;
            int countContent = CurrentCrossIconInfo.countContent;
            indexContent++;
            if (indexContent < countContent)
            {
                CurrentCrossIconInfo.indexContent = indexContent;
                CurrentCrossIconInfo.contentUrl = CrossIcons[CurrentCrossIconInfo.crossApp][indexContent];
            }
            else
            {
                int countCross = CurrentCrossIconInfo.countCross;
                int indexCross = CurrentCrossIconInfo.indexCross;
                indexCross++;
                CurrentCrossIconInfo.indexContent = 0;
                if (indexCross >= countCross)
                {
                    indexCross = 0;
                }
                CurrentCrossIconInfo.indexCross = indexCross;
                CurrentCrossIconInfo.crossApp = GetKey(CrossIcons.Keys.ToArray(), indexCross);
                CurrentCrossIconInfo.contentUrl = CrossIcons[CurrentCrossIconInfo.crossApp][0];
                CurrentCrossIconInfo.countContent = CrossIcons[CurrentCrossIconInfo.crossApp].Length;
            }

            PlayerPrefs.SetString("CrossIconInfo", JsonConvert.SerializeObject(CurrentCrossIconInfo));
        }
        //Chuyển sang Icon quảng cáo chéo của game tiếp theo sau khi User đã click vào Icon cho dù game đó vẫn còn Icon khác
        public void NextCrossIconByClick()
        {
            int countCross = CurrentCrossIconInfo.countCross;
            int indexCross = CurrentCrossIconInfo.indexCross;
            indexCross++;
            CurrentCrossIconInfo.indexContent = 0;
            if (indexCross >= countCross)
            {
                indexCross = 0;
            }
            CurrentCrossIconInfo.indexCross = indexCross;
            CurrentCrossIconInfo.crossApp = GetKey(CrossIcons.Keys.ToArray(), indexCross);
            CurrentCrossIconInfo.contentUrl = CrossIcons[CurrentCrossIconInfo.crossApp][0];
            CurrentCrossIconInfo.countContent = CrossIcons[CurrentCrossIconInfo.crossApp].Length;

            PlayerPrefs.SetString("CrossIconInfo", JsonConvert.SerializeObject(CurrentCrossIconInfo));
        }
        #endregion

        #region Video
        /// <summary>
        /// Config Data của tất cả video quảng cáo chéo
        /// </summary>
        private Dictionary<string, string[]> CrossVideos = new Dictionary<string, string[]>();

        [SerializeField] private CrossInfo CurrentCrossVideoInfo;

        public string GetVideoURL()
        {
            return CurrentCrossVideoInfo.contentUrl;
        }

        public string GetCurrentCrossAppByVideo()
        {
            return CurrentCrossVideoInfo.crossApp;
        }

        public string GetCurrentUrlStoreCrossAppByVideo()
        {
#if UNITY_ANDROID
            return $"https://play.google.com/store/apps/details?id={CurrentCrossVideoInfo.crossApp}";
#elif UNITY_IOS
            return $"itms-apps://itunes.apple.com/app/{CurrentCrossVideoInfo.crossApp}";
#else
            return string.Empty;
#endif
        }

        /// <summary>
        /// Chuyển sang video quảng cáo chéo tiếp theo sau mỗi ngày. Nếu game cũ có nhiều video thì next sang video tiếp theo cho tới khi hết rồi mới next sang video của game tiếp theo. Hết tất cả thì quay vòng
        /// </summary>
        public void NextCrossVideoByDay()
        {
            int indexContent = CurrentCrossVideoInfo.indexContent;
            int countContent = CurrentCrossVideoInfo.countContent;
            indexContent++;
            if (indexContent < countContent)
            {
                CurrentCrossVideoInfo.indexContent = indexContent;
                CurrentCrossVideoInfo.contentUrl = CrossVideos[CurrentCrossVideoInfo.crossApp][indexContent];
            }
            else
            {
                int countCross = CurrentCrossVideoInfo.countCross;
                int indexCross = CurrentCrossVideoInfo.indexCross;
                indexCross++;
                CurrentCrossVideoInfo.indexContent = 0;
                if (indexCross >= countCross)
                {
                    indexCross = 0;
                }
                CurrentCrossVideoInfo.indexCross = indexCross;
                CurrentCrossVideoInfo.crossApp = GetKey(CrossVideos.Keys.ToArray(), indexCross);
                CurrentCrossVideoInfo.contentUrl = CrossVideos[CurrentCrossVideoInfo.crossApp][0];
                CurrentCrossVideoInfo.countContent = CrossVideos[CurrentCrossVideoInfo.crossApp].Length;
            }

            PlayerPrefs.SetString("CrossVideoInfo", JsonConvert.SerializeObject(CurrentCrossVideoInfo));
        }
        //Chuyển sang video quảng cáo chéo của game tiếp theo sau khi User đã click vào video cho dù game đó vẫn còn video khác
        public void NextCrossVideoByClick()
        {
            int countCross = CurrentCrossVideoInfo.countCross;
            int indexCross = CurrentCrossVideoInfo.indexCross;
            indexCross++;
            CurrentCrossVideoInfo.indexContent = 0;
            if (indexCross >= countCross)
            {
                indexCross = 0;
            }
            CurrentCrossVideoInfo.indexCross = indexCross;
            CurrentCrossVideoInfo.crossApp = GetKey(CrossVideos.Keys.ToArray(), indexCross);
            CurrentCrossVideoInfo.contentUrl = CrossVideos[CurrentCrossVideoInfo.crossApp][0];
            CurrentCrossVideoInfo.countContent = CrossVideos[CurrentCrossVideoInfo.crossApp].Length;

            PlayerPrefs.SetString("CrossVideoInfo", JsonConvert.SerializeObject(CurrentCrossVideoInfo));
        }
        #endregion

        #region Banner
        /// <summary>
        /// Config Data của tất cả Banner quảng cáo chéo
        /// </summary>
        private Dictionary<string, string[]> CrossBanners = new Dictionary<string, string[]>();

        [SerializeField] private CrossInfo CurrentCrossBannerInfo;

        public string GetBannerURL()
        {
            return CurrentCrossBannerInfo.contentUrl;
        }

        public string GetCurrentCrossAppByBanner()
        {
            return CurrentCrossBannerInfo.crossApp;
        }

        /// <summary>
        /// Chuyển sang Banner quảng cáo chéo tiếp theo sau mỗi ngày. Nếu game cũ có nhiều Banner thì next sang Banner tiếp theo cho tới khi hết rồi mới next sang Banner của game tiếp theo. Hết tất cả thì quay vòng
        /// </summary>
        public void NextCrossBannerByDay()
        {
            int indexContent = CurrentCrossBannerInfo.indexContent;
            int countContent = CurrentCrossBannerInfo.countContent;
            indexContent++;
            if (indexContent < countContent)
            {
                CurrentCrossBannerInfo.indexContent = indexContent;
                CurrentCrossBannerInfo.contentUrl = CrossBanners[CurrentCrossBannerInfo.crossApp][indexContent];
            }
            else
            {
                int countCross = CurrentCrossBannerInfo.countCross;
                int indexCross = CurrentCrossBannerInfo.indexCross;
                indexCross++;
                CurrentCrossBannerInfo.indexContent = 0;
                if (indexCross >= countCross)
                {
                    indexCross = 0;
                }
                CurrentCrossBannerInfo.indexCross = indexCross;
                CurrentCrossBannerInfo.crossApp = GetKey(CrossBanners.Keys.ToArray(), indexCross);
                CurrentCrossBannerInfo.contentUrl = CrossBanners[CurrentCrossBannerInfo.crossApp][0];
                CurrentCrossBannerInfo.countContent = CrossBanners[CurrentCrossBannerInfo.crossApp].Length;
            }

            PlayerPrefs.SetString("CrossBannerInfo", JsonConvert.SerializeObject(CurrentCrossBannerInfo));
        }
        //Chuyển sang Banner quảng cáo chéo của game tiếp theo sau khi User đã click vào Banner cho dù game đó vẫn còn Banner khác
        public void NextCrossBannerByClick()
        {
            int countCross = CurrentCrossBannerInfo.countCross;
            int indexCross = CurrentCrossBannerInfo.indexCross;
            indexCross++;
            CurrentCrossBannerInfo.indexContent = 0;
            if (indexCross >= countCross)
            {
                indexCross = 0;
            }
            CurrentCrossBannerInfo.indexCross = indexCross;
            CurrentCrossBannerInfo.crossApp = GetKey(CrossBanners.Keys.ToArray(), indexCross);
            CurrentCrossBannerInfo.contentUrl = CrossBanners[CurrentCrossBannerInfo.crossApp][0];
            CurrentCrossBannerInfo.countContent = CrossBanners[CurrentCrossBannerInfo.crossApp].Length;

            PlayerPrefs.SetString("CrossBannerInfo", JsonConvert.SerializeObject(CurrentCrossBannerInfo));
        }
        #endregion


        private string GetKey(string[] keys, int index)
        {
            if (index < keys.Length)
                return keys.ToArray()[index];
            return null;
        }

        public void SetUp()
        {
            bool isNewDay = MyTime.instance.isFirstLoginNewDay;

            CrossIcons = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(iconConfigStr);
            if (PlayerPrefs.HasKey("CrossIconInfo"))
            {
                string iconInfoString = PlayerPrefs.GetString("CrossIconInfo");
                CurrentCrossIconInfo = JsonConvert.DeserializeObject<CrossInfo>(iconInfoString);
                if (isNewDay) NextCrossIconByDay();
            }
            else
            {
                string key = GetKey(CrossIcons.Keys.ToArray(), 0);
                CurrentCrossIconInfo = new CrossInfo();
                CurrentCrossIconInfo.contentUrl = CrossIcons[key][0];
                CurrentCrossIconInfo.countContent = CrossIcons[key].Length;
                CurrentCrossIconInfo.countCross = CrossIcons.Count;
                CurrentCrossIconInfo.crossApp = key;
                CurrentCrossIconInfo.indexContent = 0;
                CurrentCrossIconInfo.indexCross = 0;

                PlayerPrefs.SetString("CrossIconInfo", JsonConvert.SerializeObject(CurrentCrossIconInfo));
            }
            if (coroutineIcon != null) StopCoroutine(coroutineIcon);
            coroutineIcon = GetImage(GetIconURL(), (result) => { appIcon = result; });

            CrossVideos = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(videoConfigStr);
            if (PlayerPrefs.HasKey("CrossVideoInfo"))
            {
                string videoInfoString = PlayerPrefs.GetString("CrossVideoInfo");
                CurrentCrossVideoInfo = JsonConvert.DeserializeObject<CrossInfo>(videoInfoString);
                if (isNewDay) NextCrossVideoByDay();
            }
            else
            {
                string key = GetKey(CrossVideos.Keys.ToArray(), 0);
                CurrentCrossVideoInfo = new CrossInfo();
                CurrentCrossVideoInfo.contentUrl = CrossVideos[key][0];
                CurrentCrossVideoInfo.countContent = CrossVideos[key].Length;
                CurrentCrossVideoInfo.countCross = CrossVideos.Count;
                CurrentCrossVideoInfo.crossApp = key;
                CurrentCrossVideoInfo.indexContent = 0;
                CurrentCrossVideoInfo.indexCross = 0;

                PlayerPrefs.SetString("CrossVideoInfo", JsonConvert.SerializeObject(CurrentCrossVideoInfo));
            }

            CrossBanners = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(bannerConfigStr);
            if (PlayerPrefs.HasKey("CrossBannerInfo"))
            {
                string bannerInfoString = PlayerPrefs.GetString("CrossBannerInfo");
                CurrentCrossBannerInfo = JsonConvert.DeserializeObject<CrossInfo>(bannerInfoString);
                if (isNewDay) NextCrossBannerByDay();
            }
            else
            {
                string key = GetKey(CrossBanners.Keys.ToArray(), 0);
                CurrentCrossBannerInfo = new CrossInfo();
                CurrentCrossBannerInfo.contentUrl = CrossBanners[key][0];
                CurrentCrossBannerInfo.countContent = CrossBanners[key].Length;
                CurrentCrossBannerInfo.countCross = CrossBanners.Count;
                CurrentCrossBannerInfo.crossApp = key;
                CurrentCrossBannerInfo.indexContent = 0;
                CurrentCrossBannerInfo.indexCross = 0;

                PlayerPrefs.SetString("CrossBannerInfo", JsonConvert.SerializeObject(CurrentCrossBannerInfo));
            }
            if (coroutineBanner != null) StopCoroutine(coroutineBanner);
            coroutineBanner = GetImage(GetBannerURL(), (result) => { appBanner = result; });
        }

        public bool CheckAppInstallation(string bundleId)
        {
#if UNITY_ANDROID
            bool installed = false;
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageManager = curActivity.Call<AndroidJavaObject>("getPackageManager");
            AndroidJavaObject launchIntent = null;
            try
            {
                launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleId);
                if (launchIntent == null) installed = false;
                else installed = true;
            }
            catch (System.Exception e)
            {
                installed = false;
                Debug.LogError(e.Message);
            }
            return installed;
#else
            return false;
#endif
        }

        #region Loader
        public void GetText(string url, Action<string> callback)
        {
            StartCoroutine(loadStringFileFromURL(url, callback));
        }

        public Coroutine GetImage(string url, Action<Texture2D> callback)
        {
            Texture2D texture = new Texture2D(1, 1);
            return StartCoroutine(loadTexture2DFileFromURL(url, callback));
        }

        public void GetVideo(string url, Action<string> callback)
        {
            string[] splitted = url.Split('/');
            string fileName = splitted[splitted.Length - 1];
            string path = Application.persistentDataPath + "/" + fileName;
            if (File.Exists(path))
            {
                if (callback != null) callback.Invoke(path);
            }
            else
            {
                StartCoroutine(loadArrayByteFileFromURL(url,
                (result) =>
                {
                    File.WriteAllBytes(path, result);
                    if (callback != null) callback.Invoke(path);
                }));
            }
        }

        IEnumerator loadFileFromURL(string url, Action<UnityWebRequest> callback)
        {
            if (!string.IsNullOrEmpty(url))
            {
                using (UnityWebRequest www = new UnityWebRequest(url))
                {
                    yield return www;
                    if (www.isDone && www != null)
                    {
                        callback.Invoke(www);
                    }
                }
            }
        }
        IEnumerator loadTexture2DFileFromURL(string url, Action<Texture2D> callback)
        {
            if (!string.IsNullOrEmpty(url))
            {
                using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
                {
                    yield return uwr.SendWebRequest();
#if UNITY_2020_1_OR_NEWER
                    if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                    {
                        Debug.Log(uwr.error);
                    }
                    else
                    {
                        // Get downloaded asset bundle
                        var texture = DownloadHandlerTexture.GetContent(uwr);
                        callback.Invoke(texture);
                    }
#else
                    if (uwr.isNetworkError || uwr.isHttpError)
                    {
                        Debug.Log(uwr.error);
                    }
                    else
                    {
                        // Get downloaded asset bundle
                        var texture = DownloadHandlerTexture.GetContent(uwr);
                        callback.Invoke(texture);
                    }
#endif
                }
            }
        }

        IEnumerator loadStringFileFromURL(string url, Action<string> result)
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(url))
            {
                yield return uwr.SendWebRequest();
#if UNITY_2020_1_OR_NEWER
                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(uwr.error);
                }
                else
                {
                    Debug.Log(uwr.downloadHandler.data);
                    if (result != null)
                        result(uwr.downloadHandler.text);
                }
#else
                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    Debug.Log(uwr.error);
                }
                else
                {
                    Debug.Log(uwr.downloadHandler.data);
                    if (result != null)
                        result(uwr.downloadHandler.text);
                }
#endif
            }
        }

        IEnumerator loadArrayByteFileFromURL(string url, Action<byte[]> result)
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(url))
            {
                yield return uwr.SendWebRequest();
#if UNITY_2020_1_OR_NEWER
                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(uwr.error);
                }
                else
                {
                    Debug.Log(uwr.downloadHandler.data);
                    if (result != null)
                        result(uwr.downloadHandler.data);
                }
#else
                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    Debug.Log(uwr.error); 
                }
                else
                {
                    Debug.Log(uwr.downloadHandler.data);
                    if (result != null)
                        result(uwr.downloadHandler.data);
                }
#endif
            }
        }
        #endregion
    }
}