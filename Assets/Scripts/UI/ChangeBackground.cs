using System;
using UnityEngine;
using UnityEngine.UI;

public class ChangeBackground : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite[] backgroundSprites;
    private int index = 0;
    public void ApplyBackground(Sprite sprite)
    {
        backgroundImage.sprite = sprite;
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ApplyBackground(backgroundSprites[index]);
            index = (index + 1) % backgroundSprites.Length;
        }
    }
}
