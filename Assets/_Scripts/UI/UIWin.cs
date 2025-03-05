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
        
        
        [Space]
        [Header("Setup Animation Gift")]
        [SerializeField] private RandomLuckPakage randomLuckPakage;
        [SerializeField] private AnimationOpenBlindBag animationOpen;
        [SerializeField] private GameObject giftEmpty;
        private bool canOpenGift;
        [SerializeField] private Image bag1;
        [SerializeField] private Image bag2;
        [SerializeField] private Transform[] parentSpawn;
        [SerializeField] private Sprite[] spriteGiftBag;
        [SerializeField] private Sprite[] imgBag1;
        [SerializeField] private Sprite[] imgBag2;

        private void Start()
        {
            UnityEvent eShineRotate = new UnityEvent();
            eShineRotate.AddListener(ShineAnimation);
            
            UnityEvent eRewardAnim = new UnityEvent();
            eRewardAnim.AddListener(RewardAnimation);
            
            shine.SetOnFinishEvent(eShineRotate);
            reward.SetOnFinishEvent(eRewardAnim);
            animationOpen.SetAction(ActionAfterFinishOpenGift);
        }
        private void OnEnable()
        {
            UpdateSpriteGift(PlayerPrefs.GetInt(USESTRING.ID_IMAGE_GIFT, 0));
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
                if ((float)(level - 8) % 7 == 0)
                {
                    currentValue = 1;
                }
                else
                {
                    currentValue = (float)(level - 8) % 7 / 7;
                }
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
                    DOTween.Kill("giftAnimation");
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
                    canOpenGift = true;
                    animationOpen.gameObject.SetActive(true);
                    
                    btnNoThanks.duration = 3.5f;
                }
                else
                {
                    coinReward.gameObject.SetActive(true);
                    btnGetIt.gameObject.SetActive(true);
                    btnNoThanks.duration = 5f;
                }

                btnNoThanks.gameObject.SetActive(true);
            });
        }

        private void ShineAnimation()
        {
            shine.gameObject.transform.DORotate(Vector3.back * 360, 10f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
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
                if (canOpenGift)
                {
                    ActionAfterFinishOpenGift();
                }
                else
                {
                    DisplayWinPanel(false);
                    GameController.Instance.NextLevel();
                }
            }, 30);
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
                    }, 60);
                    animRewardCoin.gameObject.SetActive(true);
                });
                BridgeController.instance.ShowRewarded("_reward_x2_coin", e);
            }
        }

        public void OnButtonOpenGiftClick()
        {
            rewardImage.gameObject.SetActive(false);
            giftEmpty.SetActive(false);
            if (level <= 15)
            {
                btnOpenGift.Close(delegate
                {
                    //animation nhan qua
                    animationOpen.ActiveAnimation(PlayerPrefs.GetInt(USESTRING.ID_IMAGE_GIFT, 0));
                }, true);
                btnNoThanks._Close(true);
                
            }
            else
            {
                //goi qc reward =>
                if (BridgeController.instance.IsRewardReady())
                {
                    UnityEvent e = new UnityEvent();
                    e.AddListener(delegate
                    {
                        btnOpenGift.Close(delegate
                        {
                            //animation nhan qua
                            animationOpen.ActiveAnimation(PlayerPrefs.GetInt(USESTRING.ID_IMAGE_GIFT, 0));
                        }, true);
                        btnNoThanks._Close(true);
                    });
                    BridgeController.instance.ShowRewarded("_reward_open_gift", e);
                }
            }
        }
        private void ActionAfterFinishOpenGift()
        {
            GameController.Instance.NextLevel();
            PlayerPrefs.SetInt(USESTRING.ID_IMAGE_GIFT, randomLuckPakage.GetRandomPieceIndex());
            UpdateSpriteGift(PlayerPrefs.GetInt(USESTRING.ID_IMAGE_GIFT, 0));
            DisplayWinPanel(false);
            canOpenGift = false;
            rewardImage.gameObject.SetActive(true);
            giftEmpty.SetActive(true);
        }
        
        public void UpdateSpriteGift(int index)
        {
            rewardImage.sprite = spriteGiftBag[index];
            bag1.sprite = imgBag1[index];
            bag2.sprite = imgBag2[index];
            animationOpen.SetParentSpawn(parentSpawn[index]);
        }
    }
}