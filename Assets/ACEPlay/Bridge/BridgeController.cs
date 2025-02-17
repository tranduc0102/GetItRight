using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TA;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;


namespace ACEPlay.Bridge
{
    public enum AdsEvent
    {
        RewardLoad,
        RewardClick,
        RewardShow,
        RewardFail,
        RewardComplete,
        InterFail,
        InterImpression,
        InterLoad,
        InterShow,
        InterClick,
        InterAdmobFail,
        InterAdmobLoad,
        InterAdmobShow,
        InterAdmobClick,
        AoaLoad,
        AoaFail,
        AoaShow,
        BannerLoaded,
        MrecLoaded,
    }
    public class BridgeController : MonoBehaviour
    {
        public static BridgeController instance;

        #region Controll Game

        // Ads All Game
        public bool CanShowAds //true: show ads, false: not show ads
        {
            get { return PlayerPrefs.GetInt("CanShowAds", 1) == 1; }
            set { PlayerPrefs.SetInt("CanShowAds", value ? 1 : 0); }
        }
        public bool CanShowAdsWithVip //true: show ads VIP, false: not show ads VIP
        {
            get { return PlayerPrefs.GetInt("CanShowAdsWithVip", 1) == 1; }
            set { PlayerPrefs.SetInt("CanShowAdsWithVip", value ? 1 : 0); }
        }
        public bool OpenAdsOnAwake //Lần đầu tiên khi ấn để play level thì có show inter hay không
        {
            get { return PlayerPrefs.GetInt("OpenAdsOnAwake", 1) == 1; }
            set { PlayerPrefs.SetInt("OpenAdsOnAwake", value ? 1 : 0); }
        }
        public float TimeIntersitial  // thời gian delay sau mỗi lần hiện quảng cáo inter
        {
            get { return PlayerPrefs.GetFloat("TimeIntersitial", 20f); }
            set { PlayerPrefs.SetFloat("TimeIntersitial", value); }
        }
        public int LevelAllowInter // Level bat dau xuat hien inter
        {
            get { return PlayerPrefs.GetInt("LevelAllowInter", 2); }
            set { PlayerPrefs.SetInt("LevelAllowInter", value); }
        }
        public int TotalLevelPlayToShowInter // số level hoàn thành để xuất hiện inter
        {
            get { return PlayerPrefs.GetInt("TotalLevelPlayToShowInter", 2); }
            set { PlayerPrefs.SetInt("TotalLevelPlayToShowInter", value); }
        }
        public bool CanShowInterIngame // có show quảng cáo trong lúc chơi game không
        {
            get { return PlayerPrefs.GetInt("CanShowInterIngame", 0) == 1; }
            set { PlayerPrefs.SetInt("CanShowInterIngame", value ? 1 : 0); }
        }
        public float TimeShowInterIngame // thời gian delay mỗi lần show quảng cáo ingame
        {
            get { return PlayerPrefs.GetFloat("TimeShowAdsIngame", 40f); }
            set { PlayerPrefs.SetFloat("TimeShowAdsIngame", value); }
        }
        public bool CanReloadInterFail // có show popup neu khong load duoc quang cao khong
        {
            get { return PlayerPrefs.GetInt("CanReloadInterFail", 0) == 1; }
            set { PlayerPrefs.SetInt("CanReloadInterFail", value ? 1 : 0); }
        }


        //AOA

        public bool CanShowAOA //Có cho phép hiển thị quảng cáo Open App hay không
        {
            get { return PlayerPrefs.GetInt("CanShowAOA", 1) == 1; }
            set { PlayerPrefs.SetInt("CanShowAOA", value ? 1 : 0); }
        }
        public int CountShowAOA //số lần mở game để show AOA
        {
            get { return PlayerPrefs.GetInt("aoa_show", 2); }
            set { PlayerPrefs.SetInt("aoa_show", value); }
        }
        public bool ShouldSkipAppOpenAd { get; set; }


