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
        Skin
    }
    public class ItemInShop : MonoBehaviour
    {
        [SerializeField] private Button btn;
        [SerializeField] private TypeItemInShop type;
        public int ID;

        private void OnValidate()
        {
            btn = GetComponent<Button>();
        }

        private void Start()
        {
            btn.onClick.AddListener(ActionClick);
        }

        private void ActionClick()
        {
            if (type == TypeItemInShop.Theme)
            {
                GameController.Instance.CurrentTheme = ID;
            }
            else
            {
                ///
            }
        }
    }
}
