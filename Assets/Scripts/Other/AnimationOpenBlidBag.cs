using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AnimationOpenBlindBag : MonoBehaviour
{
    [SerializeField] private RectTransform bag1;   
    [SerializeField] private RectTransform bag2;    
    
    [SerializeField] private float shakeDuration = 0.2f;    
    [SerializeField] private float shakeAngle = 20f;        
    [SerializeField] private float separateDistance = 200f; 
    [SerializeField] private float separateDuration = 0.5f; 
    [SerializeField] private float moveToCenterDuration = 0.5f; 
    [SerializeField] private float scaleDuration = 0.3f;       
    [SerializeField] private float scaleFactor = 1.5f;         

    private Vector2 bag1InitialPos;    
    private Vector2 bag2InitialPos;    
    private RectTransform rectTransform;   
    [SerializeField] private Canvas canvas; 

    private void Start()
    {
        bag1InitialPos = bag1.anchoredPosition;
        bag2InitialPos = bag2.anchoredPosition;
        rectTransform = GetComponent<RectTransform>(); 
        
        ActiveAnimation();
    }

    public void ActiveAnimation()
    {
        Sequence sequence = DOTween.Sequence();
    
        Vector2 centerScreen = GetCanvasCenter();

        Transform originalParent = transform.parent;
        transform.SetParent(canvas.transform, true);

        sequence.Append(rectTransform.DOAnchorPos(centerScreen, moveToCenterDuration))
                .Join(rectTransform.DOScale(Vector3.one * scaleFactor, scaleDuration))
                .SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    transform.SetParent(originalParent, true);
                    ShakeBag();
                });
    }




    private void ShakeBag()
    {
        Sequence shakeSequence = DOTween.Sequence();
        
        shakeSequence.Append(rectTransform.DORotate(new Vector3(0f, 0f, -shakeAngle), shakeDuration, RotateMode.Fast))
                    .Append(rectTransform.DORotate(new Vector3(0f, 0f, shakeAngle), shakeDuration, RotateMode.Fast))
                    .SetLoops(4, LoopType.Yoyo)
                    .OnComplete(() => 
                    {
                        rectTransform.DORotate(new Vector3(0f, 0f, 0f), 0.1f);
                        SeparateBags();
                    });
    }

    private void SeparateBags()
    {
        Sequence separateSequence = DOTween.Sequence();
        
        separateSequence.Append(bag1.DOAnchorPos(bag1InitialPos + Vector2.up * separateDistance, separateDuration))
                       .Join(bag2.DOAnchorPos(bag2InitialPos + Vector2.down * separateDistance, separateDuration))
                       .SetEase(Ease.OutQuad);
    }

    public void ResetAnimation()
    {
        DOTween.KillAll();
        
        rectTransform.rotation = Quaternion.identity;
        rectTransform.localScale = Vector3.one; 
        rectTransform.anchoredPosition = GetCanvasCenter(); 
        bag1.anchoredPosition = bag1InitialPos;
        bag2.anchoredPosition = bag2InitialPos;
        bag1.localScale = Vector3.one; 
        bag2.localScale = Vector3.one;
        
        ActiveAnimation();
    }
    private Vector2 GetCanvasCenter()
    {
        if (canvas == null) return Vector2.zero;
        
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 canvasSize = canvasRect.sizeDelta;
        return canvasSize * 0.5f; 
    }
}