        // Mrec ads
        public bool CanShowMREC  // co show quang cao Mrec khong
        {
            get { return PlayerPrefs.GetInt("CanShowMREC  ", 1) == 1 ? true : false; }
            set { PlayerPrefs.SetInt("CanShowMREC  ", value ? 1 : 0); }
        }

        //banner collap
        public bool CanShowBannerCollapsible  // co show quang cao Banner cuon khong
        {
            get { return PlayerPrefs.GetInt("CanShowBannerCollapsible", 1) == 1 ? true : false; }
            set { PlayerPrefs.SetInt("CanShowBannerCollapsible", value ? 1 : 0); }
        }
        public int LevelAllowBannerCollapse // Level show quảng cáo banner Collap
        {
            get { return PlayerPrefs.GetInt("LevelAllowBannerCollapse", 2); }
            set { PlayerPrefs.SetInt("LevelAllowBannerCollapse", value); }
        }
        public float TimeBannerCollapse  // thời gian delay sau mỗi lần hiện quảng cáo bannme
        {
            get { return PlayerPrefs.GetFloat("TimeBannerCollapse", 15f); }
            set { PlayerPrefs.SetFloat("TimeBannerCollapse", value); }
        }

        // native ads
        public bool CanShowNativeAds  // co show quang cao Native khong
        {
            get { return PlayerPrefs.GetInt("CanShowNativeAds  ", 1) == 1 ? true : false; }
            set { PlayerPrefs.SetInt("CanShowNativeAds  ", value ? 1 : 0); }
        }

        public float TimeRefreshNative // thời gian delay sau mỗi lần hiện quảng cáo inter
        {
            get { return PlayerPrefs.GetFloat("TimeRefreshNative", 15f); }
            set { PlayerPrefs.SetFloat("TimeRefreshNative", value); }
        }

        public bool IsFreeContinue //true: free continue, false: not free
        {
            get { return PlayerPrefs.GetInt("IsFreeContinue", 0) == 1; }
            set { PlayerPrefs.SetInt("IsFreeContinue", value ? 1 : 0); }
        }

        public bool IsShowPackAtStart //true: show pack menu, false: not show pack menu
        {
            get { return PlayerPrefs.GetInt("IsShowPackAtStart", 0) == 1; }
            set { PlayerPrefs.SetInt("IsShowPackAtStart", value ? 1 : 0); }
        }
        public int RewardedLevelPlayToday // RewardedLevelPlayToday++ khi xem reward thanh cong
        {
            get { return PlayerPrefs.GetInt("RewardedLevelPlayToday", 0); }
            set { PlayerPrefs.SetInt("RewardedLevelPlayToday", value); }
        }
        public bool IsVipComplete //true: VIP, false: not VIP
        {
            get { return PlayerPrefs.GetInt("IsVip", 0) == 1; }
            set { PlayerPrefs.SetInt("IsVip", value ? 1 : 0); }
        }

        public string LastLogin
        {
            get { return PlayerPrefs.GetString("LastLogin", DateTime.Now.ToString()); }
            set { PlayerPrefs.SetString("LastLogin", value); }
        }
        public bool InternetRequire //Có yêu cầu internet hay không?
        {
            get { return PlayerPrefs.GetInt("InternetRequire", 1) == 1; }
            set { PlayerPrefs.SetInt("InternetRequire", value ? 1 : 0); }
        }
        public int LevelCheckInternet
        {
            get { return PlayerPrefs.GetInt("LevelCheckInternet", 2); }
            set { PlayerPrefs.SetInt("LevelCheckInternet", value); }
        }
        public bool IsNotification
        {
            get { return PlayerPrefs.GetInt("IsNotification", 1) == 1; }
            set { PlayerPrefs.SetInt("IsNotification", value ? 1 : 0); }
        }
       
       
       
        public float CameraAspectGame
        {
            get { return PlayerPrefs.GetFloat("CameraAspectGame", 0.56f); }
            set { PlayerPrefs.SetFloat("CameraAspectGame", value); }
        }
        public List<string> NonConsumableList = new List<string>();
        public List<string> VipPackageList = new List<string>();
        #endregion

