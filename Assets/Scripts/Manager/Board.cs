using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Extension;
using ACEPlay.Bridge;
using DG.Tweening;
using pooling;
using UnityEngine;

namespace Game
{
    public class Board : MonoBehaviour
    {
        [Space]
        [Header("Setting Board")]
        [SerializeField] private int countInRow;
        [SerializeField] public HolderItem[] amountObjects;
        [SerializeField] private int amountObjectReward;
        [SerializeField] private GameObject[] objectRewards;

        [SerializeField] private Material materialWhite;
        [SerializeField] private Material materialYes;
        [SerializeField] private Material materialNo;
        [SerializeField] private Material materialMaybe;

        [SerializeField] private float heightOfObjects;

        private int currentIndex;
        public int CurrentIndex
        {
            get => currentIndex;
            set
            {
                currentIndex += value;
                    
                if (currentIndex % countInRow == 0)
                {
                    GameController.Instance.CanClick = false;
                }
            }
        }
        bool win = false;
        private List<EnumAnswer> answers;

        private void Start()
        {
            DOVirtual.DelayedCall(0.2f, delegate
            {
                SetLevel(GameController.Instance.Answers);
            });
        }
        public void SetLevel(List<EnumAnswer> answersOther)
        {
            if (answers == null) answers = new List<EnumAnswer>();
            else answers.Clear();
            answers.AddRange(answersOther);
        }
        public void NextLine()
        {
            StartCoroutine(ProcessNextLine());
        }
        private IEnumerator ProcessNextLine()
        {
            if (currentIndex > amountObjects.Length) yield break;
            if (currentIndex % countInRow == 0)
            {
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(CheckAnswerInLine());
                if (GameController.Instance.IsTest)
                {
                    if (currentIndex + countInRow <= amountObjects.Length - amountObjectReward && !win)
                    {
                        for (int i = currentIndex - countInRow; i < currentIndex; i++)
                        {
                            int i1 = i;
                            amountObjects[i].transform.parent.DOLocalMoveX(0.2f, 1f);
                        }
                        for (int i = currentIndex; i < currentIndex + countInRow ; i++)
                        {
                            amountObjects[i].transform.parent.DOLocalMoveY(heightOfObjects, 1f);
                        }
                    }
                }
                if (win)
                {
                    GameController.Instance.IsWin = true;
                    yield break;
                }
                /*if (GameController.Instance.Mode == ModeGame.MultiPlayer)
                {
                    GameController.Instance.playerMoved = true;
                    GameController.Instance.PlayerManager.NextPlayerMovement();
                }*/
                if (GameController.Instance.AmountMove <= 0 || !win && currentIndex >= amountObjects.Length)
                {
                    UIController.instance.ShowDisplayLose(true);
                    BridgeController.instance.LogLevelFailWithParameter(PlayerPrefs.GetInt("CurrentLevel", 0));
                    yield break;
                }
                DOVirtual.DelayedCall(0.5f, delegate { GameController.Instance.CanClick = true; });
            }
            else
            {
                GameController.Instance.CanClick = true;
            }
        }
        private IEnumerator CheckAnswerInLine()
        {
            List<EnumAnswer> tempAnswers = new List<EnumAnswer>(answers);
            Material[] tempMaterials = new Material[countInRow];

            for (int index = currentIndex - countInRow; index < currentIndex; index++)
            {
                EnumAnswer currentAnswer = amountObjects[index].answer;
                EnumAnswer correctAnswer = GameController.Instance.Answers[index % countInRow];

                if (currentAnswer == correctAnswer)
                {
                    tempMaterials[index % countInRow] = materialYes;
                    tempAnswers.Remove(currentAnswer);
                }
            }

            for (int index = currentIndex - countInRow; index < currentIndex; index++)
            {
                EnumAnswer currentAnswer = amountObjects[index].answer;

                if (tempMaterials[index % countInRow] == null)
                {
                    if (tempAnswers.Contains(currentAnswer))
                    {
                        tempMaterials[index % countInRow] = materialMaybe;
                        tempAnswers.Remove(currentAnswer);
                    }
                    else
                    {
                        tempMaterials[index % countInRow] = materialNo;
                    }
                }
            }

            for (int index = currentIndex - countInRow; index < currentIndex; index++)
            {
                var meshRenderer = amountObjects[index].GetComponent<MeshRenderer>();
                if (meshRenderer.sharedMaterial != materialYes)
                {
                    meshRenderer.sharedMaterial = tempMaterials[index % countInRow];

                    if (tempMaterials[index % countInRow] == materialYes)
                    {
                        GameController.Instance.BoxManager.Boxes[index % countInRow].ShowBox(amountObjects[index].currentItem);
                    }
                    yield return new WaitForSeconds(0.5f);
                } 
            }
            int count = 0;
            for (int index = currentIndex - countInRow; index < currentIndex; index++)
            {
                if (tempMaterials.Contains(materialMaybe) || tempMaterials.Contains(materialNo))
                {
                    count++;
                    if (currentIndex >= amountObjects.Length) break;
                    amountObjects[index].GetComponent<MeshRenderer>().material = materialWhite;
                    if (GameController.Instance.IsTest)
                    {
                
                    }
                    else
                    {
                        GameController.Instance.PanelAnswerController.ReturnPos(amountObjects[index].currentItem, index);
                    }
                    yield return new WaitForSeconds(0.05f);
                }
            }
            if (!GameController.Instance.IsTest)
            {
                currentIndex = 0;
            }
            if (count == 0)
            {
                win = true;
                Vibration.Vibrate();
            }
        }
    }
}
