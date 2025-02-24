using System;
using System.Collections.Generic;
using _Scripts;
using _Scripts.Extension;
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
        private void OnValidate()
        {
            cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        private void OnEnable()
        {
            DOVirtual.DelayedCall(0.2f, InitializePanel);
        }

        private void OnDisable()
        {
            ClearPanel();
        }

        private void InitializePanel()
        {
            results.Clear();
            transform.position = new Vector3(10f, transform.position.y, transform.position.z);
            UpdateChangeTheme(GameController.Instance.CurrentSkin);
            transform.DOLocalMoveX(0f, 1f);
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
            float high = results.Count > 0 && results[^1].name.Contains("Egg") ? 1f : 0;

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
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleClick();
            }
        }
        private void HandleClick()
        {
            if (GameController.Instance.playerMoved || !GameController.Instance.CanClick) return;

            if (cam != null)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform.TryGetComponent(out Item component) && component.CanMove)
                    {
                        HandleItemSelection(component);
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

                    Quaternion spawnRotation = Quaternion.Euler(0, 180, 0);
                    Item t = PoolingManager.Spawn(component, component.transform.position, spawnRotation, GameController.Instance.Board.amountObjects[i].transform);
                    GameController.Instance.Board.amountObjects[i].currentItem = t.transform;
                    t.CanMove = false;
                    AudioManager.instance.PlaySoundClickObject();
                    t.transform.localScale = t.transform.localScale * 1.2f;
                    MoveAndRotateToPosition(t.transform, GameController.Instance.Board.amountObjects[i]);
                    break;
                }
            }
        }

        private void MoveAndRotateToPosition(Transform objToMove, HolderItem targetPos)
        {
            GameController.Instance.Board.NextLine();
            targetPos.IsNone = false;
            float high = objToMove.name.Contains("Can") ? 0.14f : 0.6f;
            float distance = 0.15f;
            objToMove.DOJump(targetPos.transform.position + new Vector3(0f, 1 * high, -distance), jumpPower: 0.5f, numJumps: 1, duration: 0.5f)
                .SetEase(Ease.OutQuad)
                .OnComplete(delegate
                {
                    objToMove.transform.position = targetPos.transform.position + new Vector3(0f, 1 * high, -distance);
                });
            UIController.instance.ShowButtonShop(false);
        }
    }
}