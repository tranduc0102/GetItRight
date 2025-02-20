using System.Collections.Generic;
using _Scripts.Extension;
using DG.Tweening;
using pooling;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class PanelAnswerController : MonoBehaviour
    {
        [SerializeField] private List<Transform> posObject;
        [SerializeField] private List<Vector3> posOrigin;
        [SerializeField] private List<Transform> objItem;
        [SerializeField] private DataThemeObject dataThemeObject;
        private List<Item> themeObjects = new List<Item>();
        private List<Item> results = new List<Item>();
        private void OnEnable()
        {
            results.Clear();
            if (posOrigin.Count > 0)
            {
                for (int i = 0; i < objItem.Count; i++)
                {
                    PoolingManager.Despawn(objItem[i].gameObject);
                }
                posOrigin.Clear();
            }
            DOVirtual.DelayedCall(0.1f, delegate
            {
                transform.position = new Vector3(10f, transform.position.y, transform.position.z);
                UpdateChangeTheme(GameController.Instance.CurrentTheme);
                transform.DOLocalMoveX(0f, 1f).OnComplete(delegate
                {
                    for (int i = 0; i < posObject.Count; i++)
                    {
                        posOrigin.Add(objItem[i].position);
                    }
                });
            });
        }
        public void UpdateChangeTheme(int id, bool enable = false)
        {
            themeObjects.Clear();
            if (id >= dataThemeObject.themeData.Count) return;
            objItem.Clear();
            if (enable)
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

            themeObjects.AddRange(dataThemeObject.themeData[id].items);
            results.Clear();
            if (results.Count <= 0)
            {
                foreach (var answer in themeObjects)
                {
                    for (int i = 0; i < GameController.Instance.Answers.Count; i++)
                    {
                        if (answer.Answer == GameController.Instance.Answers[i])
                        {
                            if (!results.Contains(answer))
                            {
                                results.Add(answer);
                                Debug.Log(GameController.Instance.Answers[i]);
                            }
                        }
                    }
                }
                
                foreach (var answer in themeObjects)
                {
                    if(results.Count < posObject.Count && !results.Contains(answer))
                    {
                        results.Add(answer);
                    }
                }
            }
            else
            {
                List<Item> newResualt = new List<Item>(results);
                results.Clear();
                for (int i = 0; i < newResualt.Count; i++)
                {
                    foreach (var item in newResualt)
                    {
                        if (themeObjects[i].Answer == item.Answer)
                        {
                            results.Add(themeObjects[i]);
                        }
                    }
                }
            }
            ShuffleList(results);
            float high = 0;
            if (results[^1].name.Contains("Egg"))
            {
                high = 0.2f;
            }
            for (int i = 0; i < posObject.Count; i++)
            {
                objItem.Add(PoolingManager.Spawn(results[i].transform, posObject[i].position + Vector3.up*high, posObject[i].rotation, posObject[i].transform));
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

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (GameController.Instance.playerMoved) return;
                if (UIDetection.IsPointerOverUIObject()) return;
                if (Camera.main != null)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out RaycastHit hit) && GameController.Instance.CanClick)
                    {
                        if (hit.transform.TryGetComponent(out Item component) && component.CanMove)
                        {
                            for (int i = 0; i < GameController.Instance.Board.amountObjects.Length; i++)
                            {
                                if (GameController.Instance.Board.amountObjects[i].IsNone)
                                {
                                    GameController.Instance.Board.amountObjects[i].answer = component.Answer;
                                    component.transform.DOScale(component.transform.localScale * 0.9f, 0.05f).SetLoops(2, LoopType.Yoyo);
                                    Item t;
                                    Quaternion spawnRotation = Quaternion.Euler(-11, 180, 0);

                                    t = PoolingManager.Spawn(component, component.transform.position, spawnRotation,
                                                             GameController.Instance.Board.amountObjects[i].transform);
                                    /*
                                    t.transform.localScale = t.transform.localScale * 1.1f;
                                    */
                                    /*else
                                    {
                                        t = PoolingManager.Spawn(component, component.transform.position, component.transform.rotation);
                                    }*/
                                    GameController.Instance.Board.amountObjects[i].currentItem = t.transform;
                                    if (!objItem.Contains(t.transform))
                                    {
                                        objItem.Add(t.transform);
                                        posOrigin.Add(t.transform.position);
                                    }
                                    t.CanMove = false;
                                    MoveAndRotateToPosition(t.transform, GameController.Instance.Board.amountObjects[i]);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void MoveAndRotateToPosition(Transform objToMove, HolderItem targetPos)
        { 
            GameController.Instance.Board.NextLine();
            float moveDuration = 0.5f;
            targetPos.IsNone = false;
            float high = 0.6f;
            if (objToMove.name.Contains("Can"))
            {
                high = 0.3f;
            }
            objToMove.DOJump(targetPos.transform.position + Vector3.up * high, 1f, 1, moveDuration)
                     .SetEase(Ease.Linear).OnComplete(delegate
                     {
                         objToMove.transform.position = targetPos.transform.position + Vector3.up * high;
                     });

            /*objToMove.DORotate(new Vector3(360f, 0f, 0f), moveDuration, RotateMode.FastBeyond360)
                     .SetEase(Ease.Linear);*/
            GameController.Instance.AmountMove -= 1;
            UIController.instance.ShowButtonShop(false);
        }

        public void ReturnPos(Transform objToMove, int i)
        {
            float moveDuration = 1f;

            int index = objItem.IndexOf(objToMove);
            if (index >= 0 && index < posOrigin.Count)
            {
                Vector3 targetPos = posOrigin[index];

                objToMove.DOMove(targetPos, moveDuration).SetEase(Ease.Linear).OnComplete(() =>
                {
                    objToMove.localEulerAngles = Vector3.zero;

                    GameController.Instance.CanClick = true;

                    objToMove.GetComponent<Item>().CanMove = true;
                    GameController.Instance.Board.amountObjects[i].answer = EnumAnswer.None;
                    GameController.Instance.Board.amountObjects[i].IsNone = true;
                    PoolingManager.Despawn(objToMove.gameObject);
                });
            }
            else
            {
                Debug.LogWarning("Không tìm thấy vị trí gốc cho object: " + objToMove.name);
            }
        }

    }
}