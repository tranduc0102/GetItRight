using System;
using System.Collections;
using System.Collections.Generic;
using pooling;
using UnityEngine;

namespace Game
{
    public class ItemShopManager : Singleton<ItemShopManager>
    {
        [SerializeField] private List<ItemInShop> itemSkins = new List<ItemInShop>();
        [SerializeField] private Transform itemSkinParent;
        public List<ItemInShop> ItemSkins => itemSkins;
        [SerializeField] private List<ItemInShop> itemTheme = new List<ItemInShop>();
        [SerializeField] private Transform itemThemeParent;
        public List<ItemInShop> ItemTheme => itemTheme;
        [SerializeField] private List<ItemInShop> itemChar = new List<ItemInShop>();
        [SerializeField] private Transform itemCharParent;
        public List<ItemInShop> ItemChar => itemChar;
        
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

        private ItemInShop GetRandomPieceIndex(List<ItemInShop> items)
        {
            List<ItemInShop> shopItems = CalculateWeights(items);
            if (shopItems.Count == 0) return null;

            float r = (float)(rand.NextDouble() * totalWeight);
            for (int i = 0; i < shopItems.Count; i++)
            {
                if (shopItems[i].AccumulatedWeight >= r)
                {
                    shopItems[i].Lock = false;
                    return shopItems[i];
                }
            }
            return null;
        }
        public void SpawnRandomItemInShop(int index)
        {
            switch (index)
            {
                case 0:
                    if (itemSkinParent.childCount > 0)
                    {
                        foreach (Transform item in itemSkinParent)
                        {
                            PoolingManager.Despawn(item.gameObject);
                        }
                    }
                    ItemInShop item1 = GetRandomPieceIndex(itemSkins);
                    if (item1 != null)
                    {
                        item1.GetComponent<CanvasGroup>().alpha = 1;
                        PoolingManager.Spawn(item1, itemSkinParent.position, itemSkinParent.rotation, itemSkinParent);
                    }
                    break;
                case 1:
                    if (itemThemeParent.childCount > 0)
                    {
                        foreach (Transform item in itemThemeParent)
                        {
                            PoolingManager.Despawn(item.gameObject);
                        }
                    }
                    ItemInShop item2 = GetRandomPieceIndex(itemTheme);
                    if (item2 != null)
                    {
                        item2.GetComponent<CanvasGroup>().alpha = 1;
                        PoolingManager.Spawn(item2, itemThemeParent.position, Quaternion.identity, itemThemeParent);   
                    }
                    break;
                case 2:
                    if (itemCharParent.childCount > 0)
                    {
                        foreach (Transform item in itemCharParent)
                        {
                            PoolingManager.Despawn(item.gameObject);
                        }
                    }
                    ItemInShop item3 = GetRandomPieceIndex(itemChar);
                    if (item3 != null)
                    {
                        PoolingManager.Spawn(item3, itemCharParent.position, itemCharParent.rotation, itemCharParent);   
                    }
                    break;
            }
        }
    }
}