using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public enum TypeItemInShop
    {
        Theme,
        Skin,
        Character
    }
    public class ItemInShop : MonoBehaviour
    {
        [SerializeField] private Button btn;
        [SerializeField] private TypeItemInShop type;
        [SerializeField] private bool unclock;
        public int ID;
        public bool UnLock
        {
            get => PlayerPrefs.GetInt($"UnLock: Type {type}, ID {ID}" , 0) == 1;
            set
            {
                PlayerPrefs.SetInt($"UnLock: Type {type}, ID {ID}", value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }
        
        private void Start()
        {
            if (unclock && !UnLock)
            {
                UnLock = true;
            }
        }

        public void ActionEuqip(ref int updateChange)
        {
            if(UnLock)return;
            updateChange = ID;
        }
    }
}
