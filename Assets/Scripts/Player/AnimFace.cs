using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum StateFace
{
    Idle,
    DoanDung,
    DoanSai,
    Win,
    ThatBai
}
public class AnimFace : MonoBehaviour
{
    [SerializeField] private Material spriteMaterial;
    [SerializeField] private float frameDuration = 0.08f;
    [SerializeField] private float frameDelayDuration = 1.8f;
    private int counter = 0;
    private bool canStop;
    private const float frameOffsetX = 0.1793f;
    private const float frameIdleOffsetY = 0f;
    private const float frameHappyOffsetY = -0.155f;
    private const float frameAngryOffsetY = -0.31f;
    private const float frameWinOffsetY = -0.469f;
    private const float frameOffsetY = -0.63f;
    private const float frameLoseOffsetY = -0.788f;
    private float elapsedTime;
    private int currentFrame = 0;
    private int frameDirection = 1;

    [SerializeField] private StateFace State;
    public void SetState(StateFace state, float duration)
    {
        this.State = state;
        counter = 0;
        currentFrame = 0;
        elapsedTime = 0;
        canStop = false;
        switch (State)
        {
            case StateFace.DoanDung:
                spriteMaterial.mainTextureOffset = new Vector2(frameOffsetX * currentFrame, frameHappyOffsetY);
                break;
            case StateFace.DoanSai:
                spriteMaterial.mainTextureOffset = new Vector2(frameOffsetX * currentFrame, frameAngryOffsetY);
                break;
            case StateFace.Win:
                spriteMaterial.mainTextureOffset = new Vector2(frameOffsetX * currentFrame, frameWinOffsetY);
                break;
            case StateFace.ThatBai:
                spriteMaterial.mainTextureOffset = new Vector2(frameOffsetX * currentFrame, frameLoseOffsetY);
                break;
        }
        StartCoroutine(ResetStateAfterDelay(duration, state));
    }

    private IEnumerator ResetStateAfterDelay(float delay, StateFace state)
    {
        yield return new WaitForSeconds(delay - delay * 0.15f);
        if(state == StateFace.Win || state == StateFace.ThatBai) yield break;
        counter = 0;
        currentFrame = 0;
        elapsedTime = 0;
        canStop = false;
        this.State = StateFace.Idle;
    }
   private Tween _delayTween;

private void Update()
{
    if (canStop) return;
    
    elapsedTime += Time.deltaTime;
    if (elapsedTime >= frameDuration + Time.deltaTime * 2)
    {
        if (State != StateFace.ThatBai)
        {
            switch (State)
            {
                case StateFace.Idle:
                    spriteMaterial.mainTextureOffset = new Vector2(frameOffsetX * currentFrame, frameIdleOffsetY);
                    break;
                case StateFace.DoanDung:
                    spriteMaterial.mainTextureOffset = new Vector2(frameOffsetX * currentFrame, frameHappyOffsetY);
                    break;
                case StateFace.DoanSai:
                    spriteMaterial.mainTextureOffset = new Vector2(frameOffsetX * currentFrame, frameAngryOffsetY);
                    break;
                case StateFace.Win:
                    spriteMaterial.mainTextureOffset = new Vector2(frameOffsetX * currentFrame, frameWinOffsetY);
                    break;
            }

            if (currentFrame >= 4) 
            {
                frameDirection = -1;
            }
        }
        else 
        {
            spriteMaterial.mainTextureOffset = new Vector2(frameOffsetX * currentFrame, frameLoseOffsetY);
            if (currentFrame >= 2)
            {
                frameDirection = -1;
            }
        }
        if (currentFrame <= 0 && frameDirection == -1) 
        {
            frameDirection = 1;

            if (State != StateFace.DoanSai) 
            {
                counter++;

                if (counter >= 1) 
                {
                    canStop = true;

                    _delayTween?.Kill();

                    _delayTween = DOVirtual.DelayedCall(frameDelayDuration, () =>
                    {
                        canStop = false;
                        counter = 0;
                    });
                }
            }
        }

        currentFrame += frameDirection;
        elapsedTime -= frameDuration;
    }
}

}