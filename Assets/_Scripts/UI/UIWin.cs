using System;
using _Scripts.Extension;
using ACEPlay.Bridge;
using DG.Tweening;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIWin : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelComplete;
        [SerializeField] private UIAppear winPanel;
        [SerializeField] private UIAppear shine;
        [SerializeField] private UIAppear reward;
        [SerializeField] private UIAppear btnGetIt;
        [SerializeField] private UIAppear btnNoThanks;
        [SerializeField] private UIAppear btnOpenGift;
        [SerializeField] private ParticleSystem vfx;
        
        [Space]
        [Header("Reward")]
        private int level;
        [SerializeField] private int levelTest;
        [SerializeField] private float currentValue;
        [SerializeField] private Transform gift;
        [SerializeField] private Image rewardImage;
        [SerializeField] private TextMeshProUGUI percent;
        [SerializeField] private GameObject iconAds;
        
        [Space]
        [Header("CoinReward")]
        [SerializeField] private UIAppear coinReward;
        [SerializeField] private TextMeshProUGUI coinAmount;
        [SerializeField] private EffectRewardCoin animRewardCoin;

        private void Start()
        {
            UnityEvent eShineRotate = new UnityEvent();
            eShineRotate.AddListener(ShineAnimation);
            
            UnityEvent eRewardAnim = new UnityEvent();
            eRewardAnim.AddListener(RewardAnimation);
            
            shine.SetOnFinishEvent(eShineRotate);
            reward.SetOnFinishEvent(eRewardAnim);
        }

        private void Update()
        {
            // if (Input.GetKeyDown(KeyCode.T))
            // {
            //     ShowWinPanel(levelTest);
            // }
            // if (Input.GetKeyDown(KeyCode.C))
            // {
            //     DisplayWinPanel(false);
            // }
        }
        
        //goi ham nay moi lan win

        public void ShowWinPanel(int lv)
        {
            level = lv;
            levelComplete.text = $"Level {level}";
                            
            if (level <= 3)
            {
                rewardImage.fillAmount = (float)(level - 1) / 3;
                percent.text = $"{Mathf.FloorToInt(((float)(level - 1) / 3) * 100)}%";
                    
                currentValue = (float)level / 3;
            }
            else if (level <= 8)
            {
                rewardImage.fillAmount = (float)(level - 4) / 5;
                percent.text = $"{Mathf.FloorToInt(((float)(level - 4) / 5) * 100)}%";
                    
                currentValue = (float)(level - 3)/ 5;
            }
            else
            {
                rewardImage.fillAmount = (float)(level - 9) % 7 / 7;
                percent.text = $"{Mathf.FloorToInt(((float)(level - 9) % 7 / 7) * 100)}%";
                currentValue = (float)(level - 8) % 7 / 7;
            }
            
            DisplayWinPanel(true);
        }

        private void DisplayWinPanel(bool enable)
        {
            if (enable)
            {
                vfx.gameObject.SetActive(true);

                DOVirtual.DelayedCall(1.5f, delegate
                {
                    winPanel.gameObject.SetActive(true);
                });
            }
            else
            {
                winPanel.Close(delegate
                {
                    coinReward.gameObject.SetActive(false);
                    btnGetIt.gameObject.SetActive(false);
                    btnOpenGift.gameObject.SetActive(false);
                    btnNoThanks.gameObject.SetActive(false);
                },true);
            }
        }

        private void RewardAnimation()
        {
            var t = rewardImage.fillAmount;
            
            DOVirtual.Float(t, currentValue, 1f, value =>
            {
                rewardImage.fillAmount = value;
                percent.text = $"{Mathf.FloorToInt(value * 100)}%";
            })
            .OnComplete(delegate
            {
                if (currentValue >= 1f)
                {
                    GiftAnimation();
                    
                    if (level <= 15)
                    {
                        iconAds.SetActive(false);
                    }
                    else
                    {
                        iconAds.SetActive(true);
                    }
                    btnOpenGift.gameObject.SetActive(true);
                }
                else
                {
                    coinReward.gameObject.SetActive(true);
                    btnGetIt.gameObject.SetActive(true);
                }

                btnNoThanks.gameObject.SetActive(true);
            });
        }

        private void ShineAnimation()
        {
            shine.gameObject.transform.DORotate(Vector3.back * 360, 10f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
        }

        private void GiftAnimation()
        {
            gift.DORotate(Vector3.back*3f,0.8f, RotateMode.WorldAxisAdd ).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo).SetId("giftAnimation");
        }

        public void OnButtonNoThanksClick()
        {
            //animation nhan coin o day => complete thi goi ham ben duoi
            animRewardCoin.SetActionFinishAnimation(delegate
            {
                DisplayWinPanel(false);
                GameController.Instance.NextLevel();
            }, 100);
            animRewardCoin.gameObject.SetActive(true);
        }

        public void OnButtonGetItClick()
        {
            //Goi reward xong show animation nhu tren
            if (BridgeController.instance.IsRewardReady())
            {
                UnityEvent e = new UnityEvent();
                e.AddListener(delegate
                {
                    animRewardCoin.SetActionFinishAnimation(delegate
                    {
                        DisplayWinPanel(false);
                        GameController.Instance.NextLevel();
                    }, 200);
                    animRewardCoin.gameObject.SetActive(true);
                });
                BridgeController.instance.ShowRewarded("Reward X2 Coin", e);
            }
        }

        public void OnButtonOpenGiftClick()
        {
            if (level <= 15)
            {
                btnOpenGift.Close(delegate
                {
                    //animation nhan qua
                }, true);
                
            }
            else
            {
                //goi qc reward =>
                if (BridgeController.instance.IsRewardReady())
                {
                    UnityEvent e = new UnityEvent();
                    e.AddListener(delegate {});
                    BridgeController.instance.ShowRewarded("Reward Open Gift", e);
                }
                btnOpenGift.Close(delegate
                {
                    //animation nhan qua
                }, true);
            }
        }
    }
}