using System;
using UnityEngine;
using DG.Tweening;

public class FadeWithPropertyBlock : MonoBehaviour
{
    private Renderer objRenderer;

    public bool IsFade = true;
    public bool canFade = true;

    void OnEnable()
    {
        objRenderer = GetComponent<Renderer>();
        if (IsFade)
        {
            SetOpacity(0.2f);
            Debug.Log("Ok");
        }
    }

    public void SetOpacity(float alpha)
    {
        Color currentColor =  objRenderer.material.color;
        objRenderer.material.SetColor("_Color", new Color(currentColor.r, currentColor.g, currentColor.b, alpha));
    }

    public void FadeIn(float duration)
    {
        if(!canFade)return;
        float startAlpha = 0f;
        float targetAlpha = 1f;

        DOTween.To(() => startAlpha, x =>
        {
            startAlpha = x;
            SetOpacity(startAlpha);
        }, targetAlpha, duration);
    }
}