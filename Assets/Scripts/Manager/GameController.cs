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
        public Level CurrentLevelGame => currentLevel;
        
        [Space]
        [Header("Player")]
        [SerializeField] private PlayerManager playerManager;
        
        [Space]
        [Header("Board Game")]
        [SerializeField] private BoardManager board;
        
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
        [SerializeField] private GameObject effectWin;
        private bool isWin;
        public bool IsWin
        {
            get => isWin;
            set
            {
                isWin = value;
                effectWin.gameObject.SetActive(value);
            }
        }
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
            currentLevel = dataLevelGame.levels[IndexCurrentLevel];
            if (mode == ModeGame.SinglePlayer)
            {
                playerManager.SetNumberPlayer(3, false);
            }
            else
            {
                playerManager.SetNumberPlayer(4, true);
            }
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit) && canClick)
                {
                    canClick = false;
                    Item component;
                    if (hit.transform.TryGetComponent(out component))
                    {
                        for (int i = 0; i < board.AmountObjects.Length; i++)
                        {
                            if (board.AmountObjects[i].IsNone)
                            {
                                board.AmountObjects[i].answer = component.Answer;
                                Transform t = Instantiate(component.gameObject, component.transform.position, Quaternion.identity).GetComponent<Transform>();
                                
                                board.AmountObjects[i].currentItem = t;
                                MoveAndRotateToPosition(t, board.AmountObjects[i], i);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void MoveAndRotateToPosition(Transform objToMove, HolderObject targetPos, int i)
        {
            float moveDuration = 0.5f;
            targetPos.IsNone = false;
            board.NextLine();
            objToMove.DOJump(targetPos.transform.position, 1f,1, moveDuration).SetEase(Ease.Linear);
        }
        private void ReturnPos(Transform objToMove, Transform targetPos){
            float moveDuration = 1f;

            objToMove.DOMove(targetPos.transform.position, moveDuration).SetEase(Ease.Linear).OnComplete(() =>
            {
                canClick = true;
                objToMove.SetParent(targetPos);

                DOVirtual.DelayedCall(0.4f, () =>
                {
                    if (objToMove != null)
                    {
                        Destroy(objToMove.gameObject);
                    }
                });
            });
        }
        private void NextLevel()
        {
            
        }
    }
}