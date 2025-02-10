using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public enum EnumAnswer
    {
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
    }

}