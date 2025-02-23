using System.Collections.Generic;
using System.Linq;
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
        public int CurrentTheme
        {
            get => PlayerPrefs.GetInt("CurrentThem", 0);
            set
            {
                if (value < 0) return;
                PlayerPrefs.SetInt("CurrentThem", value);
                currentPanel.UpdateChangeTheme(value, true);
            }
        }
        public LevelData CurrentLevelGame => currentLevel;

        [FormerlySerializedAs("_answers")] [SerializeField] private List<EnumAnswer> answers;
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

        [FormerlySerializedAs("IsTest1")] public bool isTest1;
        [FormerlySerializedAs("IsGameTest1")] public bool isGameTest1;
        [FormerlySerializedAs("IsGameTest2")] public bool isGameTest2;

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
                    playerManager.PlayAnim("Win");
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
        public ParticleSystem[] effectYesAnswer;
        [FormerlySerializedAs("effectNOAnswer")] public ParticleSystem[] effectNoAnswer;
        public EffectRewardCoin EffectCoin => effectCoin;

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
            InitializeGame();
        }

        private void InitializeGame()
        {
            currentLevel = CreateLevel.instance.GetLevelData(PlayerPrefs.GetInt("CurrentLevel", 1));
            GetAnswers(currentLevel.amountDistinct);
            SpawnBoard();
            SpawnPanel();
            boxManager.NextLevelOrReplay(currentLevel.amountBox);
            playerManager.MoveToTarget();
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
                    currentBoard.SaveMeBoard();
                });
                BridgeController.instance.ShowRewarded("SaveMe", e);
            }
        }

        private void PlayAgain()
        {
            currentPanel.gameObject.SetActive(false);
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
            IsWin = false;
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
            if (isTest1) return;
            int typeBoard = currentLevel.amountBox;
            int line = currentLevel.amountLine;

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

        private const float DistanceInGame1 = -4f;
        private const float DistancePanel1 = -7f;
        private const float DistanceInGame2 = -2f;
        private const float DistancePanel2 = -9f;
        private const float DistanceInGame3 = 0f;
        private const float DistancePanel3 = -11f;

        private void GetBoard(int id, Board[] boards)
        {
            switch (id)
            {
                case 3:
                    currentBoard = PoolingManager.Spawn(boards[0], table.position, Quaternion.identity, table);
                    inGame.position = new Vector3(0f, 0f, DistanceInGame1);
                    parentPanel.localPosition = new Vector3(0f, parentPanel.localPosition.y, DistancePanel1);
                    break;
                case 4:
                    currentBoard = PoolingManager.Spawn(boards[1], table.position, Quaternion.identity, table);
                    inGame.position = new Vector3(0f, 0f, DistanceInGame2);
                    parentPanel.localPosition = new Vector3(0f, parentPanel.localPosition.y, DistancePanel2);
                    break;
                case 5:
                    currentBoard = PoolingManager.Spawn(boards[2], table.position, Quaternion.identity, table);
                    inGame.position = new Vector3(0f, 0f, DistanceInGame3);
                    parentPanel.localPosition = new Vector3(0f, parentPanel.localPosition.y, DistancePanel3);
                    break;
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
    }
}