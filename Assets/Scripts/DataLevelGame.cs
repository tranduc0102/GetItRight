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
        public string name;
        public List<EnumAnswer> answers = new List<EnumAnswer>();
        
        public int amountSameEnumAnswers;
        public int AmountAnswers => answers.Count;
    }
}
