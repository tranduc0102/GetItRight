using UnityEngine;

namespace _Scripts.Extension
{
    public static class Vibration
    {
#if !UNITY_STANDALONE_WIN

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern bool _HasVibrator();

        [DllImport("__Internal")]
        private static extern void _Vibrate();

        [DllImport("__Internal")]
        private static extern void _VibratePop();

        [DllImport("__Internal")]
        private static extern void _VibratePeek();

        [DllImport("__Internal")]
        private static extern void _VibrateNope();

        [DllImport("__Internal")]
        private static extern void _impactOccurred(string style);

        [DllImport("__Internal")]
        private static extern void _notificationOccurred(string style);

        [DllImport("__Internal")]
        private static extern void _selectionChanged();
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#else
        public static AndroidJavaClass unityPlayer;
        public static AndroidJavaObject currentActivity;
        public static AndroidJavaObject vibrator;
#endif
        public static void Vibrate()
        {
            if (UI.UIController.instance.UISetting.IsOffVibration) return;
            if (isAndroid())
                vibrator.Call("vibrate");
            else
                Handheld.Vibrate();
        }

        public static void Vibrate(long milliseconds)
        {
            if (UI.UIController.instance.UISetting.IsOffVibration) return;
            if (isAndroid())
                vibrator.Call("vibrate", milliseconds);
            else
            {
#if UNITY_IOS
            _VibratePop();
#endif
            }
        }

        public static void Vibrate(long[] pattern, int repeat)
        {
            if (UI.UIController.instance.UISetting.IsOffVibration) return;
            if (isAndroid())
                vibrator.Call("vibrate", pattern, repeat);
            else
            {
#if UNITY_IOS
                _VibrateNope();
#endif
            }
        }

        public static bool HasVibrator()
        {
#if UNITY_ANDROID
            return isAndroid();
#elif UNITY_IOS
            return _HasVibrator();
#else
            return false;
#endif
        }

        public static void Cancel()
        {
            if (isAndroid())
                vibrator.Call("cancel");
        }

        public static bool isAndroid()
        {
            return Application.platform == RuntimePlatform.Android && !Application.isEditor;
        }
#else
        public static void Vibrate()
        {
            return;
        }
        public static void Vibrate(int value)
        {
            return;
        }
#endif

        ///<summary>
        /// Tiny pop vibration
        ///</summary>
        public static void VibratePop()
        {
            if (Application.isMobilePlatform)
            {
                if (UI.UIController.instance.UISetting.IsOffVibration) return;
#if UNITY_IOS
                _VibratePop ();
#elif UNITY_ANDROID
                Vibrate(50);
#endif
            }
        }
        ///<summary>
        /// Small peek vibration
        ///</summary>
        public static void VibratePeek()
        {
            if (Application.isMobilePlatform)
            {
                if (UI.UIController.instance.UISetting.IsOffVibration) return;
#if UNITY_IOS
                _VibratePeek ();
#elif UNITY_ANDROID
                Vibrate(100);
#endif
            }
        }
        ///<summary>
        /// 3 small vibrations
        ///</summary>
        public static void VibrateNope()
        {
            if (Application.isMobilePlatform)
            {
                if (UI.UIController.instance.UISetting.IsOffVibration) return;
#if UNITY_IOS
                _VibrateNope ();
#elif UNITY_ANDROID
                long[] pattern = { 0, 50, 50, 50 };
                Vibrate(pattern, -1);
#endif
            }
        }

        #region IOS Vibrate
        public static void VibrateIOS(ImpactFeedbackStyle style)
        {
#if UNITY_IOS
            _impactOccurred(style.ToString());
#endif
        }

        public static void VibrateIOS(NotificationFeedbackStyle style)
        {
#if UNITY_IOS
            _notificationOccurred(style.ToString());
#endif
        }

        public static void VibrateIOS_SelectionChanged()
        {
#if UNITY_IOS
            _selectionChanged();
#endif
        }
        #endregion
    }

    public enum ImpactFeedbackStyle
    {
        Heavy,
        Medium,
        Light,
        Rigid,
        Soft
    }

    public enum NotificationFeedbackStyle
    {
        Error,
        Success,
        Warning
    }
}