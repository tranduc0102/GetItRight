using System;
using DG.Tweening;
using Game;
using pooling;
using UnityEngine;

public class EffectRewardCoin : MonoBehaviour
{
    [SerializeField] private Vector3[] InitialPos;
    [SerializeField] private Quaternion[] InitialRot;
    [SerializeField] private Transform posTarget;
    private Action _action;
    private int amount;

    void Start()
    {
        InitialPos = new Vector3[10];
        InitialRot = new Quaternion[10];
        for (int i = 0; i < transform.childCount; i++)
        {
            InitialPos[i] = transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition;
            InitialRot[i] = transform.GetChild(i).GetComponent<RectTransform>().rotation;
        }
    }
    public void SetActionFinishAnimation(Action onFinish, int amount)
    {
        _action = onFinish;
        this.amount = amount;
    }
    private void OnEnable()
    {
        RewardPileOfCoint(amount);
    }
    private void ResetState()
    {
        for (int i = 0; i < InitialPos.Length; i++)
        {
            transform.GetChild(i).localPosition = InitialPos[i];
            transform.GetChild(i).localRotation = InitialRot[i];
        }
    }
    private void OnDestroy()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).DOKill();
        }
    }
    private int completedCoins = 0;
    private void RewardPileOfCoint(int amount)
    {
        ResetState();
        completedCoins = 0;
        float delay = 0;
        int amountCoin;
        amountCoin = GameController.Instance.AmountCoin - 30;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform coin = transform.GetChild(i);
            coin.DOScale(1f, 0.3f).SetDelay(delay).SetEase(Ease.OutBack);
            coin.GetComponent<RectTransform>().DOAnchorPos(new Vector2(325f, 880f), 1f).SetDelay(delay + 0.5f).SetEase(Ease.InBack);
            coin.DORotate(Vector3.zero, 0.5f).SetDelay(delay + 0.5f).SetEase(Ease.Flash);
            coin.DOScale(0f, 0.3f).SetDelay(delay + 1.5f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                completedCoins++;
                if (completedCoins == transform.childCount)
                {
                    _action?.Invoke();
                    gameObject.SetActive(false);
                }
                amountCoin += 3;
               _Scripts.UI.UIController.instance.UIHome.UpdateTextCoin(amountCoin);
            });
            posTarget.DOScale(1.1f, 0.1f).SetLoops(10,LoopType.Yoyo).SetEase(Ease.InOutSine).SetDelay(1.2f);
            delay += 0.1f;
        }
        if (amount > 30)
        {
            DOVirtual.DelayedCall(2f, delegate
            {
                GameController.Instance.AmountCoin += 30;
                amountCoin += 30;
                _Scripts.UI.UIController.instance.UIHome.UpdateTextCoin(amountCoin);
            });
        }
    }
}
