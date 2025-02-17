using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "DataLevelGame", menuName = "DataLevelGame")]
    public class DataLevelGame : ScriptableObject
    {
        public List<Level> levels;
    }
    [Serializable]
    public class Level
    {
        public string name = "level 1";
        public int amountSameValue = 1;
        public int amountAnswers = 3;
        public int amountMove = 15;
        public int amountValueRemain => amountAnswers - amountSameValue;
    }
}
