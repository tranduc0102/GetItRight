using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class LevelData
{
    public string difficultyLevel;
    public int level;
    public int amountBox;
    public int amountLine;
    public int amountCan;
    public int amountDistinct;
}

[System.Serializable]
public class LevelCollection
{
    public List<LevelData> levels = new List<LevelData>();
}

public class CreateLevel : Singleton<CreateLevel>
{
    private string filePath;
    private LevelCollection levelCollection = new LevelCollection();

    protected override void Awake()
    {
        base.Awake();
        filePath = Path.Combine(Application.persistentDataPath, "levelData.json");
        Debug.Log("File Path: " + filePath);
        LoadData();
    }
    private void LoadData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            levelCollection = JsonUtility.FromJson<LevelCollection>(json);
            Debug.Log("Data Loaded from persistentDataPath: " + json);
        }
        else
        {
            TextAsset jsonFile = Resources.Load<TextAsset>("DataLevel/levelData");
            if (jsonFile != null)
            {
                levelCollection = JsonUtility.FromJson<LevelCollection>(jsonFile.text);
                Debug.Log("Data Loaded from Resources: " + jsonFile.text);

                File.WriteAllText(filePath, jsonFile.text);
                Debug.Log("Copied levelData.json to persistentDataPath for future modifications.");
            }
            else
            {
                Debug.LogError("Failed to load levelData.json from Resources!");
                levelCollection = new LevelCollection();
            }
        }
    }

    public void SaveData(string difficultyLevel, LevelData levelData)
    {
        if (levelData == null)
        {
            Debug.LogError("SaveData: levelData is null!");
            return;
        }
        levelData.difficultyLevel = difficultyLevel;
        LevelData existingLevel = levelCollection.levels.Find(l => l.difficultyLevel == levelData.difficultyLevel && l.level == levelData.level);
        if (existingLevel != null)
        {
            existingLevel.amountBox = levelData.amountBox;
            existingLevel.amountLine = levelData.amountLine;
            existingLevel.amountCan = levelData.amountCan;
            Debug.Log("Same Level Found - Data Updated");
        }
        else
        {
            levelCollection.levels.Add(levelData);
            Debug.Log("New Level Added");
        }

        try
        {
            string json = JsonUtility.ToJson(levelCollection, true);
            File.WriteAllText(filePath, json);
            Debug.Log("Data Saved Successfully: " + json);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to save data: " + ex.Message);
        }
    }
    public LevelData GetLevelData(int levelID)
    {
        Debug.Log("GetLevelData: " + levelID);
        return levelCollection.levels.Find(l => l.level == levelID);
    }
    public LevelData GenerateRandomLevel(int levelID)
    {
        int amountBox = Random.Range(3, 8);
        int amountLine = Random.Range(3, 6);
        int amountDistinct = Random.Range(1, amountBox + 1);
        int amountCan = Random.Range(Mathf.Max(amountDistinct,3), 7);
        Debug.LogWarning(amountDistinct);
        LevelData newLevel = new LevelData
        {
            level = levelID,
            amountBox = amountBox,
            amountLine = amountLine,
            amountCan = amountCan,
            amountDistinct = amountDistinct
        };

        return newLevel;
    }
    public void DeleteData()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Data file deleted.");
        }

        levelCollection = new LevelCollection(); // Xóa dữ liệu trong bộ nhớ
    }

}
