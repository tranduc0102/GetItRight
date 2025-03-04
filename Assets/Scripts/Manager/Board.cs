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

        [SerializeField] private int currentIndex;
        public int CurrentIndex => currentIndex;
        private bool win;
        // 0,2,-1
        // 10,0,0
        // 0, 16.3  -18.3
        //43
        private List<EnumAnswer> answers;

        private void OnEnable()
        {
            InitializeBoard();
            win = false;
            showBox = false;
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
            if (currentIndex >= amountObjects.Length)
            {
                GameController.Instance.CanSkip = false;
            }

            if (currentIndex % countInRow == 0)
            {
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(CheckAnswerInLine());
                if (win)
                {
                    yield break;
                }
                if (!win && currentIndex >= amountObjects.Length)
                {
                    HandleLevelFail();
                    yield break;
                }
                int bonus = HandleBonusLogic();
                HandleBoardMovement(bonus);
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
            bonus = HandleBonusForInGame1();
            return bonus;
        }
        private int HandleBonusForInGame1()
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
                        amountObjects[targetIndex].currentItem = PoolingManager.Spawn(amountObjects[index].currentItem,
                            amountObjects[targetIndex].transform.position, amountObjects[index].currentItem.rotation, amountObjects[targetIndex].transform);
                        amountObjects[targetIndex].currentItem.localPosition = new Vector3(amountObjects[targetIndex].currentItem.localPosition.x, amountObjects[targetIndex].currentItem.localPosition.y, amountObjects[index].currentItem.transform.localPosition.z);
                        amountObjects[targetIndex].currentItem.DOLocalMoveY(amountObjects[index].currentItem.transform.localPosition.y, 0.5f).OnComplete(delegate
                        {
                            if(amountObjects[targetIndex].currentItem.TryGetComponent(out Item item))
                            {
                                item.CanMove = false;
                            }
                        });
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
                if (GameController.Instance.inGame2)
                {
                    amountObjects[currentIndex - 1].transform.parent.DOLocalMoveX(20f, 1f);
                    for (int i = currentIndex; i < amountObjects.Length; i+= countInRow)
                    {
                        amountObjects[i].transform.parent.DOLocalMoveY(amountObjects[i].transform.parent.localPosition.y + 0.7f, 0.5f);
                    }
                    currentIndex += bonus;
                }
                if (GameController.Instance.inGame1)
                {
                    amountObjects[currentIndex].transform.parent.gameObject.SetActive(true);
                    amountObjects[currentIndex].transform.parent.DOLocalMoveY(heightOfObjects, 1f).OnComplete(() =>
                    {
                        currentIndex += bonus;
                        if (GameController.Instance.IsFirstPlayGame)
                        {
                            if (currentIndex > 2 && currentIndex < 5)
                            {
                                GameController.Instance.UpdateStepsTutorial(5);
                            }else if (currentIndex >= 5)
                            {
                                GameController.Instance.UpdateStepsTutorial(7);
                            }
                        }
                    });
                }
            }
        }

        private void HandleLevelFail()
        {
            GameController.Instance.PlayerManager.PlayAnim(StateFace.ThatBai);
            _Scripts.UI.UIController.instance.UILevelFailed.ShowLevelFailedPanel(PlayerPrefs.GetInt(USESTRING.CURRENT_LEVEL, 1));
            BridgeController.instance.LogLevelFailWithParameter(PlayerPrefs.GetInt(USESTRING.CURRENT_LEVEL, 1));
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

            yield return HandleMaterialChangeForInGame1(tempMaterials);

            if (!tempMaterials.Contains(materialMaybe) && !tempMaterials.Contains(materialNo))
            {
                win = true;
                DOVirtual.DelayedCall(0.5f, () =>   GameController.Instance.IsWin = true);
                Vibration.Vibrate();
            }

            if (!showBox && !win)
            {
                AudioManager.instance.PlaySoundDoanSai();
                if (currentIndex < amountObjects.Length - 1)
                {
                    GameController.Instance.PlayerManager.PlayAnim(StateFace.DoanSai);
                }
            }
        }
        private IEnumerator HandleMaterialChangeForInGame1(Material[] tempMaterials)
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
                    if (GameController.Instance.IsFirstPlayGame)
                    {
                        if (tempMaterials[index % countInRow] == materialYes)
                        {
                            GameController.Instance.UpdateTextConnect(0);
                        }
                        else if (tempMaterials[index % countInRow] == materialMaybe)
                        {
                            GameController.Instance.UpdateTextConnect(1);
                        }
                        else
                        {
                            GameController.Instance.UpdateTextConnect(2);
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
            if (GameController.Instance.inGame2)
            {
                amountObjects[0].transform.parent.localPosition = new Vector3(0f, 5f, 0f);
            }
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
            if (GameController.Instance.inGame1)
            {
                ResetRemainingItemsInGame1();
            }
            else
            {
                ResetRemainingItemsInGame2();
            }
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
            amountObjects[index].currentItem = PoolingManager.Spawn(amountObjects[targetIndex].currentItem,
                amountObjects[targetIndex].transform.position, amountObjects[targetIndex].currentItem.rotation, amountObjects[index].transform);
            
            amountObjects[index].currentItem.DOLocalMoveZ(amountObjects[targetIndex].currentItem.transform.localPosition.z, 0.05f);
            amountObjects[index].currentItem.DOLocalMoveY(amountObjects[targetIndex].currentItem.transform.localPosition.y, 0.5f).OnComplete(delegate
            {
                if(amountObjects[index].currentItem.TryGetComponent(out Item item))
                {
                    item.CanMove = false;
                }
            });
        }

        private void ResetRemainingItemsInGame1()
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
            }
        }
        private void ResetRemainingItemsInGame2()
        {
            float posYOrigin = 5f;
            for (int index = countInRow; index < amountObjects.Length; index ++)
            {
                amountObjects[index].answer = EnumAnswer.None;
                amountObjects[index].IsNone = true;
                PoolingManager.Despawn(amountObjects[index].currentItem.gameObject);
                amountObjects[index].currentItem = null;
                amountObjects[index].transform.parent.localPosition = new Vector3(0f, posYOrigin, 0f);
                amountObjects[index].GetComponent<MeshRenderer>().material = materialWhite;
                if ((index % countInRow) == 0)
                {
                    posYOrigin -= 0.7f;
                }
            }
        }
    }
}