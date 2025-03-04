using _Scripts.Extension;
using UnityEngine;

namespace _Scripts.UI
{
    public class UIPopupLevelBonus : MonoBehaviour
    {
        [SerializeField] private UIAppear levelBonus;

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

        public void OnButtonPlayClick()
        {
            //Goi qc reward => play
        }

        public void OnButtonNoThanksClick()
        {
            DisplayLevelBonusPopup(false);
        }
    }
}