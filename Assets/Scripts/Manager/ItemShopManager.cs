using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using pooling;
using UnityEngine;

namespace Game
{
    public class ItemShopManager : Singleton<ItemShopManager>
    {
        [SerializeField] private AnimationOpenBlindBag[] _blindBags;
        public AnimationOpenBlindBag[] BlindBags => _blindBags;

        [SerializeField] private List<ItemInShop> itemSkins = new List<ItemInShop>();
        [SerializeField] private Transform itemSkinParent;
        public List<ItemInShop> ItemSkins => itemSkins;
        [SerializeField] private List<ItemInShop> itemTheme = new List<ItemInShop>();
        [SerializeField] private Transform itemThemeParent;
        public List<ItemInShop> ItemTheme => itemTheme;
        [SerializeField] private List<ItemInShop> itemChar = new List<ItemInShop>();
        [SerializeField] private Transform itemCharParent;
        public List<ItemInShop> ItemChar => itemChar;
        
        public bool CanBuyBag = true;
        private float totalWeight;
        private System.Random rand = new System.Random();
        private List<ItemInShop> CalculateWeights(List<ItemInShop> items)
        {
            totalWeight = 0;
            List<ItemInShop> shopItems = new List<ItemInShop>();
            foreach (var piece in items)
            {
                if (piece.Chance <= 0)
                {
                    piece.Chance = 100f / items.Count;
                }
            }

            foreach (var piece in items)
            {
                if (piece.Lock && piece.Chance > 0)
                {
                    totalWeight += piece.Chance;
                    piece.AccumulatedWeight = totalWeight;
                    shopItems.Add(piece);
                }
            }
            return shopItems;
        }
        private void Start()
        {
            foreach (ItemInShop item in itemTheme)
            {
                item.LoadLock();
            }
            foreach (ItemInShop item in itemSkins)
            {
                item.LoadLock();
            }  
            foreach (ItemInShop item in itemChar)
            {
                item.LoadLock();
            }
        }

        private ItemInShop GetRandomPieceIndex(List<ItemInShop> items)
        {
            List<ItemInShop> shopItems = CalculateWeights(items);
            if (shopItems.Count == 0) return null;

            float r = (float)(rand.NextDouble() * totalWeight);
            for (int i = 0; i < shopItems.Count; i++)
            {
                if (shopItems[i].AccumulatedWeight >= r && shopItems[i].Lock)
                {
                    return shopItems[i];
                }
            }
            return null;
        }
        public bool CheckCanSpawn(int index)
        {
            switch (index)
            {
                case 0:
                    if (GetRandomPieceIndex(itemSkins) != null)
                    {
                        return true;
                    }
                    break;
                case 1:
                    if (GetRandomPieceIndex(itemTheme) != null)
                    {
                        return true;
                    }
                    break;
                case 2:
                    if (GetRandomPieceIndex(itemChar) != null)
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }
        public void SpawnRandomItemInShop(int index, Transform parent = null)
        {
            ItemInShop t = null;
            switch (index)
            {
                case 0:
                    ItemInShop item1 = GetRandomPieceIndex(itemSkins);
                    if (item1 != null)
                    {
                        item1.GetComponent<CanvasGroup>().alpha = 1;
                        item1.Lock = false;
                        if (parent == null)
                        {
                            parent = itemSkinParent;
                        }
                        ResetObject(parent);
                        t = Instantiate(item1, itemSkinParent.position, itemSkinParent.rotation, parent);
                    }
                    break;
                case 1:
                    ItemInShop item2 = GetRandomPieceIndex(itemTheme);
                    if (item2 != null)
                    {
                        item2.GetComponent<CanvasGroup>().alpha = 1;
                        item2.Lock = false;
                        if (parent == null)
                        {
                            parent = itemThemeParent;
                        }
                        ResetObject(parent);
                        t = Instantiate(item2, itemThemeParent.position, Quaternion.identity, parent);   
                    }
                    break;
                case 2:
                    ItemInShop item3 = GetRandomPieceIndex(itemChar);
                    if (item3 != null)
                    {
                        item3.Lock = false;
                        if (parent == null)
                        {
                            parent = itemCharParent;
                        }
                        ResetObject(parent);
                        t = Instantiate(item3, itemCharParent.position, itemCharParent.rotation, parent);   
                    }
                    break;
            }
            if (t != null)
            {
                t.transform.localScale = Vector3.zero;
                t.transform.DOScale(Vector3.one, 0.7f);   
                t.gameObject.SetActive(true);
            }
        }
        public void ResetObject(Transform parent)
        {
            if (parent != null)
            {
                if (parent.childCount > 0)
                {
                    foreach (Transform item in parent)
                    {
                        Destroy(item.gameObject);
                    }
                }
            }
            if (itemCharParent.childCount > 0)
            {
                foreach (Transform item in itemCharParent)
                {
                    Destroy(item.gameObject);
                }
            }
            if (itemThemeParent.childCount > 0)
            {
                foreach (Transform item in itemThemeParent)
                {
                    Destroy(item.gameObject);
                }
            }
            if (itemSkinParent.childCount > 0)
            {
                foreach (Transform item in itemSkinParent)
                {
                    Destroy(item.gameObject);
                }
            }
            CanBuyBag = true;
        }
    }
}