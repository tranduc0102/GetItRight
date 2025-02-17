using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIController : Singleton<UIController>
{
    [Space]
    [Header("UI Text")]
    [SerializeField] private TextMeshProUGUI textMove;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI textLevel;
    [Space]
    [Header("UI Win")]
    [SerializeField] private GameObject uiWin;
    [SerializeField] private Button btnNext;
    private UnityAction actionNext;
    
    [Space]
    [Header("UI Lose")]
    [SerializeField] private GameObject uiLose;
    [SerializeField] private Button btnPlayAgain;
    private UnityAction actionAgain;

    [Space] [Header("Shop")] 
    [SerializeField] private Button btnShop;
    [SerializeField] private Button btnBackShop;

    [SerializeField] private GameObject objShop;

    [Space]
    [Header("Coin")]
    public Transform coinParent;
    public RectTransform imgCoin;

    private void Start()
    {
        btnNext.onClick.AddListener(delegate
        {
            actionNext?.Invoke();
        });
        btnPlayAgain.onClick.AddListener(delegate
        {
            actionAgain?.Invoke();
        });
        btnShop.onClick.AddListener(delegate
        {
            objShop.SetActive(true);
        });
        btnBackShop.onClick.AddListener(delegate
        {
            objShop.SetActive(false);
        });
        coinText.text = PlayerPrefs.GetInt("AmountCoin", 0).ToString();
        textLevel.text = "Level " + PlayerPrefs.GetInt("CurrentLevel", 0);
    }
    public void UpdateTextMove(int amountMove)
    {
        textMove.text = amountMove.ToString();
    }
    public void ShowButtonShop(bool enable)
    {
        btnShop.gameObject.SetActive(enable);
    }
    public void ShowDisplayWin(bool enable)
    {
        uiWin.SetActive(enable);
    }
    public void ShowDisplayLose(bool enable)
    {
        uiLose.SetActive(enable);
    }

    public void SetActionOnWin(UnityAction action)
    {
        actionNext = action;
    }
    public void SetActionOnLose(UnityAction action)
    {
        actionAgain = action;
    }
    public void UpdateCoin(int amountCoint)
    {
        imgCoin.transform.DOScale(Vector3.one * 1.2f, 0.05f).SetLoops(2, LoopType.Yoyo);
        PlayerPrefs.SetInt("AmountCoin", PlayerPrefs.GetInt("AmountCoin") + amountCoint);
        coinText.text = PlayerPrefs.GetInt("AmountCoin", 0).ToString();
        PlayerPrefs.Save();
    }
    public void UpdateTextedLevel(int level)
    {
        textLevel.text = "Level " + level;
    }
    public UISetting UISetting
    {
        get => GetComponentInChildren<UISetting>();
    } 
}
