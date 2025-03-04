using _Scripts.Extension;
using UnityEngine;

namespace _Scripts.UI
{
    public class UIPopupRemoveAds : MonoBehaviour
    {
        [SerializeField] private UIAppear removeAds;

        public void DisplayRemovePopup(bool enable)
        {
            if (enable)
            {
                removeAds.gameObject.SetActive(true);
            }
            else
            {
                removeAds._Close(true);
            }
        }

        public void OnButtonRemoveClick()
        {
            
        }
        
        public void OnButtonNoThanksClick()
        {
            DisplayRemovePopup(false);
        }
        
        //Co 1 ham check o day nua, khi nao ghep QC them sau
    }
}