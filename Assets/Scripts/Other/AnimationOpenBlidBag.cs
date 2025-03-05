using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game;
using Lean.Touch;
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
    private Transform orginParent;
    private Vector3 originalPosition;
    [SerializeField] private Canvas canvas;
    [SerializeField] private CanvasGroup shine;
    [SerializeField] private CanvasGroup dime;
    [SerializeField] private Transform parentSpawn;

    [SerializeField] private bool isWin;
    private Action onComplete;
    private bool canReset;

    private void Start()
    {
        bag1InitialPos = bag1.anchoredPosition;
        bag2InitialPos = bag2.anchoredPosition;
        rectTransform = GetComponent<RectTransform>(); 
    }
    private void OnEnable()
    {
        LeanTouch.OnFingerDown += ResetAnimation;
    }
    private void OnDisable()
    {
        LeanTouch.OnFingerDown -= ResetAnimation;
    }
    public void SetAction(Action onComplete)
    {
        this.onComplete = onComplete;
    }
    public void SetParentSpawn(Transform parentSpawn)
    {
        this.parentSpawn = parentSpawn;
    }
    public bool ActiveAnimation(int index)
    {
        if (!ItemShopManager.instance.CheckCanSpawn(index))
        {
            return false;
        }
        if (!ItemShopManager.instance.CanBuyBag) return false;
        orginParent = transform.parent;
        ItemShopManager.instance.CanBuyBag = false;
        Sequence sequence = DOTween.Sequence();
        if (isWin)
        {
            sequence.Append(rectTransform.DOScale(Vector3.one * scaleFactor, scaleDuration))
                   .SetEase(Ease.InOutQuad)
                   .OnComplete(() => 
                   {
                       ShakeBag(index);
                   });   
        }
        else
        {
            originalPosition = rectTransform.anchoredPosition;
            transform.SetParent(canvas.transform);
            sequence.Append(rectTransform.DOAnchorPos(Vector2.zero, moveToCenterDuration))
                    .Join(rectTransform.DOScale(Vector3.one * scaleFactor, scaleDuration))
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() => 
                    {
                        ShakeBag(index);
                    });   
        }
        return true;
    }
    private void ShakeBag(int index)
    {
        Sequence shakeSequence = DOTween.Sequence();
        shakeSequence.Append(rectTransform.DORotate(new Vector3(0f, 0f, -shakeAngle), shakeDuration, RotateMode.Fast))
                    .Append(rectTransform.DORotate(new Vector3(0f, 0f, shakeAngle), shakeDuration, RotateMode.Fast))
                    .SetLoops(4, LoopType.Yoyo)
                    .OnComplete(() => 
                    {
                        rectTransform.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.1f);
                        SeparateBags(index);
                    });
    }

    private void SeparateBags(int index)
    {
        Sequence separateSequence = DOTween.Sequence();
        if (shine != null)
        {
            shine.gameObject.SetActive(true);
            ShineAnimation();
        }
        ItemShopManager.instance.SpawnRandomItemInShop(index, parentSpawn);
        separateSequence.Append(bag1.DOAnchorPos(bag1InitialPos + Vector2.up * separateDistance, separateDuration))
                       .Join(bag2.DOAnchorPos(bag2InitialPos + Vector2.down * separateDistance, separateDuration))
                       .SetEase(Ease.OutQuad).OnComplete(delegate
                       {
                           canReset = true;
                       });
    }
    private void ShineAnimation()
    {
        dime.gameObject.SetActive(true);
        dime.DOFade(1, 0.3f);
        shine.gameObject.transform.DORotate(Vector3.back * 360, 10f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
        shine.DOFade(1f, 0.5f);
    }

    public void ResetAnimation(LeanFinger finger)
    {
        if(!canReset) return;
        canReset = false;
        onComplete?.Invoke();
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = Vector3.one;
        transform.SetParent(orginParent);
        ItemShopManager.instance.ResetObject(parentSpawn);
        rectTransform.anchoredPosition = originalPosition;
        if (shine != null)
        {
            shine.gameObject.SetActive(false);
            dime.gameObject.SetActive(false);
        }
        bag1.anchoredPosition = bag1InitialPos;
        bag2.anchoredPosition = bag2InitialPos;
        bag1.localScale = Vector3.one; 
        bag2.localScale = Vector3.one;
        if (isWin)
        {
            gameObject.SetActive(false);
        }
    }
}