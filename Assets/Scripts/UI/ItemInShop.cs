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
        public float Chance;
        [HideInInspector] public float AccumulatedWeight;
        public int ID;
        public bool Lock
        {
            get => PlayerPrefs.GetInt($"UnLock: Type {type}, ID {ID}" , 1) == 1;
            set
            {
                PlayerPrefs.SetInt($"UnLock: Type {type}, ID {ID}", value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }
        public void LoadLock()
        {
            if (unclock && Lock)
            {
                Debug.Log($"UnLock: Type {type}, ID {ID}");
                Lock = false;
            }
        }

        public void ActionEuqip(ref int updateChange)
        {
            if(Lock)return;
            updateChange = ID;
        }
    }
}
