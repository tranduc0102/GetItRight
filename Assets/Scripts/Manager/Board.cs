using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] public HolderObject[] amountObjects;
        [SerializeField] private int amountObjectReward;
        [SerializeField] private GameObject[] objectRewards;

        [SerializeField] private Material materialWhite;
        [SerializeField] private Material materialYes;
        [SerializeField] private Material materialNo;
        [SerializeField] private Material materialMaybe;

        [SerializeField] private float heightOfObjects;

        private int currentIndex;
        private int checkWinLose;
        private List<EnumAnswer> answers;

        private void Start()
        {
            checkWinLose = (amountObjects.Length - amountObjectReward) / countInRow;
            DOVirtual.DelayedCall(0.2f, delegate
            {
                SetLevel(GameController.instance.Answers);
            });
        }
        public void SetLevel(List<EnumAnswer> answersOther)
        {
            this.answers = new List<EnumAnswer>(answersOther);
        }

        public void NextLine()
        {
            StartCoroutine(ProcessNextLine());
        }

        private IEnumerator ProcessNextLine()
        {
            bool win = true;
            currentIndex++;
            int bonus = 0;

            if (currentIndex % countInRow == 0)
            {
                GameController.instance.CanClick = false;
                yield return new WaitForSeconds(0.5f);
                for (int index = currentIndex - countInRow; index < currentIndex; index++)
                {
                    if (index < 0) continue;
                    if (amountObjects[index].answer != GameController.instance.Answers[index % countInRow])
                    {
                        win = false;
                        break;
                    }
                }
                
                yield return StartCoroutine(CheckAnswerInLine());

                if (currentIndex + countInRow <= amountObjects.Length - amountObjectReward && !win)
                {
                    Sequence moveSequence = DOTween.Sequence();
                    for (int i = currentIndex; i < currentIndex + countInRow; i++)
                    {
                        moveSequence.Join(amountObjects[i].transform
                                                          .DOLocalMoveY(heightOfObjects, 0.5f)
                                                          .SetEase(Ease.Linear));
                    }

                    yield return moveSequence.WaitForCompletion();
                }

                // **Spawn Object in Next Line**
                for (int index = currentIndex - countInRow; index < currentIndex; index++)
                {
                    if (amountObjects[index].answer == GameController.instance.Answers[index % countInRow] && !win)
                    {
                        if (currentIndex + index % countInRow < amountObjects.Length - amountObjectReward)
                        {
                            amountObjects[currentIndex + index % countInRow].answer = amountObjects[index].answer;
                            amountObjects[currentIndex + index % countInRow].IsNone = false;
                            
                            amountObjects[currentIndex + index % countInRow].currentItem =
                                PoolingManager.Spawn(amountObjects[index].currentItem,
                                    new Vector3(
                                        amountObjects[currentIndex + index % countInRow].transform.position.x,
                                        amountObjects[currentIndex + index % countInRow].transform.position.y - 1f,
                                        amountObjects[currentIndex + index % countInRow].transform.position.z),
                                    new Quaternion(0f,180f,0f,0f), 
                                    amountObjects[currentIndex + index % countInRow].transform);
                            amountObjects[currentIndex + index % countInRow].currentItem.localScale = new Vector3(1f,1f,1f);
                            
                            amountObjects[currentIndex + index % countInRow].currentItem
                                .DOMoveY(amountObjects[currentIndex + index % countInRow].transform.position.y + 0.3f, 0.5f);
                            bonus++;
                        }
                    }
                }
                currentIndex += bonus;
                checkWinLose -= 1;

                if (win)
                {
                    Debug.Log("Win");
                    GameController.instance.IsWin = true;
                    yield break;
                }
                if (GameController.instance.Mode == ModeGame.MultiPlayer)
                {
                    GameController.instance.PlayerMoved = true;
                    GameController.instance.PlayerManager.NextPlayerMovement();
                }
                GameController.instance.CanClick = true;
                if (checkWinLose == 0)
                {
                    Debug.Log("Lose");
                    UIController.instance.ShowDisplayLose(true);
                }
            }
            else
            {
                GameController.instance.CanClick = true;
            }
        }

        private IEnumerator CheckAnswerInLine()
        {
            List<EnumAnswer> tempAnswers = new List<EnumAnswer>(answers); 
            Material[] tempMaterials = new Material[countInRow];
            for (int index = currentIndex - countInRow; index < currentIndex; index++)
            {
                EnumAnswer currentAnswer = amountObjects[index].answer;
                EnumAnswer correctAnswer = GameController.instance.Answers[index % countInRow];

                if (currentAnswer == correctAnswer)
                { 
                    tempMaterials[index % countInRow] = materialYes;
                    tempAnswers.Remove(currentAnswer); 
                }
            }
            
            for (int index = currentIndex - countInRow; index < currentIndex; index++)
            {
                EnumAnswer currentAnswer = amountObjects[index].answer;
                EnumAnswer correctAnswer = GameController.instance.Answers[index % countInRow];

                if (currentAnswer != correctAnswer)
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
                amountObjects[index].GetComponent<MeshRenderer>().material = tempMaterials[index % countInRow];
                if (tempMaterials[index % countInRow] == materialYes)
                {
                    GameController.instance.BoxManager.Boxes[index % countInRow].ShowBox(amountObjects[index].currentItem);
                }
                yield return new WaitForSeconds(0.5f);
            }
        }


        /*void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && amountObjectReward != 0 && objectRewards.Length > 0)
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
        }*/
        /*private void OnDestroy()
        {
            GameController.instance.CanClick = true;
        }*/
    }
}
