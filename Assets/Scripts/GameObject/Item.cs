using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Game
{
    public enum EnumAnswer
    {
        None,
        Zero,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
    }
    public class Item : MonoBehaviour
    {
        [SerializeField] private EnumAnswer _answer;
        public EnumAnswer Answer => _answer;
        public Transform Parent
        {
            get; private set;
        }
        private void Start()
        {
            Parent = transform.parent;
        }
        private void OnDestroy()
        {
            transform.DOKill();
        }
    }

}