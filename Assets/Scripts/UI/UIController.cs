using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIController : Singleton<UIController>
{
    [Space]
    [Header("UI Text")]
    [SerializeField] private TextMeshProUGUI textMove;
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
    }
    public void UpdateTextMove(int amountMove)
    {
        textMove.text = "Can Move: "+amountMove;
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
    
    public UISetting UISetting
    {
        get => GetComponentInChildren<UISetting>();
    } 
}
