using System;
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
        [SerializeField] private List<Transform> objItem;
        [SerializeField] private DataThemeObject dataThemeObject;
        private List<Item> themeObjects = new List<Item>();
        private List<Item> results = new List<Item>();
        private void OnEnable()
        {
            DOVirtual.DelayedCall(0.2f, delegate
            {
                results.Clear();
                transform.position = new Vector3(10f, transform.position.y, transform.position.z);
                UpdateChangeTheme(GameController.Instance.CurrentTheme);
                transform.DOLocalMoveX(0f, 1f);
            });
        }
        private void OnDisable()
        {
            if (objItem.Count > 0)
            {
                for (int i = 0; i < objItem.Count; i++)
                {
                    PoolingManager.Despawn(objItem[i].gameObject);
                }
            }
            objItem.Clear();
        }
        public void UpdateChangeTheme(int id, bool enable = false)
        {
            themeObjects.Clear();
            if (id >= dataThemeObject.themeData.Count) return;
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
    
            foreach (var answer in themeObjects)
            {
                if (GameController.Instance.Answers.Contains(answer.Answer))
                {
                    if (!results.Contains(answer))
                    {
                        results.Add(answer);
                    }
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
    
            float high = results.Count > 0 && results[^1].name.Contains("Egg") ? 0.2f : 0;
    
            for (int i = 0; i < Mathf.Min(posObject.Count, results.Count); i++)
            {
                objItem.Add(PoolingManager.Spawn(results[i].transform, posObject[i].position + Vector3.up * high, posObject[i].rotation, posObject[i].transform));
            }
            Debug.LogWarning("Ok1");
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
                                    GameController.Instance.Board.amountObjects[i].currentItem = t.transform;
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
            UIController.instance.ShowButtonShop(false);
        }
    }
}