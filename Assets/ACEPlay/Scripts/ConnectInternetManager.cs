using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace TA
{
    public class ConnectInternetManager : MonoBehaviour
    {
        public static ConnectInternetManager instance;

        private bool isBusy;
        private bool isFirst;

        public bool isOnline;
        public Action onConnectInternet;
        public Action onDisconnectInternet;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);

                DOVirtual.DelayedCall(1f, delegate
                {
                    if (!isFirst) Check();
                });
            }
            else Destroy(this);
        }

        private void OnApplicationFocus(bool focus)
        {
            if (SceneManager.GetActiveScene().buildIndex == 0) return;
            if (focus)
            {
                Check();
            }
        }

        public void Check()
        {
            if (isFirst)
            {
                if (IsInvoking())
                    CancelInvoke();
            }
            isFirst = true;
#if UNITY_ANDROID && !UNITY_EDITOR
                var wifiStatus = DeviceWifiAndroidRequest();
                //bool isConnecting = NetworkConnectionAndroidRequest();
                bool isConnecting = IsOnline();

                Debug.Log($"wifiStatus: {wifiStatus.ToString()}   isConnecting: {isConnecting}");
                if (isConnecting && wifiStatus != WifiStatus.WIFI_STATE_DISABLING)
                {
                    isOnline = true;
                    if (onConnectInternet != null) onConnectInternet.Invoke();
                }
                else
                {
                    isOnline = false;
                    if (onDisconnectInternet != null) onDisconnectInternet.Invoke();
                    Invoke(nameof(Check), 15);
                }
#else
            //CheckInternetWithUrl("https://www.amazon.com");
            if (Application.internetReachability != NetworkReachability.NotReachable && IsOnline())
            {
                isOnline = true;
                if (onConnectInternet != null) onConnectInternet.Invoke();
            }
            else
            {
                isOnline = false;
                if (onDisconnectInternet != null) onDisconnectInternet.Invoke();
                Invoke(nameof(Check), 15);
            }
#endif
        }

        public void CheckInternet(Action onConnected, Action onDisconnect)
        {
            if (isFirst)
            {
                if (IsInvoking())
                    CancelInvoke();
            }
            isFirst = true;
#if UNITY_ANDROID && !UNITY_EDITOR
            var wifiStatus = DeviceWifiAndroidRequest();
            bool isConnecting = IsOnline();

            Debug.Log($"wifiStatus: {wifiStatus.ToString()}   isConnecting: {isConnecting}");
            if (isConnecting && wifiStatus != WifiStatus.WIFI_STATE_DISABLING)
            {
                isOnline = true;
                if (onConnected != null) onConnected.Invoke();
                if (onConnectInternet != null) onConnectInternet.Invoke();
            }
            else
            {
                isOnline = false;
                if (onDisconnect != null) onDisconnect.Invoke();
                if (onDisconnectInternet != null) onDisconnectInternet.Invoke();
                Invoke(nameof(Check), 15);
            }
#else
            if (Application.internetReachability != NetworkReachability.NotReachable && IsOnline())
            {
                isOnline = true;
                if (onConnected != null) onConnected.Invoke();
                if (onConnectInternet != null) onConnectInternet.Invoke();
            }
            else
            {
                isOnline = false;
                if (onDisconnect != null) onDisconnect.Invoke();
                if (onDisconnectInternet != null) onDisconnectInternet.Invoke();
                Invoke(nameof(Check), 15);
            }
#endif
        }

        private bool IsOnline()
        {
            int index = 0;
            float startTime = Time.realtimeSinceStartup;
        Again:
            try
            {
                IPHostEntry iPHostEntry = Dns.GetHostEntry(MyTime.instance.GetHostNameOrAddress(index));
                return true;
            }
            catch (SocketException ex)
            {
                Debug.LogError("HAhahahahah: " + ex.Message);
                if (Time.realtimeSinceStartup - startTime > 5)
                {
                    Debug.LogError("Time Out!");
                    return false;
                }
                index++;
                if (index < 10) goto Again;
            }
            return false;
        }

        //private void CheckInternetWithUrl(string url)
        //{
        //    isBusy = true;
        //    TA.Extension.CheckInternet(this, url,
        //        () =>
        //        {
        //            isBusy = false;
        //            isOnline = true;
        //            if (onConnectInternet != null) onConnectInternet.Invoke();
        //        },
        //        () =>
        //        {
        //            isBusy = false;
        //            isOnline = false;
        //            if (onDisconnectInternet != null) onDisconnectInternet.Invoke();
        //        },
        //        10,
        //        () =>
        //        {
        //            isBusy = false;
        //            isOnline = false;
        //            CheckInternetWithUrl(url);
        //        });
        //}

        public static int getAPI_Level()
        {
            using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                return version.GetStatic<int>("SDK_INT");
            }
        }

        ///
        /// Send request to JavaClass to check if wifi is enabled on oculus
        ///
        public WifiStatus DeviceWifiAndroidRequest()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            Debug.Log($"API Level: {getAPI_Level()}");
