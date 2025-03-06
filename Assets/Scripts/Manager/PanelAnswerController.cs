using System.Collections.Generic;
using _Scripts;
using _Scripts.UI;
using DG.Tweening;
using Lean.Touch;
using pooling;
using UnityEngine;

namespace Game
{
    public class PanelAnswerController : MonoBehaviour
    {
        [SerializeField] private List<Transform> posObject;
        [SerializeField] private List<Transform> objItem;
        [SerializeField] private DataThemeObject dataThemeObject;

        private List<Item> themeObjects = new List<Item>();
        private List<Item> results = new List<Item>();
        [SerializeField] private Camera cam;
        private bool canShowBtnSkip;

        private void Start()
        {
            cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        private void OnEnable()
        {
            transform.position = new Vector3(10f, transform.position.y, transform.position.z);
            InitializePanel();
            canShowBtnSkip = false;
            LeanTouch.OnFingerDown += HandleClick;
        }

        private void OnDisable()
        {
            ClearPanel();
            LeanTouch.OnFingerDown -= HandleClick;
        }

        private void InitializePanel()
        {
            results.Clear();
            if (GameController.Instance.IsFirstPlayGame && !GameController.Instance.IsFinishTutorial && PlayerPrefs.GetInt(USESTRING.CURRENT_LEVEL,1) == 1)
            {
                results.Clear();
                themeObjects.Clear();
                themeObjects.AddRange(dataThemeObject.themeData[0].items);
                results = new List<Item>(3)
                {
                    themeObjects[2],
                    themeObjects[1],
                    themeObjects[0]
                };
                SpawnResults();
                objItem.Reverse();
            }
            else
            {
                UpdateChangeTheme(GameController.Instance.CurrentSkin);
            }
            transform.DOLocalMoveX(0f, 1f).OnComplete(delegate
            {
                if (!GameController.Instance.IsFinishTutorial)
                {
                    GameController.Instance.UpdateStepsTutorial(1);
                }
            });
        }

        public void ResetState()
        {
            transform.DOLocalMoveX(10f, 1f).OnComplete(delegate
            {
                gameObject.SetActive(false);
            });
        }

        private void ClearPanel()
        {
            if (objItem.Count > 0)
            {
                foreach (var item in objItem)
                {
                    PoolingManager.Despawn(item.gameObject);
                }
                objItem.Clear();
            }
        }

        public void UpdateChangeTheme(int id, bool enable = false)
        {
            themeObjects.Clear();
            if (id >= dataThemeObject.themeData.Count) return;

            if (enable)
            {
                ClearChildren();
            }

            themeObjects.AddRange(dataThemeObject.themeData[id].items);
            FilterAndShuffleResults();
            SpawnResults();
        }

        private void ClearChildren()
        {
            foreach (Transform child in posObject)
            {
                foreach (Transform obj in child)
                {
                    if (obj != null)
                    {
                        PoolingManager.Despawn(obj.gameObject);
                    }
                }
            }
        }
        private void FilterAndShuffleResults()
        {
            results.Clear();
            foreach (var answer in themeObjects)
            {
                if (GameController.Instance.Answers.Contains(answer.Answer) && !results.Contains(answer))
                {
                    results.Add(answer);
                }
            }

            foreach (var answer in themeObjects)
            {
                if (results.Count < posObject.Count && !results.Contains(answer))
                {
                    results.Add(answer);
                }
            }

            ShuffleList(results);
        }

        private void SpawnResults()
        {
            for (int i = 0; i < Mathf.Min(posObject.Count, results.Count); i++)
            {
                objItem.Add(PoolingManager.Spawn(results[i].transform, posObject[i].position + Vector3.up, results[i].transform.rotation, posObject[i].transform));
            }
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
        private int indexTutorial = 0;
        private void HandleClick(LeanFinger leanFinger)
        {
            if (!GameController.Instance.CanClick)
            {
                return;
            }

            if (!canShowBtnSkip)
            {
                canShowBtnSkip = true;
                _Scripts.UI.UIController.instance.UIInGame.ShowSkipButton(true);
            }

            if (cam != null)
            {
                Ray ray = cam.ScreenPointToRay(leanFinger.StartScreenPosition);

                if (GameController.Instance.IsFirstPlayGame && !GameController.Instance.IsFinishTutorial)
                {
                    for (int i = 0; i < objItem.Count; i++)
                    {
                        if (i == GameController.Instance.IDCanClick)
                        {
                            objItem[i].GetComponent<Item>().CanMove = true;
                        }
                        else
                        {
                            objItem[i].GetComponent<Item>().CanMove = false;
                        }
                    }
                }
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform.parent.TryGetComponent(out Item componentParent) && componentParent.CanMove)
                    {
                        HandleItemSelection(componentParent);
                        if (!GameController.Instance.IsFinishTutorial)
                        {
                            switch (indexTutorial)
                            {
                                case 0:
                                    GameController.Instance.UpdateStepsTutorial(2);
                                    break;
                                case 1:
                                    GameController.Instance.UpdateStepsTutorial(3);
                                    break;
                                case 2:
                                    GameController.Instance.UpdateStepsTutorial(4);
                                    break;
                                case 3:
                                    GameController.Instance.UpdateStepsTutorial(5);
                                    break;
                                case 4:
                                    GameController.Instance.UpdateStepsTutorial(6);
                                    break;
                                case 5:
                                    GameController.Instance.UpdateStepsTutorial(6);
                                    break;
                            }
                            indexTutorial += 1;
                        }
                    }
                }
            }
        }
        private void HandleItemSelection(Item component)
        {
            for (int i = 0; i < GameController.Instance.Board.amountObjects.Length; i++)
            {
                if (GameController.Instance.Board.amountObjects[i].IsNone)
                {
                    GameController.Instance.Board.amountObjects[i].answer = component.Answer;
                    component.transform.DOScale(component.transform.localScale * 0.9f, 0.05f).SetLoops(2, LoopType.Yoyo);

                    Item t = PoolingManager.Spawn(component, component.transform.position, component.transform.rotation, GameController.Instance.Board.amountObjects[i].transform);
                    GameController.Instance.Board.amountObjects[i].currentItem = t.transform;
                    t.CanMove = false;
                    AudioManager.instance.PlaySoundClickObject();
                    t.transform.localScale = Vector3.one * 1.2f;
                    MoveAndRotateToPosition(t.transform, GameController.Instance.Board.amountObjects[i]);
                    break;
                }
            }
        }

        private void MoveAndRotateToPosition(Transform objToMove, HolderItem targetPos)
        {
            GameController.Instance.Board.NextLine();
            targetPos.IsNone = false;
            float high = 0.75f;
            float distance = 0f;
            objToMove.DOJump(targetPos.transform.position + new Vector3(0f, 1 * high, -distance), jumpPower: 0.5f, numJumps: 1, duration: 0.5f)
                .SetEase(Ease.OutQuad)
                .OnUpdate(delegate
                {
                    if (objToMove.TryGetComponent(out LookAtCamera look))
                    {
                        look.LookAt();
                    }
                })
                .OnComplete(delegate
                {
                    objToMove.transform.position = targetPos.transform.position + new Vector3(0f, 1 * high, -distance);
                    if (objToMove.TryGetComponent(out Item item))
                    {
                        item.CanMove = false;
                    }
                });
        }
    }
}