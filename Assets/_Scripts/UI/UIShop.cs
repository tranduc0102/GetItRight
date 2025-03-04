using System;
using System.Collections.Generic;
using _Scripts.Extension;
using DG.Tweening;
using Game;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIShop : MonoBehaviour
    {
        
        [SerializeField] private List<ItemInShop> itemSkins = new List<ItemInShop>();
        [SerializeField] private List<ItemInShop> itemTheme = new List<ItemInShop>();
        [SerializeField] private List<ItemInShop> itemChar = new List<ItemInShop>();

        private int IndexCurrentSkin = 0;
        private int IndexCurrentTheme = 0;
        private int IndexCurrentChar = 0;

        private int CurrentIndex
        {
            get
            {
                if (inventoryTabs[0].activeSelf)
                {
                    return IndexCurrentSkin;
                }
                if (inventoryTabs[1].activeSelf)
                {
                    return IndexCurrentTheme;
                }
                if (inventoryTabs[2].activeSelf)
                {
                    return IndexCurrentChar;
                }
                return 0;
            }
            set
            { 
                if (inventoryTabs[0].activeSelf)
                {
                    if (value >= itemSkins.Count)
                    {
                        value = 0;
                    }
                    if (value < 0)
                    {
                        value = itemSkins.Count - 1;
                    }
                    IndexCurrentSkin = value;
                }
                if (inventoryTabs[1].activeSelf)
                {
                    if (value >= itemTheme.Count)
                    {
                        value = 0;
                    }
                    if (value < 0)
                    {
                        value = itemTheme.Count - 1;
                    }
                    IndexCurrentTheme = value;
                }
                if (inventoryTabs[2].activeSelf)
                {
                    if (value >= itemChar.Count)
                    {
                        value = 0;
                    }
                    if (value < 0)
                    {
                        value = itemChar.Count - 1;
                    }
                    IndexCurrentChar = value;
                }
            }
        }
        
        [SerializeField] private UIAppear shopPanel;
        [SerializeField] private UIAppear inventoryTab;
        [SerializeField] private UIAppear shopTab;
        
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
                    break;
            }
        }
        public void OnButtonClaimRewardsClick()
        {
            //goi qc reward xong tang coin => 200 chac the
        }

        public void OnButtonBuyBlindBoxClick(int index)
        {
            switch (index)
            {
                case 0:
                    //tru tien va mo blind box
                    break;
                case 1:
                    //tru tien va mo blind box
                    break;
                case 2:
                    //tru tien va mo blind box
                    break;
            }
        }
        public void UpdateIndex(int count)
        {
            CurrentIndex += count;
        }
        public void OnButtonEquipClick()
        {
            //xu li equip o day
            //chac la se gan id cho tung item
            int value = -1;
            if (inventoryTabs[0].activeSelf)
            {
                itemSkins[IndexCurrentSkin].ActionEuqip(ref value);
                if(value < 0) return;
                GameController.Instance.CurrentSkin = value;
            }
            if (inventoryTabs[1].activeSelf)
            {
                itemTheme[IndexCurrentTheme].ActionEuqip(ref value);
                if(value < 0) return;
                GameController.Instance.CurrentTheme = value;
            }
            if (inventoryTabs[2].activeSelf)
            {
                itemChar[IndexCurrentChar].ActionEuqip(ref value);
                if(value < 0) return;
                GameController.Instance.CurrentPlayer = value;
            }
        }
    }
}