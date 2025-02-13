using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
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
        
        [Space]
        [Header("Player")]
        [SerializeField] private PlayerManager playerManager;

        [Space]
        [Header("Panel Objects")]
        [SerializeField] private PanelController pane;
        
        [Space]
        [Header("Board Game")]
        [SerializeField] private BoardManager board;

        [Space] [Header("Box")] [SerializeField]
        private BoxManager boxManager;

        public BoxManager BoxManager => boxManager;
        public BoardManager Board => board;
        
        [SerializeField] private List<Transform> posReturn;
        
        [Space]
        [Header("Mode Game")]
        [SerializeField] private ModeGame mode = ModeGame.SinglePlayer;
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
        [Space]
        [Header("Win Game")]
        [SerializeField] private ParticleSystem effectWin;
        private bool isWin;
        public bool IsWin
        {
            get => isWin;
            set
            {
                isWin = value;
                if (isWin)
                {
                    effectWin.Play();
                }
            }
        }
        
        public Action<bool> ClickAction;
        
        [Space]
        [Header("Box Answer")]
        [SerializeField] private BoxManager box;
        public BoxManager Box => box;
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
            CurrentTheme = 0;
            currentLevel = dataLevelGame.levels[IndexCurrentLevel];
            if (mode == ModeGame.SinglePlayer)
            {
                playerManager.SetNumberPlayer(3, false);
            }
            else
            {
                playerManager.SetNumberPlayer(4, true);
                ClickAction += playerManager.SetCanMove;
            }
        }
        private void NextLevel()
        {
            
        }
    }
}