using System;
using System.Collections.Generic;
using UnityEngine;
public class RandomLuckPakage : MonoBehaviour
{
    [Serializable]
    public class PakagePiece
    {
        public string Name;
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

    public int GetRandomPieceIndex()
    {
        float r = (float)(rand.NextDouble() * totalWeight);
        for (int i = 0; i < pakagePieces.Length; i++)
        {
            if (pakagePieces[i].AccumulatedWeight >= r)
                return i;
        }
        return 0;
    }
}
