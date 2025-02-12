using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class HolderObject
    {
        public EnumAnswer answer;
        public Transform transform;
        public bool IsNone = true;
        public Transform currentItem;
    }

    public class BoardManager : MonoBehaviour
    {
        [SerializeField] private int countInRow;
        [SerializeField] private HolderObject[] amountObjects;

        [SerializeField] private int amountObjectReward;
        [SerializeField] private GameObject[] objectRewards;
        public HolderObject[] AmountObjects => amountObjects;

        [SerializeField] private float heightOfObjects;
        [SerializeField] private int currentIndex;
        private int checkWinLose;
        
        [Space]
        [Header("Tips")]
        [SerializeField] private Material[] tipMaterials;

        private void Start()
        {
            checkWinLose = (amountObjects.Length - amountObjectReward) / countInRow;
        }
        public void NextLine()
        {
            bool win = true;
            currentIndex++;
            int bonus = 0;
            if (currentIndex % countInRow == 0)
            {
                GameController.instance.CanClick = false;
                for (int index = currentIndex - 3; index < currentIndex; index++)
                {
                    if(index < 0)continue;
                    if (amountObjects[index].answer != GameController.instance.CurrentLevelGame.answers[index % countInRow])
                    {
                        win = false;
                        break;
                    }
                }
                if (currentIndex + 3 <= amountObjects.Length - amountObjectReward && !win)
                {
                    for (int i = currentIndex; i < currentIndex + 3 ; i++)
                    {
                        amountObjects[i].transform.parent.DOLocalMoveY(heightOfObjects, 0.5f);
                    }
                }
                DOVirtual.DelayedCall(0.6f, delegate
                {
                    for (int index = currentIndex - 3; index < currentIndex; index++)
                    {
                        if (amountObjects[index].answer == GameController.instance.CurrentLevelGame.answers[index % countInRow] && !win)
                        {
                            if (currentIndex + index % countInRow < amountObjects.Length - amountObjectReward)
                            {
                                amountObjects[currentIndex + index % countInRow].answer = amountObjects[index].answer;
                                amountObjects[currentIndex + index % countInRow].IsNone = false;
                                amountObjects[currentIndex + index % countInRow].currentItem = 
                                    Instantiate(amountObjects[index].currentItem, new Vector3( amountObjects[currentIndex + index % countInRow].transform.position.x, amountObjects[currentIndex + index % countInRow].transform.position.y - 1f,  amountObjects[currentIndex + index % countInRow].transform.position.z), Quaternion.identity);
                                amountObjects[currentIndex + index % countInRow].currentItem.DOLocalMoveY(amountObjects[currentIndex + index % countInRow].transform.position.y, 0.5f);
                                bonus++;
                            }
                        }/*else if (answers.Contains(amountObjects[index].answer))
                        {
                            
                        }
                        else
                        {
                            
                        }*/
                    }
                    currentIndex += bonus;
                    checkWinLose -= 1;
                    if (win)
                    {
                        Debug.Log("Win");
                        GameController.instance.IsWin = true;
                        return;
                    }
                    GameController.instance.CanClick = true;
                    if (checkWinLose == 0)
                    {
                        if (win) Debug.Log("Win");
                        else
                        {
                            if(amountObjectReward == 0) return;
                        }
                    }
                });
            }
            else
            {
                GameController.instance.CanClick = true;
            }
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && amountObjectReward != 0)
            {
                checkWinLose = 1;
                for (int i = amountObjects.Length - amountObjectReward; i < amountObjects.Length; i++)
                {
                    amountObjects[i].transform.parent.gameObject.SetActive(true);
                }
                amountObjectReward = 0;
                currentIndex -= 1;
                NextLine();
                amountObjectReward = 0;
            }
        }
        private void OnDestroy()
        {
            GameController.instance.CanClick = true;
        }
    }
}
