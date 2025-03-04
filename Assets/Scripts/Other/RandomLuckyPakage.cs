using System;
using System.Collections.Generic;
using UnityEngine;
public class RandomLuckPakage : MonoBehaviour
{
    [Serializable]
    public class PakagePiece
    {
        public string Name;
        public Transform Transform;
        public float Chance;
        [HideInInspector] public float AccumulatedWeight;
    }
    
    public PakagePiece[] pakagePieces;
    private float totalWeight;
    private System.Random rand = new System.Random();

    void Start()
    {
        CalculateWeights();
    }
    private void CalculateWeights()
    {
        totalWeight = 0;
        foreach (var piece in pakagePieces)
        {
            totalWeight += piece.Chance;
            piece.AccumulatedWeight = totalWeight;
        }
    }

    private int GetRandomPieceIndex()
    {
        float r = (float)(rand.NextDouble() * totalWeight);
        for (int i = 0; i < pakagePieces.Length; i++)
        {
            if (pakagePieces[i].AccumulatedWeight >= r)
                return i;
        }
        return 0;
    }
    private void TestRandomSystem(int testCount)
    {
        Dictionary<string, int> results = new Dictionary<string, int>();
        foreach (var piece in pakagePieces) results[piece.Name] = 0;

        for (int i = 0; i < testCount; i++)
        {
            int index = GetRandomPieceIndex();
            results[pakagePieces[index].Name]++;
        }

        foreach (var result in results)
        {
            float percentage = (result.Value / (float)testCount) * 100f;
            Debug.Log($"{result.Key}: {percentage}%");
        }
    }

}
