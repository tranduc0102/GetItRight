using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ItemShopManager : Singleton<ItemShopManager>
    {
        [SerializeField] private List<ItemInShop> itemSkins = new List<ItemInShop>();
        public List<ItemInShop> ItemSkins => itemSkins;
        [SerializeField] private List<ItemInShop> itemTheme = new List<ItemInShop>();
        public List<ItemInShop> ItemTheme => itemTheme;
        [SerializeField] private List<ItemInShop> itemChar = new List<ItemInShop>();
        public List<ItemInShop> ItemChar => itemChar;
    }
}
