using System.Collections.Generic;
using ACEPlay.Bridge;
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
        [SerializeField] private DataLevelGame dataLevelGame;
        [SerializeField] private Level currentLevel;
        public int CurrentTheme
        {
            get => PlayerPrefs.GetInt("CurrentThem", 0);
            set
            {
                if(value < 0) return;
                PlayerPrefs.SetInt("CurrentThem", value);
                pane.UpdateChangeTheme(value, true);
            }
        }
        public Level CurrentLevelGame => currentLevel;
        [SerializeField] private int amountMove;
        public int AmountMove {
            get => amountMove;
            set
            {
                amountMove = value;
                UIController.instance.UpdateTextMove(value);
            }
        }
        [FormerlySerializedAs("_answers")] [SerializeField] private List<EnumAnswer> answers;
        public List<EnumAnswer> Answers => answers;
        
        [Space]
        [Header("Player")]
        [SerializeField] private PlayerManager playerManager; 
        public PlayerManager PlayerManager => playerManager;
        [FormerlySerializedAs("PlayerMoved")] public bool playerMoved = true;

        [Space]
        [Header("Panel Objects")]
        [SerializeField] private PanelAnswerController pane;
        public PanelAnswerController PanelAnswerController => pane;

        [Space]
        [Header("Board Game")]
        [SerializeField] private Transform table;
        [SerializeField] private Board[] boardDat;
        [SerializeField] private Board currentBoard;
        public Board Board => currentBoard;
        public bool IsTest1;
        public bool IsGameTest1;
        
        /*
        [SerializeField] private List<Transform> posReturn;
        */
        
        /*
        [Space]
        [Header("Mode Game")]
        [SerializeField] private ModeGame mode = ModeGame.SinglePlayer;
        public ModeGame Mode => mode;*/
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
                    playerManager.AnimWin();
                    UIController.instance.ShowDisplayWin(true);
                    pane.gameObject.SetActive(false);
                }
            }
        }
        
        [Space]
        [Header("Box Answer")]
        [SerializeField] private BoxManager boxManager;
        public BoxManager BoxManager => boxManager;
        
        [Space]
        [Header("EffectCoin")]
        [SerializeField] private EffectRewardCoin effectCoin;
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
            currentLevel = dataLevelGame.levels[IndexCurrentLevel];
            GetAnswers();
            SpawnBoard();
            boxManager.NextLevelOrReplay(currentLevel.amountAnswers);
            playerManager.MoveToTarget();
            BridgeController.instance.LogLevelStartWithParameter(PlayerPrefs.GetInt("CurrentLevel", 0));
        }
        private void SaveLevelFail()
        {
            if (BridgeController.instance.IsRewardReady())
            {
                UnityEvent e = new UnityEvent();
                e.AddListener(delegate
                {
                    IsWin = false;
                    AmountMove = currentLevel.amountAnswers * 2;
                    canClick = true;
                });
                BridgeController.instance.ShowRewarded("SaveMe", e);
            }
        }
        private void PlayAgain()
        {
            pane.gameObject.SetActive(false);
            pane.gameObject.SetActive(true);
            IsWin = false;
            currentLevel = dataLevelGame.levels[IndexCurrentLevel];
            canClick = true;
            GetAnswers();
            Destroy(currentBoard.gameObject);
            SpawnBoard();
            boxManager.NextLevelOrReplay(currentLevel.amountAnswers);
        }
        private void NextLevel()
        {
            pane.gameObject.SetActive(true);
            IsWin = false;
            IndexCurrentLevel++;
            BridgeController.instance.LogLevelCompleteWithParameter(PlayerPrefs.GetInt("CurrentLevel", 0));
            currentLevel = dataLevelGame.levels[IndexCurrentLevel];
            canClick = true;
            GetAnswers();
            Destroy(currentBoard.gameObject);
            SpawnBoard();
            boxManager.NextLevelOrReplay(currentLevel.amountAnswers);
        }
        private void GetAnswers()
        {
            answers.Clear();
            EnumAnswer randomSameValue = (EnumAnswer)Random.Range(1, 7);
            for (int i = 0; i < currentLevel.amountSameValue; i++)
            {
                answers.Add(randomSameValue);
            }
            for (int i = 0; i < currentLevel.amountValueRemain; )
            {
                EnumAnswer randomValue = (EnumAnswer)Random.Range(1, 7);
                if (randomValue != randomSameValue)
                {
                    answers.Add(randomValue);
                    i++;
                }
            }
            AmountMove = currentLevel.amountMove;
            ShuffleList(answers);
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
            if(IsTest1) return;
            switch (currentLevel.amountAnswers)
            {
                case 3:
                    currentBoard = PoolingManager.Spawn(boardDat[0], table.position, Quaternion.identity, table);
                    break;
                case 4:
                    currentBoard = PoolingManager.Spawn(boardDat[1], table.position, Quaternion.identity, table);
                    break;
                case 5:
                    currentBoard = PoolingManager.Spawn(boardDat[2], table.position, Quaternion.identity, table);
                    break;
                case 6:
                    currentBoard = PoolingManager.Spawn(boardDat[3], table.position, Quaternion.identity, table);
                    break;
            }
        }
    }
}