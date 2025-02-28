using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

[CreateAssetMenu(fileName = "DataCharacter", menuName = "DataCharacter")]
public class DataCharacter : ScriptableObject
{
    public List<Transform> characters;
}