using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Extension;
using ACEPlay.Bridge;
using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class Board : MonoBehaviour
    {
        [Space]
        [Header("Setting Board")]
        [SerializeField] private int countInRow;
        [SerializeField] public HolderItem[] amountObjects;
        /*[SerializeField] private int amountObjectReward;
        [SerializeField] private GameObject[] objectRewards;*/

        [SerializeField] private Material materialWhite;
        [SerializeField] private Material materialYes;
        [SerializeField] private Material materialNo;
        [SerializeField] private Material materialMaybe;
        
        [Header("Setting pos spawn")]
        [SerializeField] private float heightOfObjects;

        private int currentIndex;

        bool win = false;
        private List<EnumAnswer> answers;
      
        private void Start()
        {
            currentIndex = 0;
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
            currentIndex += 1;
            GameController.Instance.CanClick = false;
            if (currentIndex > amountObjects.Length) yield break;
            if (currentIndex % countInRow == 0)
            {
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(CheckAnswerInLine());
                yield return new WaitForSeconds(0.5f);
                int bonus = 0;

                if (GameController.Instance.IsGameTest1)
                {
                    int index = currentIndex - countInRow;
                    for (; index < currentIndex;)
                    {
                        if (currentIndex + index % countInRow >= amountObjects.Length) break;
                        if (amountObjects[index].answer == GameController.Instance.Answers[index % countInRow] && !win && amountObjects[currentIndex + index % countInRow].IsNone)
                        {
                            if (currentIndex + index % countInRow < amountObjects.Length)
                            {
                                amountObjects[currentIndex + index % countInRow].answer = amountObjects[index].answer;
                                amountObjects[currentIndex + index % countInRow].IsNone = false;
                                amountObjects[currentIndex + index % countInRow].gameObject.SetActive(false);
                                bonus++;
                            }
                        }
                        index++;
                    }
                }
                DOVirtual.DelayedCall(0.9f, delegate
                {
                    if (GameController.Instance.IsGameTest2)
                    {
                        int tmp = currentIndex;
                        for (int index = tmp - countInRow; index < tmp; index++)
                        {
                            if (currentIndex + (index % countInRow) >= amountObjects.Length || index < 0) break;
                            if (amountObjects[index].answer == GameController.Instance.Answers[index % countInRow] && !win)
                            {
                                int targetIndex = currentIndex + (index % countInRow);

                                if (targetIndex < amountObjects.Length)
                                {
                                    amountObjects[targetIndex].answer = amountObjects[index].answer;
                                    amountObjects[targetIndex].IsNone = false;
                                    float high = 0f;
                                    if (amountObjects[index].currentItem.name.Contains("Egg"))
                                    {
                                        high = 0.003f;
                                    }
                                    Quaternion spawnRotation = Quaternion.Euler(-11, 180, 0);
                                    amountObjects[targetIndex].currentItem =
                                        Instantiate(amountObjects[index].currentItem,
                                                    new Vector3(amountObjects[targetIndex].transform.position.x,
                                                                amountObjects[targetIndex].transform.position.y,
                                                                amountObjects[targetIndex].transform.position.z), 
                                                    spawnRotation,
                                                    amountObjects[targetIndex].transform);

                                    amountObjects[targetIndex].currentItem.DOLocalMoveY(0.00312f + high, 0.5f);
                                    bonus++;
                                }
                            }
                        }
                    }
                });

                if (currentIndex + countInRow <= amountObjects.Length && !win)
                {
                    if (GameController.Instance.IsGameTest1)
                    {
                        amountObjects[currentIndex - 1].transform.parent.DOLocalMoveX(0.2f, 1f);
                        amountObjects[currentIndex].transform.parent.DOLocalMoveY(heightOfObjects, 1f);
                    }
                    if (GameController.Instance.IsGameTest2)
                    {
                        amountObjects[currentIndex].transform.parent.DOLocalMoveY(heightOfObjects, 1f).OnComplete(delegate
                        {
                            currentIndex += bonus;
                        });
                        foreach (FadeWithPropertyBlock fadeObject in amountObjects[currentIndex].transform.parent.GetComponentsInChildren<FadeWithPropertyBlock>())
                        {
                            fadeObject.FadeIn(0.6f);
                        }
                    }
                }
                if (win)
                {
                    GameController.Instance.IsWin = true;
                    yield break;
                }
                if (!win && currentIndex >= amountObjects.Length)
                {
                    GameController.Instance.PlayerManager.PlayAnim("ThatBai");
                    yield return new WaitForSeconds(0.7f);
                    UIController.instance.ShowDisplayLose(true);
                    BridgeController.instance.LogLevelFailWithParameter(PlayerPrefs.GetInt("CurrentLevel", 0));
                    yield break;
                }
                else
                {
                    GameController.Instance.PlayerManager.PlayAnim("DoanSai");
                }
                DOVirtual.DelayedCall(1f, delegate { GameController.Instance.CanClick = true; });
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
            if (GameController.Instance.IsGameTest1)
            {
                for (int index = currentIndex - countInRow; index < currentIndex; index++)
                {
                    var meshRenderer = amountObjects[index].GetComponent<MeshRenderer>();
                    if (meshRenderer.sharedMaterial != materialYes && amountObjects[index].gameObject.activeSelf)
                    {
                        meshRenderer.sharedMaterial = tempMaterials[index % countInRow];

                        if (tempMaterials[index % countInRow] == materialYes)
                        {
                            GameController.Instance.BoxManager.Boxes[index % countInRow].ShowBox(amountObjects[index].currentItem);
                        }
                        yield return new WaitForSeconds(0.5f);
                    }
                }
            }
            if (GameController.Instance.IsGameTest2)
            {
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
            }
            int count = 0;
            if (tempMaterials.Contains(materialMaybe) || tempMaterials.Contains(materialNo))
            {
                count++;
            }
            if (count == 0)
            {
                win = true;
                Vibration.Vibrate();
            }
        }
    }
}