        #region Device Test
        public void ChangeTestMode()
        {
            IsDeviceTest = !IsDeviceTest;
            if (IsDeviceTest)
            {
                if (TryGetComponent(out FPSDisplay Fps))
                {
                    Fps.enabled = true;
                }
                else
                {
                    gameObject.AddComponent<FPSDisplay>();
                }
            }
            else
            {
                if (TryGetComponent(out FPSDisplay Fps))
                {
                    Fps.enabled = false;
                }
            }
        }
        public bool IsDeviceTest
        {
            get
            {
#if UNITY_EDITOR
                return true;
#else

                return PlayerPrefs.GetInt(nameof(IsDeviceTest), 1) == 1;
#endif
            }
            private set
            {
                PlayerPrefs.SetInt(nameof(IsDeviceTest), value ? 1 : 0);
            }
        }


        public void Debug_Log(string log, bool alwayShowLog = false)
        {
            if (alwayShowLog || IsDeviceTest) Debug.Log(log);
        }
        public void Debug_LogWarning(string log, bool alwayShowLog = false)
        {
            if (alwayShowLog || IsDeviceTest) Debug.LogWarning(log);
        }
        public void Debug_LogError(string log, bool alwayShowLog)
        {
            if (alwayShowLog || IsDeviceTest) Debug.LogError(log);
        }
        #endregion


        public bool testGame;
        [SerializeField] private Camera cam;

        public bool isLoadGameSuccess = false;
        public bool isCheckFirebase;
        [HideInInspector]
        public bool IsBannerShowing;
        [HideInInspector]
        public bool IsMRECsShowing;
        [HideInInspector]
        public bool canShowBanner = false;
        [HideInInspector]
        public float lastTimeShowAd = 0;
        [HideInInspector]
        public float lastTimeShowBannerCollap = 0;
        [HideInInspector]
        public float lastTimeShowAdIngame = 0;

        [SerializeField] private string[] urls = new string[] { "https://www.google.com", "https://www.wikipedia.com", "https://www.amazon.com", "https://y.qq.com", "https://www.microsoft.com" };
        int amountOpen;

        public int PlayCount { get; set; }


        [SerializeField]
        public List<ItemIAP> availableItemsIAP = new List<ItemIAP>();
        private void Awake()
        {
            //PlayerPrefs.DeleteAll();
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
                if (PlayerPrefs.HasKey(nameof(NonConsumableList)))
                {
                    try
                    {
                        NonConsumableList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(PlayerPrefs.GetString(nameof(NonConsumableList)));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                    };
                }
            }
            else Destroy(this.gameObject);
        }

