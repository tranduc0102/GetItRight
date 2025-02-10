using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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
        public int AmountAnswers => answers.Count;
    }
}
