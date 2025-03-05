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
        
    }

   
    private void RewardCoin()
    {
       
    }
}