        private IEnumerator Start()
        {
            amountOpen = PlayerPrefs.GetInt("Amount_Open", 0);
            amountOpen++;
            PlayerPrefs.SetInt("Amount_Open", amountOpen);
            if (cam != null)
            {
                CameraAspectGame = cam.aspect;
                Debug.LogError(" aspect : " + CameraAspectGame);
            }
            lastTimeShowAd = Time.time;
            lastTimeShowAdIngame = Time.time;

            yield return new WaitWhile(() => Input.touchCount == 0);
            int countTop = 0;
            foreach (Touch touch in Input.touches)
            {
                if (touch.position.y > Screen.height / 2)
                {
                    countTop++;
                }
            }
            if (countTop != 3)
            {
                IsDeviceTest = false;
            }
            else
            {
                IsDeviceTest = false;
                ChangeTestMode();
            }
        }
        public string GetHostNameOrAddress(int index)
        {
            index = index % urls.Length;
            return urls[index].Replace("https://", "");
        }
        public float TotalIAP
        {
            get { return PlayerPrefs.GetInt("TotalIAP", 0); }
            set { PlayerPrefs.SetInt("TotalIAP", (int)Math.Ceiling(value)); }
        }
        public float TotalIAP_Day
        {
            get { return PlayerPrefs.GetInt("TotalIAP_Day", 0); }
            set { PlayerPrefs.SetInt("TotalIAP_Day", (int)Math.Ceiling(value)); }
        }
        public bool CheckOwnerNonConsumable(string keyIAP)
        {
            if (NonConsumableList != null && NonConsumableList.Count > 0)
            {
                return NonConsumableList.Contains(keyIAP);
            }
            return false;
        }
        public void AddNonConsumableLists(string productSku)
        {
            if (!NonConsumableList.Contains(productSku))
            {
                NonConsumableList.Add(productSku);
                PlayerPrefs.SetString(nameof(NonConsumableList), Newtonsoft.Json.JsonConvert.SerializeObject(NonConsumableList));
            }
        }
        public void AddVipPackageLists(string productSku)
        {
            if (!VipPackageList.Contains(productSku))
            {
                VipPackageList.Add(productSku);
            }
        }
        public bool HasInternet() // true : co internet - false : khong co internet
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                return true;
            }
            if (ConnectInternetManager.instance != null)
            {
                return ConnectInternetManager.instance.isOnline;
            }
            return false;
        }

        #region ADS
        public float GetBannerHeightInPixels()
        {
            return 150f;
        }
        public bool IsShowingBanner()
        {
            return IsBannerShowing;
        }
        bool startShowBanner = false;
        public bool CheckTimeShowBannerCollap()
        {
            //trả về tình trạng quảng cáo inter cho bên Code
            if (CanShowAds)
            {
                Debug.Log("Time Banner : " + TimeBannerCollapse);
                if ((Time.time - lastTimeShowBannerCollap >= TimeBannerCollapse) || (!startShowBanner))
                {
                    startShowBanner = true;
                    return true;
                }
                return false;
            }
            return false;
        }
        public void ShowBannerCollapsible(bool showMrec = false)
        {
            //if (testGame)
            //    return;
            //if (CanShowAds)
            //{
                Debug.Log("=====Banner Collap Show success!=====");
                canShowBanner = true;
            //}
            //else
            //    HideBannerCollapsible();
        }
        public void HideBannerCollapsible()
        {
            Debug.Log("=====Banner Collap Hide!=====");
        }
        public void ShowBanner()
        {
            //if (testGame)
            //    return;
            //if (CanShowAds)
            //{
                Debug.Log("=====Banner Show success!=====");
                canShowBanner = true;
            //}
            //else
            //    HideBanner();

        }
        public void HideBanner()
        {
            canShowBanner = false;
            Debug.Log("=====Banner Hide success!=====");
        }
        public bool IsMRECsAdsReady()
        {
            return true;
        }
        public void ShowMRECs()
        {
            //if (CanShowMREC)
            //{
            //    if (IsMRECsAdsReady())
            //    {
                    Debug.Log("=====Mrec Show success!=====");
            //    }
            //}
        }

        public void HideMRECs(bool activeBanner = true)
        {
            //if (IsMRECsShowing)
            //{
                Debug.Log("=====Hide Mrec!=====");
            //}
        }
        bool startShow = false;
        public bool CheckTimeShowInter()
        {
            //trả về tình trạng quảng cáo inter cho bên Code
            if (CanShowAds)
            {
                Debug.Log("Time Inter : " + TimeIntersitial);
                if ((Time.time - lastTimeShowAd >= TimeIntersitial) || (!startShow && OpenAdsOnAwake))
                {
                    startShow = true;
                    return true;
                }
                return false;
            }
            return false;
        }
        public bool IsInterReady()
        {
            return CheckTimeShowInter() && CheckLevelShowAds(LevelAllowInter);
        }
        public bool CheckStatusShowInter()
        {
            return true;
        }
        public bool CheckLevelShowAds(int level)
        {
            if (CanShowAds)
            {
                return true;
            }
            return false;
        }
        public bool IsCheckLevelShowInter()
        {
            return false;
            // return countAdsIngame >= LevelCountShowAds;
        }

        /// <param name="placement"></param>
        /// <param name="onClosed">Được gọi khi không có inter hoặc khi đóng quảng cáo inter</param>
        /// <param name="onDone">Được gọi sau khi show inter thành công</param>
        /// <param name="counter">Tùy từng kịch bản, nó có thể là số lần Win/Lose hay số lần chơi</param>
        /// <param name="alwayShowInters"></param>
        /// <param name="onFailed"></param>
        public void ShowInterstitial(string placement, UnityEvent onClosed, UnityEvent onDone = null, UnityEvent onFailed = null, int counter = 0, bool alwayShowInters = false)
        {
            Debug.Log("=====Interstitial Show success!=====");
            if (counter >= 2)
            {
                if (onDone != null) onDone.Invoke();
            }
            
            if (onClosed != null) onClosed.Invoke();
        }
        public bool IsRewardReady()
        {
            return true;
        }
        public void ShowRewarded(string placement, UnityEvent onRewarded, UnityEvent onFailed = null)
        {
            Debug.Log($"=====Rewarded Show success!===== | placement: {placement}");
            if (testGame)
            {
                if (onRewarded != null) onRewarded.Invoke();
                return;
            }
            if (onRewarded != null) onRewarded.Invoke();
        }
        public bool CheckShowInterReward()
        {
            return false;
        }
        public void ShowIntersitialRewardedAd(UnityEvent onRewarded, UnityEvent onFailed = null)
        {
            Debug.Log($"===== Show Intersitial RewardedAd =====");
            if (onRewarded != null) onRewarded.Invoke();
        }

        #endregion

        #region IAP
        /// @param productId
        /// 	Store product id
        /// @return Item with given product id
        /// 
        public ItemIAP GetItem(string _productId)
        {
            foreach (var item in availableItemsIAP)
            {
                if (item.productIdIAP == _productId)
                    return item;
            }

            return null;
        }
        public void PurchaseProduct(string productSku, UnityStringEvent onDonePurchaseEvent)
        {
            TotalIAP += 0.99f;
            TotalIAP_Day += 0.99f;
            if (productSku.Contains("removeads"))
            {
                AddNonConsumableLists(productSku);
            }
            if (onDonePurchaseEvent != null) onDonePurchaseEvent.Invoke(productSku);
        }

        public void RestorePurchase()
        {
            Debug.Log("=====Restore Purchase success!=====");

        }

        public void PurchaseFailed()
        {
            Debug.Log("=====Purchase failed!=====");
        }
        #endregion


        #region Social
        public void RateGame(UnityEvent onRateRewarded = null)
        {
            Debug.Log("=====Rate Success=====");
            if (onRateRewarded != null) onRateRewarded.Invoke();
        }

        public void ShareGame(UnityEvent onShareRewarded)
        {
            Debug.Log("=====Share Success=====");
            if (onShareRewarded != null) onShareRewarded.Invoke();
        }
        public void PublishHighScore(int score)
        {
            Debug.Log("=====Publish Score:" + score + "Success=====");
        }
        //hien thi ban xep hang
        public void ShowLeaderBoard()
        {
            Debug.Log("=====Show Leaderboard success!=====");
        }

      
        public void Moregames()
        {
            Debug.Log("=====Show Moregames success!=====");
        }
        public void ShowWebsite()
        {
            Debug.Log("=====Show Website success!=====");
        }   
        public void SubcribeYoutube(UnityEvent onSubcribedYoutube)
        {
            Debug.Log("=====Subcribe Youtube Success=====");
            if (onSubcribedYoutube != null) onSubcribedYoutube.Invoke();
        }
        public void FollowTwitter(UnityEvent onFollowTwitter)
        {
            Debug.Log("=====Show Twitter Success=====");
            if (onFollowTwitter != null) onFollowTwitter.Invoke();
        }
        public void FolllowTikTok(UnityEvent onFolllowTikTok)
        {
            Debug.Log("=====Folllow TikTok Success=====");
            if (onFolllowTikTok != null) onFolllowTikTok.Invoke();
        }
        public void FolllowFacebook(UnityEvent onFolllowFacebook)
        {
            Debug.Log("=====Folllow TikTok Success=====");
            if (onFolllowFacebook != null) onFolllowFacebook.Invoke();
        }
        public void FolllowInstagram(UnityEvent onFolllowInstagram)
        {
            Debug.Log("=====Folllow TikTok Success=====");
            if (onFolllowInstagram != null) onFolllowInstagram.Invoke();
        }

        public void InstallGame()
        {
            //link store game install
        }
        public bool CheckAppInstallation(string bundleId)
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_ANDROID
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
            }
            return installed;
