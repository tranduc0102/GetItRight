using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ACEPlay.Native
{
    public class NativeAds : MonoBehaviour
    {
        public static NativeAds instance;
#if UNITY_ANDROID
        [SerializeField] string adUnitId = "ca-app-pub-6632644511740977/6806026708";
        string idTest = "ca-app-pub-3940256099942544/2247696110";
#elif UNITY_IOS
        string adUnitId = "ca-app-pub-66326445117409T77/2493351992";
        string idTest = "ca-app-pub-3940256099942544/3986624511";
#endif

        [Header("UI Native")]
        [SerializeField] private bool isIconNative;
        [SerializeField] private GameObject nativeAds;
        [SerializeField] private RawImage rawMainImage, rawIconImage;
        [SerializeField] private TextMeshProUGUI txtTitle;
        [SerializeField] private TextMeshProUGUI txtDescription;
        [SerializeField] private TextMeshProUGUI txtButton;
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            nativeAds.SetActive(false);
        }

        public void DisplayNativeAds(bool enable)
        {
            nativeAds.SetActive(false);
        }
    }
}
