using System;
using _Scripts.Extension;
using ACEPlay.Bridge;
using TMPro;
using UnityEngine;

namespace _Scripts.UI
{
    public class UIPopupRemoveAds : MonoBehaviour
    {
        [SerializeField] private UIAppear removeAds;
        [SerializeField] private GameObject buttonRemoveAds;
        [SerializeField] private GameObject buttonCloseBanner;
        [SerializeField] private TextMeshProUGUI priceText;
        
        [SerializeField] private int showPopupCounter;
        public int showPopupTarget;

        private void Awake()
        {
            OnNonConsumable();
            showPopupCounter = 0;
        }

        private void Start()
        {
            LoadIAPPrice();
            DisplayRemovePopup(false);
        }
        
        public void DisplayRemovePopup(bool enable)
        {
            if (enable)
            {
                if (showPopupCounter < showPopupTarget)
                {
                    showPopupCounter++;
                }
                else
                {
                    if (OnNonConsumable())
                    {
                        removeAds.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                removeAds._Close(true);
            }
        }

        public void OnButtonRemoveClick()
        {
            UnityStringEvent e = new UnityStringEvent();
            e.AddListener(result =>
            {
                BridgeController.instance.CanShowAds = false;
                OnNonConsumable();
                BridgeController.instance.LogEvent(UIController.instance.keyRemoveAds);
                
                DisplayRemovePopup(false);
            });
            BridgeController.instance.PurchaseProduct(UIController.instance.keyRemoveAds, e);
        }
        
        public void OnButtonNoThanksClick()
        {
            DisplayRemovePopup(false);
        }
        
        private bool OnNonConsumable()
        {
            if (BridgeController.instance.CanShowAds || !BridgeController.instance.CheckOwnerNonConsumable(UIController.instance.keyRemoveAds))
            {
                buttonRemoveAds.SetActive(true);
                buttonCloseBanner.SetActive(true);
                
                return true;
            }

            buttonRemoveAds.SetActive(false);
            buttonCloseBanner.SetActive(false);
                
            return false;
        }
        
        private void LoadIAPPrice()
        {
            if (!string.IsNullOrEmpty(GetProductPrice(UIController.instance.keyRemoveAds)))
            {
                priceText.text = GetProductPrice(UIController.instance.keyRemoveAds);
            }
            else
            {
                priceText.text = "????";
            }
        }

        private string GetProductPrice(string iapKey)
        {
            if (BridgeController.instance.availableItemsIAP.Count != 0)
            {
                foreach (var product in BridgeController.instance.availableItemsIAP)
                {
                    if (iapKey.Equals(product.productIdIAP))
                    {
                        if (!string.IsNullOrEmpty(product.localizedPriceString))
                        {
                            return product.localizedPriceString;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return null;
        }
    }
}