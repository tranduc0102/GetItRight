using System;
using _Scripts.Extension;
using ACEPlay.Bridge;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.UI
{
    public class UIPopupLevelBonus : MonoBehaviour
    {
        [SerializeField] private UIAppear levelBonus;
        private Action _callbackPlay;
        private Action _callbackNoThanks;

        public void DisplayLevelBonusPopup(bool enable)
        {
            if (enable)
            {
                levelBonus.gameObject.SetActive(true);
            }
            else
            {
                levelBonus._Close(true);
            }
        }
        public void SetPlayAction(Action callback)
        {
            _callbackPlay = callback;
        }
        public void SetNoThanksAction(Action callback)
        {
            _callbackNoThanks = callback;
        }
        public void OnButtonPlayClick()
        {
            //Goi qc reward => play
            if (BridgeController.instance.IsRewardReady())
            {
                UnityEvent e = new UnityEvent();
                e.AddListener(delegate
                {
                    _callbackPlay?.Invoke();
                    DisplayLevelBonusPopup(false);
                });
                BridgeController.instance.ShowRewarded("Play_Level_Bonus", e);
            }
        }

        public void OnButtonNoThanksClick()
        {
            _callbackNoThanks?.Invoke();
            DisplayLevelBonusPopup(false);
        }
    }
}