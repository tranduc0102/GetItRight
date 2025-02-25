using _Scripts;
using ACEPlay.Bridge;
using DG.Tweening;
using Game;
using pooling;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIShop : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    private bool isTheme = true;
    [Space]
    [Header("Theme")]
    [SerializeField] private Button btnTheme;
    [SerializeField] private GameObject viewTheme;
    [SerializeField] private ItemInShop[] itemThemes;
    
    [Space]
    [Header("Skin")]
    [SerializeField] private Button btnSkin;
    [SerializeField] private GameObject viewSkin;
    [SerializeField] private ItemInShop[] itemSkins;
    
    [Space]
    [Header("Button Buy Or Reward")]
    [SerializeField] private Button btnBuy;
    [SerializeField] private Button btnReward;
    void Start()
    {
        var imgTheme = btnTheme.GetComponent<Image>();
        var imgSkin = btnSkin.GetComponent<Image>();
        btnTheme.onClick.AddListener(delegate
        {
            AudioManager.instance.PlaySoundButtonClick();
            isTheme = true;
            imgTheme.sprite = sprites[0];
            imgSkin.sprite = sprites[1];
            btnTheme.transform.DOScale(Vector3.one, 0.2f);
            btnSkin.transform.DOScale(Vector3.one * 0.9f, 0.2f);
            viewSkin.SetActive(false);
            viewTheme.SetActive(true);
            btnTheme.transform.SetSiblingIndex(1);
            btnSkin.transform.SetSiblingIndex(0);
        });
        btnSkin.onClick.AddListener(delegate
        {
            AudioManager.instance.PlaySoundButtonClick();
            isTheme = false;
            imgSkin.sprite = sprites[0];
            imgTheme.sprite = sprites[1];
            btnSkin.transform.DOScale(Vector3.one, 0.2f);
            btnTheme.transform.DOScale(Vector3.one * 0.9f, 0.2f);
            viewSkin.SetActive(true);
            viewTheme.SetActive(false);
            btnSkin.transform.SetSiblingIndex(1);
            btnTheme.transform.SetSiblingIndex(0);
        });
        btnBuy.onClick.AddListener(BuyNew);
        btnReward.onClick.AddListener(RewardCoin);
    }

    private void BuyNew()
    {
        AudioManager.instance.PlaySoundButtonClick();
        if (PlayerPrefs.GetInt("AmountCoin", 0) >= 500)
        {
            if (isTheme)
            {
                ItemInShop i = null;
                foreach (var item in itemThemes)
                {
                    if (!item.UnLock)
                    {
                        i = item;
                        item.UnLock = true;
                        break;
                    }
                }
                if(i != null) UIController.instance.UpdateCoin(-500);
            }
            else
            {
                foreach (var item in itemSkins)
                {
                    if (!item.UnLock)
                    {
                        item.UnLock = true;
                        break;
                    }
                }
            }
        }
    }
    private void RewardCoin()
    {
        AudioManager.instance.PlaySoundButtonClick();
        if (BridgeController.instance.IsRewardReady())
        {
            UnityEvent e = new UnityEvent();
            e.AddListener(delegate
            {
                PoolingManager.Spawn(GameController.Instance.EffectCoin, transform.position, Quaternion.identity, UIController.instance.coinParent);
                DOVirtual.DelayedCall(1.5f, delegate
                {
                    UIController.instance.UpdateCoin(990);
                });
            });
            BridgeController.instance.ShowRewarded("RewardCoin", e);
        }
    }
}