#elif UNITY_IOS
            return false;
#endif
            return false;
        }
        #endregion

        #region Analytics

        public void LogEvent(string eventName)
        {
            Debug_Log($"eventName: {eventName}");
        }
        public void LogEvent(string eventName, string paramName, string paramValue)
        {
            Debug_Log($"eventName: {eventName} | paramName: {paramName} | paramValue: {paramValue}");
        }
        public void LogEvent(string eventName, Dictionary<string, object> parameters)
        {
            Debug_Log($"eventName: {eventName} | parameters: {Newtonsoft.Json.JsonConvert.SerializeObject(parameters)}");
        }
        public void LogAdsEvent(AdsEvent eventType, string placement, string errorMsg = "")
        {
            Debug_Log($"eventType: {eventType} | placement: {placement} | errorMsg: {errorMsg}");

        }
        public void SetUserProperty(string name, string value)
        {
            Debug_Log($"name: {name} | value: {value}");
        }
        /// <summary>
        /// Retention of user.
        /// </summary>
        /// <param name="value"></param> retend day. Ex: D0 => 0, D7 => 7
        public void SetPropertyRetendDay(string value)
        {
            SetUserProperty("retent_type", value);
        }
        /// <summary>
        /// Number of days the user has played, different from Retention.
        /// </summary>
        /// <param name="value"></param> day played. Ex: the user installs at D0 and plays after 7 days, the retention is D7 and days_played is 2
        public void SetPropertyDayPlayed(string value)
        {
            SetUserProperty("days_playing", value);
        }
        public void SetPropertyPayPlayed(string value)
        {
            SetUserProperty("paying_type", value);
        }
        /// <summary>
        /// Level values ​​of events are logged afterwards, updated after level related events are logged.
        /// </summary>
        /// <param name="value"></param> the value is the maximum user level passed plus 1, (initial value when first installing the game and not passing any which level is 1). Ex: when pass level 1 => 2
        public void SetPropertyLevel(string value)
        {
            SetUserProperty("level_reach", value);
        }
        public void SetPropertyAppVersion()
        {
            SetUserProperty("app_version", Application.version);
        }

        public void SetPropertyCoinSpend(string value)
        {
            SetUserProperty("total_spent", value);
        }

        public void SetPropertyCoinEarn(string value)
        {
            SetUserProperty("total_earn", value);
        }

        /// <summary>
        /// Level start event(Log when start level)
        /// </summary>
        /// <param name="level"></param> level start
        /// <param name="currentGold"></param>  amount of money at the start of level
        public void LogLevelStartWithParameter(int level)
        {
            var firebase_evt = new Dictionary<string, object>
            {
                {"level", level }
            };
            LogEvent("start_level", firebase_evt);

            if (PlayerPrefs.GetInt($"playlevel_{level}", 0) == 0)
            {
                PlayerPrefs.SetInt($"playlevel_{level}", 1);
                LogEvent($"level_start_{level:0000}");
            }
            else
            {
                LogEvent($"level_retry_{level:0000}");
            }
        }
        /// <summary>
        /// Level Complete Event(Log when end level)
        /// </summary>
        /// <param name="level"></param> level complete
        /// <param name="timePlay"></param> time to complete level
        public void LogLevelCompleteWithParameter(int level)
        {
            var firebase_evt = new Dictionary<string, object>
            {
                {"level", level }
            };
            LogEvent("win_level", firebase_evt);

            if (PlayerPrefs.GetInt($"winlevel_{level}", 0) == 0)
            {
                PlayerPrefs.SetInt($"winlevel_{level}", 1);
                LogEvent($"level_complete_{level:0000}");
            }
        }
        /// <summary>
        /// Level Fail Event(Log when end level)
        /// </summary>
        /// <param name="level"></param> level fail
        /// <param name="failCount"></param> amount fail on level
        public void LogLevelFailWithParameter(int level)
        {
            var firebase_evt = new Dictionary<string, object>
            {
                {"level", level }
            };
            LogEvent("lose_level", firebase_evt);
        }
        #region Tracking Level with GameName
        /// <summary>
        /// Level start event(Log when start level)
        /// </summary>
        /// <param name="level"></param> level start
        /// <param name="currentGold"></param>  amount of money at the start of level
        public void LogLevelStartWithParameter(string gamename, int level)
        {
            var firebase_evt = new Dictionary<string, object>
            {
                {$"level_{gamename}", level }
            };
            LogEvent($"start_level_{gamename}", firebase_evt);

            if (PlayerPrefs.GetInt($"playlevel_{gamename}{level}", 0) == 0)
            {
                PlayerPrefs.SetInt($"playlevel_{gamename}{level}", 1);
                LogEvent($"level_start_{gamename}_{level:0000}");
            }
            else
            {
                LogEvent($"level_retry_{gamename}_{level:0000}");
            }
        }
        /// <summary>
        /// Level Complete Event(Log when end level)
        /// </summary>
        /// <param name="level"></param> level complete
        /// <param name="timePlay"></param> time to complete level
        public void LogLevelCompleteWithParameter(string gamename, int level)
        {
            var firebase_evt = new Dictionary<string, object>
            {
                {$"level_{gamename}", level }
            };
            LogEvent($"win_level_{gamename}", firebase_evt);

            if (PlayerPrefs.GetInt($"winlevel_{gamename}_{level}", 0) == 0)
            {
                PlayerPrefs.SetInt($"winlevel_{gamename}_{level}", 1);
                LogEvent($"level_complete_{gamename}_{level:0000}");
            }
        }
        /// <summary>
        /// Level Fail Event(Log when end level)
        /// </summary>
        /// <param name="level"></param> level fail
        /// <param name="failCount"></param> amount fail on level
        public void LogLevelFailWithParameter(string gamename, int level)
        {
            var firebase_evt = new Dictionary<string, object>
            {
                 {$"level_{gamename}", level }
            };
            LogEvent($"level_fail_{gamename}", firebase_evt);
        }

        #endregion
        /// <summary>
        /// Spend Virtual Currency event.
        /// </summary>
        /// <param name="typeCurrency"></param> type currency of earn. Ex: gold, diamond,...
        /// <param name="amountSpend"></param> amount of spend
        /// <param name="itemName"></param> currency to buy something. Ex: skin1, weapon1,...
        public void LogSpendCurrency(string typeCurrency, int amountSpend, string itemName)
        {
            var firebase_evt = new Dictionary<string, object>
            {
                {"virtual_currency_name", typeCurrency },
                {"value", amountSpend},
                {"item_name", itemName}
            };
            LogEvent("spend_virtual_currency", firebase_evt);
        }
        /// <summary>
        /// Earn Virtual Currency event.
        /// </summary>
        /// <param name="typeCurrency"></param> type currency of earn. Ex: gold, diamond,...
        /// <param name="amountEarn"></param> amount of earn
        /// <param name="source"></param> source of earn. Ex: shop, win game, daily bonus,...
        public void LogEarnCurrency(string typeCurrency, int amountEarn, string source)
        {
            var firebase_evt = new Dictionary<string, object>
            {
                {"virtual_currency_name", typeCurrency },
                {"value", amountEarn},
                {"source", source}
            };
            LogEvent("earn_virtual_currency", firebase_evt);
        }
        #endregion
    }
}

[System.Serializable]
public class UnityStringEvent : UnityEvent<string>
{
}
[System.Serializable]
public class ItemIAP
{
    public string productIdIAP;
    public double localizedPrice;
    public string localizedPriceString;
    public string localizedCurrencyCode;
    public bool isNonConsume;

    public ItemIAP(string _productId, bool _isNonConsume)
    {
        productIdIAP = _productId;
        isNonConsume = _isNonConsume;
    }

    public void AddStoreData(double _price, string _priceString, string _currencyCode)
    {
        localizedPrice = _price;
        localizedPriceString = _priceString;
        localizedCurrencyCode = _currencyCode;
    }
}


