using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.UI
{
    public class UIIngame : MonoBehaviour
    {
        [SerializeField] private GameObject inGame;
        [SerializeField] private GameObject coinBox;
        [SerializeField] private GameObject btnShop;
        [SerializeField] private GameObject btnSkip;

        public void DisplayInGame(bool enable)
        {
            if (enable)
            {
                inGame.SetActive(true);
                coinBox.SetActive(true);
                btnShop.SetActive(true);
                btnSkip.SetActive(false);
            }
            else
            {
                inGame.SetActive(false);
                coinBox.SetActive(false);
                btnShop.SetActive(false);
                btnSkip.SetActive(false);
            }
        }

        public void ShowSkipButton(bool isShow)
        {
            if (isShow)
            {
                btnShop.SetActive(false);
                DOVirtual.DelayedCall(0.5f, delegate
                {
                    btnSkip.SetActive(true);
                });
            }
            else
            {
                btnSkip.SetActive(false);
                DOVirtual.DelayedCall(0.5f, delegate
                {
                    btnShop.SetActive(true);
                });
            }
        }

        public void ShowSKip(bool enable)
        {
            btnSkip.SetActive(enable);
        }
    }
}