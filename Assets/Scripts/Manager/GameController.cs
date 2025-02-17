using System.Collections.Generic;
using UnityEngine;
using pooling;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
namespace Game
{
    public enum ModeGame
    {
        SinglePlayer,
        MultiPlayer,
    }

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
        
        /*
        [SerializeField] private List<Transform> posReturn;
        */
        
        [Space]
        [Header("Mode Game")]
        [SerializeField] private ModeGame mode = ModeGame.SinglePlayer;
        public ModeGame Mode => mode;
        [SerializeField] private bool canClick = true;
        public bool CanClick
        {
            get => canClick;
            set => canClick = value;
        }
        
        private int IndexCurrentLevel
        {
            set => PlayerPrefs.SetInt("CurrentLevel", value);
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
            PlayerPrefs.DeleteAll();
            UIController.instance.SetActionOnWin(NextLevel);
            UIController.instance.SetActionOnLose(PlayAgain);
            currentLevel = dataLevelGame.levels[IndexCurrentLevel];
            if (mode == ModeGame.SinglePlayer)
            {
                playerManager.SetNumberPlayer(3, false);
            }
            else
            {
                playerManager.SetNumberPlayer(4, true);
                /*
                ClickAction += playerManager.SetCanMove;
            */
            }
            GetAnswers();
            SpawnBoard();
            boxManager.NextLevelOrReplay(currentLevel.amountAnswers);
            playerManager.MoveToTarget();
        }
        private void PlayAgain()
        {
            pane.gameObject.SetActive(false);
            UIController.instance.ShowDisplayWin(false);
            UIController.instance.ShowDisplayLose(false);
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
            UIController.instance.ShowDisplayWin(false);
            UIController.instance.ShowDisplayLose(false);
            pane.gameObject.SetActive(true);
            
            IsWin = false;
            IndexCurrentLevel++;
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