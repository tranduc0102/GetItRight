using ACEPlay.Bridge;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.UI
{
    public class UIController : Singleton<UIController>
    {
        public UIHome UIHome => FindObjectOfType<UIHome>();
        public UIIngame UIInGame => FindObjectOfType<UIIngame>();
        public UILevelFailed UILevelFailed => FindObjectOfType<UILevelFailed>();
        public UIPopupLevelBonus UIPopupLevelBonus => FindObjectOfType<UIPopupLevelBonus>();
        public UIPopupRemoveAds UIPopupRemoveAds => FindObjectOfType<UIPopupRemoveAds>();
        public UIShop UIShop => FindObjectOfType<UIShop>();
        public UISetting UISetting => FindObjectOfType<UISetting>();
        public UIWin UIWin => FindObjectOfType<UIWin>();
        
        [HideInInspector] public string keyRemoveAds;


        protected override void Awake()
        {
            base.KeepAlive(false);
            base.Awake();

            keyRemoveAds = $"{Application.identifier}_removeads";
        }

        public void OnButtonCloseBannerClick()
        {
            UnityEvent e = new UnityEvent();
            e.AddListener(delegate
            {
                UIPopupRemoveAds.DisplayRemovePopup(true);
            });
            
            BridgeController.instance.ShowInterstitial("button_close_banner", e);
        }
    }
}