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
    [SerializeField] private float frameDuration = 0.1f;
    [SerializeField] private float frameDelayDuration = 0.1f;
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
        StartCoroutine(ResetStateAfterDelay(duration, state));
    }

    private IEnumerator ResetStateAfterDelay(float delay, StateFace state)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log(delay);
        Debug.Log("Reset State After Delay");
        this.State = StateFace.Idle;
    }
    private void Update()
    {
        if(canStop && State == StateFace.Idle) return;
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= frameDuration + Time.deltaTime * 2)
        {
            if (State != StateFace.ThatBai)
            {
                if (State == StateFace.Idle)
                {
                    spriteMaterial.mainTextureOffset = new Vector2(frameOffsetX * currentFrame, frameIdleOffsetY);
                }  
                if (State == StateFace.DoanDung)
                {
                    spriteMaterial.mainTextureOffset = new Vector2(frameOffsetX * currentFrame, frameHappyOffsetY);
                }
                if (State == StateFace.DoanSai)
                {
                    spriteMaterial.mainTextureOffset = new Vector2(frameOffsetX * currentFrame, frameAngryOffsetY);
                }  
                if (State == StateFace.Win)
                {
                    spriteMaterial.mainTextureOffset = new Vector2(frameOffsetX * currentFrame, frameWinOffsetY);
                } 
                if (currentFrame >= 4)
                {
                    frameDirection = -1;
                }
                if (currentFrame <= 0)
                {
                    frameDirection = 1;
                    counter += 1;
                    if (counter >= 1 && State == StateFace.Idle)
                    {
                        canStop = true;
                        DOVirtual.DelayedCall(frameDelayDuration, delegate
                        {
                            canStop = false;
                            counter = 0;
                        });   
                    }
                }
            }
            else
            {
                spriteMaterial.mainTextureOffset = new Vector2(frameOffsetX * currentFrame, frameLoseOffsetY);
                if (currentFrame >= 2)
                {
                    frameDirection = -1;
                }
                if (currentFrame <= 0)
                {
                    frameDirection = 1;
                }
            }
            currentFrame += frameDirection;
            elapsedTime -= frameDuration;
        }
    }
}