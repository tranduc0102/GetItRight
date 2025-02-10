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

    public class GameController : MonoBehaviour
    {
        [SerializeField] private DataLevelGame dataLevelGame;
        [SerializeField] private Level currentLevel = null;
        [SerializeField] private List<HolderObject> posHolderObj;
        [SerializeField] private List<Transform> posReturn;
        private int IndexCurrentLevel
        {
            set => PlayerPrefs.SetInt("CurrentLevel", value);
            get => PlayerPrefs.GetInt("CurrentLevel", 0);
        }
        private void Start()
        {
            currentLevel = dataLevelGame.levels[IndexCurrentLevel];
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Item component;
                    if (hit.transform.TryGetComponent(out component))
                    {
                        for (int i = 0; i < posHolderObj.Count; i++)
                        {
                            if (posHolderObj[i].IsNone)
                            {
                                posHolderObj[i].answer = component.Answer;
                                MoveAndRotateToPosition(hit.transform, posHolderObj[i], i);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void MoveAndRotateToPosition(Transform objToMove, HolderObject targetPos, int i)
        {
            float moveDuration = 1f;
            float rotationDuration = 1f;
            posReturn.Add(objToMove.transform.parent);
            objToMove.DOMove(targetPos.transform.position, moveDuration).SetEase(Ease.Linear).OnComplete(() =>
            {
                targetPos.IsNone = false;
                objToMove.SetParent(targetPos.transform);
                if (targetPos.answer != currentLevel.answers[i])
                {
                    targetPos.IsNone = true;
                    DOVirtual.DelayedCall(1f, () =>
                    {
                        ReturnPos(objToMove, posReturn[i]);
                        posReturn.Remove(posReturn[i]);
                    });
                }
            });
        }
        private void ReturnPos(Transform objToMove, Transform targetPos){
            float moveDuration = 1f;
            float rotationDuration = 1f;

            objToMove.DOMove(targetPos.transform.position, moveDuration).SetEase(Ease.Linear).OnComplete(() =>
            {
                objToMove.SetParent(targetPos.transform);
            });
        }
    }
}