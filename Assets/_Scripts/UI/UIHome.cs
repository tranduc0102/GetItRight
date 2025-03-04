using TMPro;
using UnityEngine;

namespace _Scripts.UI
{
    public class UIHome : MonoBehaviour
    {
        [SerializeField] private GameObject home;
        [SerializeField] private GameObject coinBox;
        [SerializeField] private TextMeshProUGUI coinText;

        public void DisplayHome(bool enable)
        {
            if (enable)
            {
                home.SetActive(true);
                coinBox.SetActive(true);
            }
            else
            {
                home.SetActive(false);
                coinBox.SetActive(false);
            }
        }
        public void UpdateTextCoin(int amountCoin)
        {
            coinText.text = amountCoin.ToString();
        }
    }
}