using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts;
using _Scripts.Extension;
using ACEPlay.Bridge;
using DG.Tweening;
using Lean.Touch;
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
        public int CurrentSkin
        {
            get => PlayerPrefs.GetInt("CurrentSkin", 0);
            set
            {
                if (value < 0) return;
                PlayerPrefs.SetInt("CurrentSkin", value);
                if (currentPanel)
                {
                    currentPanel.UpdateChangeTheme(value,true);
                }
            }
        }
        public LevelData CurrentLevelGame => currentLevel;
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
                PlayerPrefs.SetInt("CurrentLevel", value);
                UIController.instance.UpdateTextedLevel(value + 1);
            }
            get => PlayerPrefs.GetInt("CurrentLevel", 0);
        }

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
                    DOVirtual.DelayedCall(0.7f, delegate
                    {
                        UIController.instance.ShowDisplayWin(true);
                    });
                    currentPanel.gameObject.SetActive(false);
                }
            }
        }

        [Space]
        [Header("Box Answer")]
        [SerializeField] private BoxManager boxManager;
        public BoxManager BoxManager => boxManager;

        [Space]
        [Header("Effect")]
        [SerializeField] private EffectRewardCoin effectCoin;
        public EffectRewardCoin EffectCoin => effectCoin;
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
            UIController.instance.SetActionOnWin(NextLevel);
            UIController.instance.SetActionSave(SaveLevelFail);
            UIController.instance.SetActionOnLose(PlayAgain);
            IsWin = false;
            canClick = true;
            inGame1 = true;
            
        }
        private void OnEnable()
        {
            LeanTouch.OnFingerDown += StartGame;
        }
        private void OnDisable()
        {
            LeanTouch.OnFingerDown -= StartGame;
        }
        private void StartGame(LeanFinger leanFinger)
        {
            if(UIDetection.IsPointerOverUIObject() || isPlay)return;
            isPlay = true;
           
            playerManager.AnimRun( cameraController.MoveAndRotateTo);
            DOVirtual.DelayedCall(3.1f, InitializeGame);
        }
        private void InitializeGame()
        {
            inGame2 = false;
            inGame1 = true;
            currentLevel = CreateLevel.instance.GetLevelData(PlayerPrefs.GetInt("CurrentLevel", 1));
            if (PlayerPrefs.GetInt("CurrentLevel", 1) == 1 && IsFirstPlayGame)
            {
                answers.Clear();
                answers = new List<EnumAnswer>(3)
                {
                    EnumAnswer.Zero,
                    EnumAnswer.Zero,
                    EnumAnswer.One,
                };
                currentBoard = tutorial.GetComponentInChildren<Board>();
                currentPanel = tutorial.GetComponentInChildren<PanelAnswerController>();
                tutorial.SetActive(true);
            }
            else
            {
                GetAnswers(currentLevel.amountDistinct);
                SpawnBoard();
                SpawnPanel();
            }
            boxManager.NextLevelOrReplay(currentLevel.amountBox);
            AudioManager.instance.StopMusic();
            DOVirtual.DelayedCall(0.5f, delegate
            {
                AudioManager.instance.PlayInGameMusic();
            });
            BridgeController.instance.LogLevelStartWithParameter(PlayerPrefs.GetInt("CurrentLevel", 1));
        }
        private void SaveLevelFail()
        {
            if (BridgeController.instance.IsRewardReady())
            {
                UnityEvent e = new UnityEvent();
                e.AddListener(delegate
                {
                    IsWin = false;
                    canClick = true;
                    playerManager.PlayAnim(StateFace.Idle);
                    currentBoard.SaveMeBoard();
                });
                BridgeController.instance.ShowRewarded("SaveMe", e);
            }
        }
        public void PlayOtherInGame()
        {
            inGame2 = true;
            inGame1 = false;
            IsWin = false;
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
            IsWin = false;
            canClick = true;
            GetAnswers(currentLevel.amountDistinct);
            Destroy(currentBoard.gameObject);
            SpawnBoard();
            SpawnPanel();
            boxManager.NextLevelOrReplay(currentLevel.amountBox);
        }

        public void NextLevel()
        {
            if (IsFirstPlayGame && !IsFinishTutorial)
            {
                IsFirstPlayGame = false;
                IsFinishTutorial = true;
                tutorial.SetActive(false);
            }
            IsWin = false;
            playerManager.PlayAnim(StateFace.Idle);
            IndexCurrentLevel += 1;
            currentLevel = CreateLevel.instance.GetLevelData(IndexCurrentLevel);
            canClick = true;
            
            GetAnswers(currentLevel.amountDistinct);
            Destroy(currentBoard.gameObject);
            SpawnBoard();
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
            Debug.LogWarning("Ok2");
            UIController.instance.ShowButtonShop(true);
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
            for (int i = 0; i < panelAnswers.Length; i++)
            {
                panelAnswers[i].gameObject.SetActive(i == index);
            }
            currentPanel = panelAnswers[index];
            currentPanel.gameObject.SetActive(true);
        }

        public void Save(string levelName)
        {
            IndexCurrentLevel++;
            /*CreateLevel.instance.SaveData(levelName, currentLevel);*/
            Debug.Log(PlayerPrefs.GetInt("CurrentLevel", 0));
        }

        public void Load()
        {
            IsWin = false;
            BridgeController.instance.LogLevelCompleteWithParameter(PlayerPrefs.GetInt("CurrentLevel", 1));
            currentLevel = CreateLevel.instance.GetLevelData(PlayerPrefs.GetInt("CurrentLevel", 1));
            canClick = true;
            GetAnswers(currentLevel.amountDistinct);
            Destroy(currentBoard.gameObject);
            SpawnBoard();
            SpawnPanel();
            boxManager.NextLevelOrReplay(currentLevel.amountBox);
        }
    #region Tutorial
        [Header("Tutorial")]
        [SerializeField] private GameObject tutorial;
        [SerializeField] private SpriteRenderer[] hand;
        [SerializeField] private GameObject[] TextConnect;
        public int IDCanClick = 0;
        public bool IsFirstPlayGame
        {
            get => PlayerPrefs.GetInt("IsFirstPlayGame", 1) == 1;
            set => PlayerPrefs.SetInt("IsFirstPlayGame", value ? 1 : 0);
        }
        public bool IsFinishTutorial
        {
            get => PlayerPrefs.GetInt("IsFinishTutorial", 0) == 1;
            set => PlayerPrefs.SetInt("IsFinishTutorial", value ? 1 : 0);
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