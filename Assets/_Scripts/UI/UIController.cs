using System;
using UnityEngine;
using UnityEngine.Serialization;

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

        public void OnButtonCloseBannerClick()
        {
            UIPopupRemoveAds.DisplayRemovePopup(true);
        }
    }
}