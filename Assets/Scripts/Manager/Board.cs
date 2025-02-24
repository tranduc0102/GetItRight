using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts;
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

        [SerializeField] private Material materialWhite;
        [SerializeField] private Material materialYes;
        [SerializeField] private Material materialNo;
        [SerializeField] private Material materialMaybe;
        
        private bool showBox;

        [Header("Setting pos spawn")]
        [SerializeField] private float heightOfObjects;

        private int currentIndex;
        private bool win;
        private List<EnumAnswer> answers;

        private void Start()
        {
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            currentIndex = 0;
            DOVirtual.DelayedCall(0.2f, () => SetLevel(GameController.Instance.Answers));
        }

        public void SetLevel(List<EnumAnswer> answersOther)
        {
            answers = answersOther ?? new List<EnumAnswer>();
        }

        public void NextLine()
        {
            StartCoroutine(ProcessNextLine());
        }

        private IEnumerator ProcessNextLine()
        {
            currentIndex++;
            GameController.Instance.CanClick = false;

            if (currentIndex > amountObjects.Length) yield break;

            if (currentIndex % countInRow == 0)
            {
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(CheckAnswerInLine());
                yield return new WaitForSeconds(0.5f);

                int bonus = HandleBonusLogic();
                HandleBoardMovement(bonus);

                if (win)
                {
                    GameController.Instance.IsWin = true;
                    yield break;
                }

                if (!win && currentIndex >= amountObjects.Length)
                {
                    HandleLevelFail();
                    yield break;
                }

                DOVirtual.DelayedCall(1f, () => GameController.Instance.CanClick = true);
            }
            else
            {
                GameController.Instance.CanClick = true;
            }
        }

        private int HandleBonusLogic()
        {
            int bonus = 0;
            if (GameController.Instance.isGameTest1)
            {
                bonus = HandleBonusForGameTest1();
            }
            if (GameController.Instance.isGameTest2)
            {
                bonus = HandleBonusForGameTest2();
            }
            return bonus;
        }

        private int HandleBonusForGameTest1()
        {
            int bonus = 0;
            int index = currentIndex - countInRow;
            while (index < currentIndex)
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
            return bonus;
        }

        private int HandleBonusForGameTest2()
        {
            int bonus = 0;
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
                        float high = amountObjects[index].currentItem.name.Contains("Egg") ? 0.003f : 0f;
                        Quaternion spawnRotation = Quaternion.Euler(-11, 180, 0);
                        amountObjects[targetIndex].currentItem = Instantiate(amountObjects[index].currentItem,
                            amountObjects[targetIndex].transform.position, spawnRotation, amountObjects[targetIndex].transform);
                        amountObjects[targetIndex].currentItem.DOLocalMoveY(0.00312f + high, 0.5f);
                        bonus++;
                    }
                }
            }
            return bonus;
        }

        private void HandleBoardMovement(int bonus)
        {
            if (currentIndex + countInRow <= amountObjects.Length && !win)
            {
                if (GameController.Instance.isGameTest1)
                {
                    amountObjects[currentIndex - 1].transform.parent.DOLocalMoveX(0.2f, 1f);
                    amountObjects[currentIndex].transform.parent.DOLocalMoveY(heightOfObjects, 1f);
                }
                if (GameController.Instance.isGameTest2)
                {
                    amountObjects[currentIndex].transform.parent.DOLocalMoveY(heightOfObjects, 1f).OnComplete(() => currentIndex += bonus);
                    foreach (var fadeObject in amountObjects[currentIndex].transform.parent.GetComponentsInChildren<FadeWithPropertyBlock>())
                    {
                        fadeObject.FadeIn(0.6f);
                    }
                }
            }
        }

        private void HandleLevelFail()
        {
            GameController.Instance.PlayerManager.PlayAnim("ThatBai");
            UIController.instance.ShowDisplayLevelFail(true);
            BridgeController.instance.LogLevelFailWithParameter(PlayerPrefs.GetInt("CurrentLevel", 1));
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

            if (GameController.Instance.isGameTest1)
            {
                yield return HandleMaterialChangeForGameTest1(tempMaterials);
            }
            if (GameController.Instance.isGameTest2)
            {
                yield return HandleMaterialChangeForGameTest2(tempMaterials);
            }

            if (!tempMaterials.Contains(materialMaybe) && !tempMaterials.Contains(materialNo))
            {
                win = true;
                Vibration.Vibrate();
            }

            if (!showBox)
            {
                GameController.Instance.PlayerManager.PlayAnim("DoanSai");
                GameController.Instance.effectNoAnswer[0].Play();
            }
        }

        private IEnumerator HandleMaterialChangeForGameTest1(Material[] tempMaterials)
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

        private IEnumerator HandleMaterialChangeForGameTest2(Material[] tempMaterials)
        {
            int idSound = 0;
            showBox = false;
            for (int index = currentIndex - countInRow; index < currentIndex; index++)
            {
                var meshRenderer = amountObjects[index].GetComponent<MeshRenderer>();
                if (meshRenderer.sharedMaterial != materialYes)
                {
                    meshRenderer.sharedMaterial = tempMaterials[index % countInRow];

                    if (tempMaterials[index % countInRow] == materialYes)
                    {
                        AudioManager.instance.PlaySoundConnect(idSound);
                        idSound += 1;
                        if (GameController.Instance.BoxManager.Boxes[index % countInRow].ShowBox(amountObjects[index].currentItem))
                        {
                            showBox = true;
                        }
                    }
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }

        public void SaveMeBoard()
        {
            currentIndex = 0;
            win = false;
            int bonus = 0;

            for (int index = 0; index < countInRow; index++)
            {
                ResetHolderItem(index);
                int targetIndex = amountObjects.Length - countInRow + (index % countInRow);
                if (amountObjects[targetIndex].answer == GameController.Instance.Answers[index % countInRow] && !win)
                {
                    ResetAndSpawnItem(index, targetIndex);
                    bonus++;
                }
            }

            currentIndex += bonus;
            ResetRemainingItems();
        }

        private void ResetHolderItem(int index)
        {
            amountObjects[index].answer = EnumAnswer.None;
            amountObjects[index].IsNone = true;
            PoolingManager.Despawn(amountObjects[index].currentItem.gameObject);
            amountObjects[index].currentItem = null;
            amountObjects[index].GetComponent<MeshRenderer>().material = materialWhite;
        }

        private void ResetAndSpawnItem(int index, int targetIndex)
        {
            amountObjects[index].answer = amountObjects[targetIndex].answer;
            amountObjects[index].IsNone = false;
            Quaternion spawnRotation = Quaternion.Euler(-11, 180, 0);
            amountObjects[index].currentItem = PoolingManager.Spawn(amountObjects[targetIndex].currentItem,
                amountObjects[index].transform.position, spawnRotation, amountObjects[index].transform);
            amountObjects[index].currentItem.DOLocalMoveY(0.00312f, 0.5f);
        }

        private void ResetRemainingItems()
        {
            for (int index = countInRow; index < amountObjects.Length; index++)
            {
                amountObjects[index].answer = EnumAnswer.None;
                amountObjects[index].IsNone = true;
                PoolingManager.Despawn(amountObjects[index].currentItem.gameObject);
                amountObjects[index].currentItem = null;
                amountObjects[index].transform.parent.localPosition = new Vector3(0f, 1.9f, amountObjects[index].transform.parent.localPosition.z);
                amountObjects[index].GetComponent<MeshRenderer>().material = materialWhite;
                amountObjects[index].transform.parent.gameObject.SetActive(false);
                amountObjects[index].transform.parent.gameObject.SetActive(true);
            }
        }
    }
}