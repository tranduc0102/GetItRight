using System;
using DG.Tweening;
using UnityEngine;

namespace Game
{
    public enum EnumAnswer
    {
        None = 0,
        Zero = 1,
        One = 2,
        Two = 3,
        Three = 4,
        Four = 5,
        Five = 6,
        Six = 7,
        Seven = 8,
        Eight = 9,
        Nine = 10,
    }
    public class Item : MonoBehaviour
    {
        [SerializeField] private EnumAnswer _answer;
        public bool CanMove
        {
            get;
            set;
        }
        public EnumAnswer Answer => _answer;
        public Transform Parent
        {
            get; private set;
        }
        private void Start()
        {
            Parent = transform.parent;
        }
        private void OnEnable()
        {
            CanMove = true;
        }
        private void OnDestroy()
        {
            transform.DOKill();
        }
    }

}