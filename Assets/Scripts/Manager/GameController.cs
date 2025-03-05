using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts;
using ACEPlay.Bridge;
using DG.Tweening;
using UnityEngine;
using pooling;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance;

        [Space]
        [Header("DataGame")]
        [SerializeField] private LevelData currentLevel;
        [SerializeField] private DataCharacter dataCharacter;
        [SerializeField] private ChangeBackground changeBackground;
        public int CurrentSkin
        {
            get => PlayerPrefs.GetInt(USESTRING.CURRENT_SKIN, 0);
            set
            {
                if (value < 0) return;
                PlayerPrefs.SetInt(USESTRING.CURRENT_SKIN, value);
                if (currentPanel)
                {
                    currentPanel.UpdateChangeTheme(value,true);
                }
            }
        }
        public int CurrentPlayer
        {
            get => PlayerPrefs.GetInt(USESTRING.CURRENT_PLAYER, 0);
            set
            {
                if (value < 0 || value > dataCharacter.characters.Count) return;
                PlayerPrefs.SetInt(USESTRING.CURRENT_PLAYER, value);
                playerManager.ChangePlayer(dataCharacter.characters[value], true);
            }
        }
        public int CurrentTheme
        {
            get => PlayerPrefs.GetInt(USESTRING.CURRENT_THEME, 0);
            set
            {
                if (value < 0) return;
                PlayerPrefs.SetInt(USESTRING.CURRENT_THEME, value);
                changeBackground.ApplyBackground(value);
            }
        }
        public int AmountCoin
        {
            get => PlayerPrefs.GetInt(USESTRING.AMOUNT_COIN, 0);
            set => PlayerPrefs.SetInt(USESTRING.AMOUNT_COIN, value);
        }
        public LevelData CurrentLevelGame => currentLevel;
        public DataCharacter DataCharacter => dataCharacter;
        [SerializeField] private List<EnumAnswer> answers;
        public List<EnumAnswer> Answers => answers;

        [Space]
        [Header("InGame")]
        [SerializeField] private Transform inGame;

        [Space]
        [Header("Player")]
        [SerializeField] private PlayerManager playerManager;
        public PlayerManager PlayerManager => playerManager;
        [FormerlySerializedAs("PlayerMoved")] public bool playerMoved = true;

        [Space]
        [Header("Panel Objects")]
        [SerializeField] private Transform parentPanel;
        [SerializeField] private PanelAnswerController[] panelAnswers;
        private PanelAnswerController currentPanel;
        public PanelAnswerController PanelAnswerController => currentPanel;

        [Space]
        [Header("Board Game")]
        [SerializeField] private Transform table;
        [SerializeField] private int IndexRandomLevelBonus;
        [SerializeField] private int CurrentLevelBonus;

        [Header("Boards")]
        [SerializeField] private Board[] boards3;
        [SerializeField] private Board[] boards4;
        [SerializeField] private Board[] boards5;
        [SerializeField] private Board[] boards6;
        [SerializeField] private Board[] boards7;

        [SerializeField] private Board currentBoard;
        public Board Board => currentBoard;

        [FormerlySerializedAs("InGame1")] public bool inGame1;
        [FormerlySerializedAs("InGame2")] public bool inGame2;


        [SerializeField] private bool canClick = true;
        public bool CanClick
        {
            get => canClick;
            set => canClick = value;
        }

        private int IndexCurrentLevel
        {
            set
            {
                PlayerPrefs.SetInt(USESTRING.CURRENT_LEVEL, value);
            }
            get => PlayerPrefs.GetInt(USESTRING.CURRENT_LEVEL, 0);
        }
        public bool CanSkip { get; set; }

        private bool isWin;
        public bool IsWin
        {
            get => isWin;
            set
            {
                isWin = value;
                if (isWin)
                {
                    playerManager.PlayAnim(StateFace.Win);
                    effectWin.Play();
                    if (IsFirstPlayGame && !IsFinishTutorial)
                    {
                        DOVirtual.DelayedCall(1.5f, delegate
                        {
                            IsFirstPlayGame = false;
                            IsFinishTutorial = true;
                            tutorial.SetActive(false);
                            currentPanel.gameObject.SetActive(false);
                            ++IndexCurrentLevel;
                            NextLevel();
                        });
                        return;
                    }
                    DOVirtual.DelayedCall(0.7f, delegate
                    {
                        if (inGame1)
                        {
                            _Scripts.UI.UIController.instance.UIWin.ShowWinPanel(IndexCurrentLevel);
                            AmountCoin += 30;
                            ++IndexCurrentLevel;
                        }
                        else
                        {
                            ++IndexCurrentLevel;
                            playerManager.ResetPlayers();
                            NextLevel();
                        }
                    });
                    currentPanel.gameObject.SetActive(false);
                }
            }
        }

        [Space]
        [Header("Box Answer")]
        [SerializeField] private BoxManager boxManager;
        public BoxManager BoxManager => boxManager;

        [Space] [Header("Effect")] [SerializeField]
        private ParticleSystem effectWin;
        [Space]
        [Header("Camera")]
        [SerializeField] SmoothCameraController cameraController;
        private bool isPlay;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                return;
            }
            Destroy(this);
        }

        private void Start()
        {
            _Scripts.UI.UIController.instance.UILevelFailed.SetRevice(SaveLevelFail);
            _Scripts.UI.UIController.instance.UILevelFailed.SetNoThanks(PlayAgain);
            isWin = false;
            CanSkip = true;
            canClick = true;
            playerManager.ChangePlayer(dataCharacter.characters[CurrentPlayer], false);
            inGame2 = false;
            inGame1 = true;
            currentLevel = CreateLevel.instance.GetLevelData(PlayerPrefs.GetInt(USESTRING.CURRENT_LEVEL, 1));
            changeBackground.ApplyBackground(CurrentTheme);
            _Scripts.UI.UIController.instance.UIHome.UpdateTextCoin(AmountCoin);
            GetAnswers(currentLevel.amountDistinct);
            IndexRandomLevelBonus = Random.Range(3, 6);
            _Scripts.UI.UIController.instance.UIPopupLevelBonus.SetPlayAction(PlayOtherInGame);
            _Scripts.UI.UIController.instance.UIPopupLevelBonus.SetNoThanksAction(delegate
            {
                CurrentLevelBonus = 0;
                NextLevel();
            });
        }
        public void StartGame()
        {
            if(isPlay)return;
            isPlay = true;
            StartCoroutine(playerManager.AnimRun(cameraController.MoveAndRotateTo, InitializeGame)); 
            _Scripts.UI.UIController.instance.UIHome.DisplayHome(false);
            BridgeController.instance.LogLevelStartWithParameter(PlayerPrefs.GetInt(USESTRING.CURRENT_LEVEL, 1));
        }
        private void InitializeGame()
        {
            if (PlayerPrefs.GetInt(USESTRING.CURRENT_LEVEL, 1) == 1 && IsFirstPlayGame)
            {
                answers.Clear();
                CanSkip = false;
                answers = new List<EnumAnswer>(3)
                {
                    EnumAnswer.Zero,
                    EnumAnswer.Zero,
                    EnumAnswer.One,
                };
                currentBoard = tutorial.GetComponentInChildren<Board>();
                currentPanel = tutorial.GetComponentInChildren<PanelAnswerController>();
                tutorial.SetActive(true);
                _Scripts.UI.UIController.instance.UIInGame.DisplayInGame(true);
            }
            else
            {
                SpawnBoard();
                _Scripts.UI.UIController.instance.UIInGame.DisplayInGame(true);
                DOVirtual.DelayedCall(0.1f, SpawnPanel);
            }
            boxManager.NextLevelOrReplay(currentLevel.amountBox);
            AudioManager.instance.StopMusic();
            DOVirtual.DelayedCall(0.5f, delegate
            {
                AudioManager.instance.PlayInGameMusic();
            });
        }
        private void SaveLevelFail(Action action)
        {
            if (BridgeController.instance.IsRewardReady())
            {
                UnityEvent e = new UnityEvent();
                e.AddListener(delegate
                {
                    isWin = false;
                    CanSkip = true;
                    canClick = true;
                    playerManager.PlayAnim(StateFace.Idle);
                    currentBoard.SaveMeBoard();
                    action?.Invoke();
                });
                BridgeController.instance.ShowRewarded("save_me", e);
            }
        }
        public void PlayOtherInGame()
        {
            CurrentLevelBonus = 0;
            playerManager.SpawnOtherPlayers();
            inGame2 = true;
            inGame1 = false;
            isWin = false;
            CanSkip = true;
            canClick = true;
            playerManager.PlayAnim(StateFace.Idle);
            int amountBox = Random.Range(3, 7);
            int amountLine = Random.Range(3, 6);
            int amountDistinct = Random.Range(2, amountBox + 1);
            int amountCan = Random.Range(Math.Max(amountDistinct,3), 7);
            currentLevel = new LevelData{
                amountBox = amountBox,
                amountLine = amountLine,
                amountCan = amountCan,
                amountDistinct = amountDistinct
            };
            GetAnswers(currentLevel.amountDistinct);
            if (currentBoard)
            {
                Destroy(currentBoard.gameObject);
            }
            SpawnBoard();
            SpawnPanel();
            boxManager.NextLevelOrReplay(currentLevel.amountBox);
        }
        private void PlayAgain()
        {
            currentPanel.gameObject.SetActive(false);
            playerManager.PlayAnim(StateFace.Idle);
            isWin = false;
            CanSkip = true;
            canClick = true;
            GetAnswers(currentLevel.amountDistinct);
            Destroy(currentBoard.gameObject);
            SpawnBoard();
            SpawnPanel();
            boxManager.NextLevelOrReplay(currentLevel.amountBox);
            _Scripts.UI.UIController.instance.UIInGame.ShowSkipButton(false);
            BridgeController.instance.LogLevelFailWithParameter(PlayerPrefs.GetInt(USESTRING.CURRENT_LEVEL, 1));
        }

        public void SkipLevel()
        {
            if(!CanSkip) return;
            if (BridgeController.instance.IsRewardReady())
            {
                UnityEvent e = new UnityEvent();
                e.AddListener(delegate
                {
                    IsWin = true;
                });
                BridgeController.instance.ShowRewarded($"skip_level_{IndexCurrentLevel}", e);
            }
        }
        public void NextLevel()
        {
            if (CurrentLevelBonus >= IndexRandomLevelBonus)
            {
                IndexRandomLevelBonus = Random.Range(3, 6);
                _Scripts.UI.UIController.instance.UIPopupLevelBonus.DisplayLevelBonusPopup(true);
                return;
            }
            inGame2 = false;
            inGame1 = true;
            isWin = false;
            CanSkip = true;
            playerManager.PlayAnim(StateFace.Idle);
            currentLevel = CreateLevel.instance.GetLevelData(IndexCurrentLevel);
            canClick = true;
            CurrentLevelBonus++;
            GetAnswers(currentLevel.amountDistinct);
            Destroy(currentBoard.gameObject);
            SpawnBoard();
            _Scripts.UI.UIController.instance.UIInGame.ShowSkipButton(false);
            DOVirtual.DelayedCall(0.1f, SpawnPanel);
            boxManager.NextLevelOrReplay(currentLevel.amountBox);
        }

        private void GetAnswers(int amountDistinct)
        {
            answers.Clear();
            HashSet<EnumAnswer> uniqueValues = new HashSet<EnumAnswer>();
            int maxDistinct = amountDistinct;
            while (uniqueValues.Count < maxDistinct)
            {
                EnumAnswer newValue = (EnumAnswer)Random.Range(1, 7);
                uniqueValues.Add(newValue);
            }
            answers.AddRange(uniqueValues);
            while (answers.Count < currentLevel.amountBox)
            {
                answers.Add(uniqueValues.ElementAt(Random.Range(0, uniqueValues.Count)));
            }
            ShuffleList(answers);
        }

        private void ShuffleList<T>(List<T> list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        private void SpawnBoard()
        {
            int typeBoard = currentLevel.amountBox;
            int line = currentLevel.amountLine;
            if (inGame1 && !inGame2)
            {
                switch (typeBoard)
                {
                    case 3:
                        GetBoard(line, boards3);
                        break;
                    case 4:
                        GetBoard(line, boards4);
                        break;
                    case 5:
                        GetBoard(line, boards5);
                        break;
                    case 6:
                        GetBoard(line, boards6);
                        break;
                    case 7:
                        GetBoard(line, boards7);
                        break;
                }   
            }

            if (inGame2 && !inGame1)
            {
                switch (typeBoard)
                {
                    case 3:
                        GetBoard(line, boards3);
                        break;
                    case 4:
                        GetBoard(line, boards4);
                        break;
                    case 5:
                        GetBoard(line, boards5);
                        break;
                    case 6:
                        GetBoard(line, boards6);
                        break;
                }
            }
        }
        private void GetBoard(int id, Board[] boards)
        {
            if (inGame1 && !inGame2)
            {
                switch (id)
                {
                    case 3:
                        currentBoard = PoolingManager.Spawn(boards[0], table.position, Quaternion.identity, table);
                        currentBoard.transform.localPosition = new Vector3(0f, 0f, -3f);
                        break;
                    case 4:
                        currentBoard = PoolingManager.Spawn(boards[1], table.position, Quaternion.identity, table);
                        currentBoard.transform.localPosition = new Vector3(0f, 0f, -1.5f);
                        break;
                    case 5:
                        currentBoard = PoolingManager.Spawn(boards[2], table.position, Quaternion.identity, table);
                        break;
                }
            }
            if (inGame2 && !inGame1)
            {
                switch (id)
                {
                    case 3:
                        currentBoard = PoolingManager.Spawn(boards[3], table.position, Quaternion.identity, table);
                        currentBoard.transform.localPosition = new Vector3(0f, -2f, 2f);
                        break;
                    case 4:
                        currentBoard = PoolingManager.Spawn(boards[4], table.position, Quaternion.identity, table);
                        currentBoard.transform.localPosition = new Vector3(0f, -2f, 2f);
                        break;
                    case 5:
                        currentBoard = PoolingManager.Spawn(boards[5], table.position, Quaternion.identity, table);
                        currentBoard.transform.localPosition = new Vector3(0f, -2f, 2f);
                        break;
                }
            }
        }

        private void SpawnPanel()
        {
            int id = currentLevel.amountCan;
            switch (id)
            {
                case 3:
                    SetActivePanel(0);
                    break;
                case 4:
                    SetActivePanel(1);
                    break;
                case 5:
                    SetActivePanel(2);
                    break;
                case 6:
                    SetActivePanel(3);
                    break;
            }
        }

        private void SetActivePanel(int index)
        {
            if (currentPanel != null)
            {
                currentPanel.gameObject.SetActive(false);
            }
            currentPanel = panelAnswers[index];
            currentPanel.gameObject.SetActive(true);
        }
    #region Tutorial
        [Header("Tutorial")]
        [SerializeField] private GameObject tutorial;
        [SerializeField] private SpriteRenderer[] hand;
        [SerializeField] private GameObject[] TextConnect;
        public int IDCanClick = 0;
        public bool IsFirstPlayGame
        {
            get => PlayerPrefs.GetInt(USESTRING.IS_FIRST_PLAYGAME, 1) == 1;
            set => PlayerPrefs.SetInt(USESTRING.IS_FIRST_PLAYGAME, value ? 1 : 0);
        }
        public bool IsFinishTutorial
        {
            get => PlayerPrefs.GetInt(USESTRING.IS_FINISH_TUT, 0) == 1;
            set => PlayerPrefs.SetInt(USESTRING.IS_FINISH_TUT, value ? 1 : 0);
        }
        public void UpdateStepsTutorial(int step)
        {
            switch (step)
            {
                case 1:
                    hand[IDCanClick].DOFade(1, 0.5f);
                    break;
                case 2:
                    ActivateCurrentHand(1);
                    break;
                case 3:
                    ActivateCurrentHand(2);
                    break;
                case 4:
                    ActivateCurrentHand(-1);
                    break;
                case 5:
                    ActivateCurrentHand(0);
                    break;
                case 6:
                    ActivateCurrentHand(-1);
                    break;
                case 7:
                    ActivateCurrentHand(1);
                    break;
            }
        }
        public void UpdateTextConnect(int id)
        {
            if (TextConnect.Any(t => !t.activeSelf))
            {
                TextConnect[id].SetActive(true);
            }
        }
        private void ActivateCurrentHand(int id)
        {
            hand[IDCanClick].DOFade(0, 0.05f).OnComplete(delegate
            {
                if(id < 0) return;
                hand[IDCanClick].gameObject.SetActive(false);
                IDCanClick = id;
                hand[IDCanClick].gameObject.SetActive(true);
                hand[IDCanClick].DOFade(1, 0.2f);
            });
        }
    #endregion
    }
}