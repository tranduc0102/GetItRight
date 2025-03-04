using System;
using UnityEngine;
using UnityEngine.UI;

public class ChangeBackground : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite[] backgroundSprites;
   
    public void ApplyBackground(int id)
    {
        backgroundImage.sprite = backgroundSprites[id];
    }
}
