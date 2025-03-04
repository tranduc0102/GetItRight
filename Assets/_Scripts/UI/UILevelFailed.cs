using System;
using _Scripts.Extension;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.UI
{
    public class UILevelFailed : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelComplete;
        [SerializeField] private UIAppear levelFailedPanel;
        
        [SerializeField] private int levelTest;
        private UnityAction<Action> _onRevices;
        private UnityAction _onNoThanks;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                ShowLevelFailedPanel(levelTest);
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                DisplayLevelFailedPanel(false);
            }
        }
        
        //goi ham nay moi lan win

        public void ShowLevelFailedPanel(int lv)
        {
            levelComplete.text = $"Level {lv}";
            
            DisplayLevelFailedPanel(true);
        }

        private void DisplayLevelFailedPanel(bool enable)
        {
            if (enable)
            {
                levelFailedPanel.gameObject.SetActive(true);
            }
            else
            {
                levelFailedPanel._Close(true);
            }
        }

        public void OnButtonNoThanksClick()
        {
            //choi lai
            DisplayLevelFailedPanel(false);
            _onNoThanks?.Invoke();
        }

        public void OnButtonReviveClick()
        {
            //Goi reward xong choi tiep
            _onRevices?.Invoke(() => DisplayLevelFailedPanel(false));
        }
        public void SetRevice(UnityAction<Action> onRevices)
        {
            _onRevices = onRevices;
        }
        public void SetNoThanks(UnityAction onNoThanks)
        {
            _onNoThanks = onNoThanks;
        }
    }
}