using System;
using System.Collections.Generic;
using _Scripts.Extension;
using ACEPlay.Bridge;
using DanielLochner.Assets.SimpleScrollSnap;
using DG.Tweening;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIShop : MonoBehaviour
    {
        [SerializeField] private SimpleScrollSnap _simpleScrollSnapSkin;
        [SerializeField] private SimpleScrollSnap _simpleScrollSnapTheme;
        [SerializeField] private SimpleScrollSnap _simpleScrollSnapChar;
 
        private int currentSkin = 0;
        [SerializeField] private Button buttonEquip;

        [SerializeField] private TextMeshProUGUI txtBtnEquip;
        
        [SerializeField] private UIAppear shopPanel;
        [SerializeField] private UIAppear inventoryTab;
        [SerializeField] private UIAppear shopTab;
        
        [SerializeField] private UIAppear popupGetMoreCoin;
        
        [SerializeField] private List<GameObject> inventoryTabs = new List<GameObject>();
        [SerializeField] private GameObject currentTab;
        [SerializeField] private List<Image> tabButtons = new List<Image>();
        [SerializeField] private Image currentButtonTab;
        
        [SerializeField] private Color32 tabColorUnSelected;
        [SerializeField] private Color32 tabColorSelected;
        public void DisplayShop(bool enable)
        {
            if (enable)
            {
                OnButtonTabClick(0);
                shopPanel.gameObject.SetActive(true);
            }
            else
            {
                shopTab.Close(delegate
                {
                    shopPanel._Close(true);
                });
                inventoryTab._Close();
            }
        }

        public void DisplayPopupGetMoreCoin(bool enable)
        {
            if (enable)
            {
                popupGetMoreCoin.gameObject.SetActive(true);
            }
            else
            {
                popupGetMoreCoin._Close(true);
            }
        }

        public void OnButtonTabClick(int index)
        {
            switch (index)
            {
                case 0:
                    if (currentButtonTab != null)
                    {
                        currentButtonTab.DOColor(tabColorUnSelected, 0.3f);
                    }
                    tabButtons[0].DOColor(tabColorSelected, 0.3f);
                    currentButtonTab = tabButtons[0];

                    if (currentTab != null)
                    {
                        currentTab.SetActive(false);
                    }
                    inventoryTabs[0].SetActive(true);
                    currentTab = inventoryTabs[0]; 
                    _simpleScrollSnapSkin.SelectedPanel = GameController.Instance.CurrentSkin;
                    _simpleScrollSnapSkin.StartingPanel = GameController.Instance.CurrentSkin;
                    UpdateIndex(  _simpleScrollSnapSkin.SelectedPanel,   _simpleScrollSnapSkin.SelectedPanel + 1);
                    break;
                case 1:
                    if (currentButtonTab != null)
                    {
                        currentButtonTab.DOColor(tabColorUnSelected, 0.3f);
                    }
                    tabButtons[1].DOColor(tabColorSelected, 0.3f);
                    currentButtonTab = tabButtons[1];
                    
                    if (currentTab != null)
                    {
                        currentTab.SetActive(false);
                    }
                    inventoryTabs[1].SetActive(true);
                    currentTab = inventoryTabs[1];
                    _simpleScrollSnapTheme.SelectedPanel = GameController.Instance.CurrentTheme;
                    _simpleScrollSnapTheme.StartingPanel = GameController.Instance.CurrentTheme;
                    UpdateIndex( _simpleScrollSnapTheme.SelectedPanel,  _simpleScrollSnapTheme.SelectedPanel + 1);
                    break;
                case 2:
                    if (currentButtonTab != null)
                    {
                        currentButtonTab.DOColor(tabColorUnSelected, 0.3f);
                    }
                    tabButtons[2].DOColor(tabColorSelected, 0.3f);
                    currentButtonTab = tabButtons[2];
                    
                    if (currentTab != null)
                    {
                        currentTab.SetActive(false);
                    }
                    inventoryTabs[2].SetActive(true);
                    currentTab = inventoryTabs[2];
                    _simpleScrollSnapChar.SelectedPanel = GameController.Instance.CurrentPlayer;
                    _simpleScrollSnapChar.StartingPanel = GameController.Instance.CurrentPlayer;
                    UpdateIndex( _simpleScrollSnapChar.SelectedPanel,  _simpleScrollSnapChar.SelectedPanel + 1);
                    break;
            }
        }
        public void OnButtonClaimRewardsClick()
        {
            //goi qc reward xong tang coin => 200 chac the
            if (BridgeController.instance.IsRewardReady())
            {
                UnityEvent e = new UnityEvent();
                e.AddListener(delegate
                {
                    GameController.Instance.AmountCoin += 200;
                    UIController.instance.UIHome.UpdateTextCoin(GameController.Instance.AmountCoin);
                });
                BridgeController.instance.ShowRewarded("Reward Coin", e);
            }
        }

        public void OnButtonBuyBlindBoxClick(int index)
        {
            switch (index)
            {
                case 0:
                    //tru tien va mo blind box
                    if (GameController.Instance.AmountCoin < 200)
                    {
                        return;
                    }
                    if (ItemShopManager.instance.BlindBags[index].ActiveAnimation(index))
                    {
                        GameController.Instance.AmountCoin -= 200;
                        UIController.instance.UIHome.UpdateTextCoin(GameController.Instance.AmountCoin);
                    }
                    break;
                case 1:
                    //tru tien va mo blind box
                    if(GameController.Instance.AmountCoin < 200) return;
                    if (ItemShopManager.instance.BlindBags[index].ActiveAnimation(index))
                    {
                        GameController.Instance.AmountCoin -= 200;
                        UIController.instance.UIHome.UpdateTextCoin(GameController.Instance.AmountCoin);
                    }
                    break;
                case 2:
                    //tru tien va mo blind box
                    if(GameController.Instance.AmountCoin < 200) return;
                    if (ItemShopManager.instance.BlindBags[index].ActiveAnimation(index))
                    {
                        GameController.Instance.AmountCoin -= 200;
                        UIController.instance.UIHome.UpdateTextCoin(GameController.Instance.AmountCoin);
                    }
                    break;
            }
        }
        
        private ItemInShop item;
        public void UpdateIndex(int current, int next)
        {
            List<ItemInShop> currentItems = null;
            int equippedIndex = -1;
            currentSkin = current;
            if (inventoryTabs[0].activeSelf)
            {
                currentItems = ItemShopManager.instance.ItemSkins;
                equippedIndex = GameController.Instance.CurrentSkin;
            }
            else if (inventoryTabs[1].activeSelf)
            {
                currentItems = ItemShopManager.instance.ItemTheme;
                equippedIndex = GameController.Instance.CurrentTheme;
            }
            else if (inventoryTabs[2].activeSelf)
            {
                currentItems = ItemShopManager.instance.ItemChar;
                equippedIndex = GameController.Instance.CurrentPlayer;
            }

            if (currentItems == null || current + 1 < 0 || current >= currentItems.Count || txtBtnEquip == null) return;

            item = currentItems[current];
            if (item == null) return;

            if (item.Lock)
            {
                txtBtnEquip.text = "Locked";
                buttonEquip.interactable = false;
                txtBtnEquip.color = Color.gray;
            }
            else
            {
                if (currentSkin == equippedIndex)
                {
                    buttonEquip.interactable = false;
                    txtBtnEquip.text = "Equipped";
                }
                else
                {
                    buttonEquip.interactable = true;
                    txtBtnEquip.text = "Equip";
                }
                txtBtnEquip.color = Color.white;
            }
        }

        public void OnButtonEquipClick()
        {
            //xu li equip o day
            //chac la se gan id cho tung item
            int value = -1;
            
            if (inventoryTabs[0].activeSelf)
            {
                item = ItemShopManager.instance.ItemSkins[currentSkin];
                item.ActionEuqip(ref value);
                if(value < 0) return;
                GameController.Instance.CurrentSkin = value;
            } else if (inventoryTabs[1].activeSelf)
            {
                item = ItemShopManager.instance.ItemTheme[currentSkin];
                item.ActionEuqip(ref value);
                if(value < 0) return;
                GameController.Instance.CurrentTheme = value;
            }
            else if (inventoryTabs[2].activeSelf)
            {
                item = ItemShopManager.instance.ItemChar[currentSkin];
                item.ActionEuqip(ref value);
                if(value < 0) return;
                GameController.Instance.CurrentPlayer = value;
            }
            UpdateIndex(currentSkin, currentSkin + 1);
        }
    }
}