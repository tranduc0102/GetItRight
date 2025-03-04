using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

[CreateAssetMenu(fileName = "DataThemeObject", menuName = "DataThemeObject")]
public class DataThemeObject : ScriptableObject
{
    public List<ThemeData> themeData;
}
[Serializable]
public class ThemeData
{
    public string name;
    public List<Item> items;
}

