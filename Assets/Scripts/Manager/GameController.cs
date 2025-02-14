using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using pooling;
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
        public static GameController instance;
        [Space]
        [Header("DataGame")]
        [SerializeField] private DataLevelGame dataLevelGame;
        [SerializeField] private Level currentLevel = null;
        public int CurrentTheme
        {
            get {return PlayerPrefs.GetInt("CurrentThem", 0); }
            set
            {
                if(value < 0) return;
                PlayerPrefs.SetInt("CurrentThem", value);
            }
        }
        public Level CurrentLevelGame => currentLevel;
        [SerializeField] private List<EnumAnswer> _answers;
        public List<EnumAnswer> Answers => _answers;
        
        [Space]
        [Header("Player")]
        [SerializeField] private PlayerManager playerManager; 
        public PlayerManager PlayerManager => playerManager;
        public bool PlayerMoved = true;

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
                }
            }
        }
        
        [Space]
        [Header("Box Answer")]
        [SerializeField] private BoxManager boxManager;
        public BoxManager BoxManager => boxManager;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                return;
            }
            Destroy(this);
        }
        private void Start()
        {
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
            UIController.instance.ShowDisplayWin(false);
            UIController.instance.ShowDisplayLose(false);
            IsWin = false;
            currentLevel = dataLevelGame.levels[IndexCurrentLevel];
            canClick = true;
            GetAnswers();
            /*
            PoolingManager.Despawn(currentLevel.);
            */
            Destroy(currentBoard.gameObject);
            SpawnBoard();
            boxManager.NextLevelOrReplay(currentLevel.amountAnswers);
        }
        private void NextLevel()
        {
            UIController.instance.ShowDisplayWin(false);
            UIController.instance.ShowDisplayLose(false);
            
            IsWin = false;
            IndexCurrentLevel++;
            currentLevel = dataLevelGame.levels[IndexCurrentLevel];
            canClick = true;
            GetAnswers();
            /*
            PoolingManager.Despawn(currentLevel.);
            */
            Destroy(currentBoard.gameObject);
            SpawnBoard();
            boxManager.NextLevelOrReplay(currentLevel.amountAnswers);
        }
        private void GetAnswers()
        {
            _answers.Clear();
            EnumAnswer randomSameValue = (EnumAnswer)Random.Range(1, 7);
            for (int i = 0; i < currentLevel.amountSameValue; i++)
            {
                _answers.Add(randomSameValue);
            }
            for (int i = 0; i < currentLevel.amountValueRemain; )
            {
                EnumAnswer randomValue = (EnumAnswer)Random.Range(1, 7);
                if (randomValue != randomSameValue)
                {
                    _answers.Add(randomValue);
                    i++;
                }
            }
            ShuffleList(_answers);
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