#endif
            //var isWifiEnabled = false;
            WifiStatus wifiStatus = WifiStatus.WIFI_STATE_DISABLED;
            try
            {
                // An activity provides the window where the app draws its UI.
                // Typically, one activity in an app is specified as the main activity, which is the first screen to appear when the user launches the app.
                // Get current activity in unity project from java classes.

                // AndroidJavaClass: Construct an AndroidJavaClass from the class name.
                // GetStatic: Get the value of a static field in an object type.
                using (var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    // Method "getSystemService": Gets the name of the system-level service that is represented by the specified class.
                    // As the parameter arguments, we pass the name of the package with handle "getSystemService".
                    // The class of the returned object varies by the requested name.

                    // Get "WifiManager" for management of Wi-Fi connectivity.
                    using (var wifiManager = activity.Call<AndroidJavaObject>("getSystemService", "wifi"))
                    {
                        // Call: Calls a Java method on an object. Call, T - return type
                        // Call method "isWifiEnabled". Return whether Wi-Fi is enabled or disabled. Returns bool value.
                        //isWifiEnabled = wifiManager.Call<bool>("isWifiEnabled");
                        wifiStatus = (WifiStatus)wifiManager.Call<int>("getWifiState");
                        //Debug.Log($"getWifiState: {wifiStatus.ToString()}");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Android request exception: " + e.Message);
            }

            //return isWifiEnabled;
            return wifiStatus;
        }

        public enum WifiStatus
        {
            WIFI_STATE_DISABLED = 1,
            WIFI_STATE_DISABLING = 0,
            WIFI_STATE_UNKNOWN = 4,
            WIFI_STATE_ENABLING = 2,
            WIFI_STATE_ENABLED = 3
        }
        ///
        /// Request to JavaClass to check if internet connection is reachable
        ///
        public static bool NetworkConnectionAndroidRequest()
        {
            var isNetworkEnabled = false;
            try
            {
                using (var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    // Get "ConnectivityManager" for handling management of network connections.
                    using (var connectivityManager = activity.Call<AndroidJavaObject>("getSystemService", "connectivity"))
                    {
                        // Call method "getActiveNetworkInfo" that returns details about the currently active default data network.
                        // Call method "isConnected" that indicates if network connectivity exists. Returns bool value.
                        isNetworkEnabled = connectivityManager.Call<AndroidJavaObject>("getActiveNetworkInfo").Call<bool>("isConnected");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Android request exception: " + e.Message);
            }
            return isNetworkEnabled;
        }


        /////
        ///// Check if the application is connected to Wi-Fi in coroutine
        /////
        //private IEnumerator CheckDeviceWifiConnection()
        //{
        //    yield return new WaitForSecondsRealtime(2f);
        //    while (true)
        //    {
        //        yield return new WaitForSecondsRealtime(checkDelay);
        //        wifiConnection = DeviceWiFiAndroidRequest();
        //        wifiMessage.text = wifiConnection ? "Wifi On" : "Wifi Off";
        //    }
        //}
        /////
        ///// Check if the application is connected to Wi-Fi in coroutine
        /////
        //private IEnumerator CheckNetworkConnection()
        //{
        //    yield return new WaitForSecondsRealtime(2f);
        //    while (true)
        //    {
        //        yield return new WaitForSecondsRealtime(checkDelay);
        //        internetConnection = NetworkConnectionAndroidRequest();
        //        internetMessage.text = internetConnection ? "Internet reachable" : "Internet not reachable";
        //    }
        //}
    }
}