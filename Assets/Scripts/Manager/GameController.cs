using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
namespace Game
{
    [Serializable]
    public class HolderObject
    {
        public EnumAnswer answer;
        public Transform transform;
        public bool IsNone = true;
    }

    public enum ModeGame
    {
        SinglePlayer,
        MultiPlayer,
    }

    public class GameController : MonoBehaviour
    {
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private DataLevelGame dataLevelGame;
        [SerializeField] private Level currentLevel = null;
        [SerializeField] private List<HolderObject> posHolderObj;
        [SerializeField] private List<Transform> posReturn;
        [SerializeField] private ModeGame mode = ModeGame.SinglePlayer;

        [SerializeField] private bool canClick = true;
        private int IndexCurrentLevel
        {
            set => PlayerPrefs.SetInt("CurrentLevel", value);
            get => PlayerPrefs.GetInt("CurrentLevel", 0);
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
                        for (int i = 0; i < posHolderObj.Count; i++)
                        {
                            if (posHolderObj[i].IsNone)
                            {
                                posHolderObj[i].answer = component.Answer;
                                Transform t = Instantiate(component.gameObject, component.transform.position, Quaternion.identity, component.transform.parent).GetComponent<Transform>();
                                MoveAndRotateToPosition(t, posHolderObj[i], i);
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
            if (posHolderObj.Any(p => p.IsNone))
            {
                canClick = true;
            }
            objToMove.DOMove(targetPos.transform.position, moveDuration).SetEase(Ease.Linear).OnComplete(() =>
            {
                objToMove.SetParent(targetPos.transform);
                if(canClick) return;
                if (posHolderObj.Any(p => !p.IsNone))
                {
                    DOVirtual.DelayedCall(1f, () =>
                    {
                        bool check = false;
                        for (int j = 0; j < posHolderObj.Count; j++)
                        {
                            if (posHolderObj[j].answer != currentLevel.answers[j])
                            {
                                // spawn...
                                Item t = posHolderObj[j].transform.GetChild(0).GetComponent<Item>();
                                ReturnPos(t.transform, t.Parent);
                                posHolderObj[j].IsNone = true;
                                posHolderObj[j].answer = EnumAnswer.None;
                                check = true;
                            }
                            else
                            {
                                // spawn ...
                            }
                        }
                        if (check)
                        {
                            // spawn....
                            /*
                            playerManager.SetCanMove(true);
                            */
                            return;
                        }
                        canClick = true;
                    });
                }
            });
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
    }
}