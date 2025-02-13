using DG.Tweening;
using pooling;
using UnityEngine;

namespace Game
{
    public class BoardManager : MonoBehaviour
    {
        [Space]
        [Header("Setting Board")]
        [SerializeField] private int countInRow;
        [SerializeField] public HolderObject[] amountObjects;
        [SerializeField] private int amountObjectReward;
        [SerializeField] private GameObject[] objectRewards;
      
        [SerializeField] private float heightOfObjects;

        private int currentIndex;
        private int checkWinLose;
        
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
                    if (index < 0) continue;
                    if (amountObjects[index].answer !=
                        GameController.instance.CurrentLevelGame.answers[index % countInRow])
                    {
                        win = false;
                        break;
                    }
                }

                /*if (currentIndex + 3 <= amountObjects.Length - amountObjectReward && !win)
                {
                    for (int i = currentIndex; i < currentIndex + 3; i++)
                    {
                        amountObjects[i].transform.parent.DOLocalMoveY(heightOfObjects, 0.5f);
                    }
                }*/
                for (int index = currentIndex - countInRow; index < currentIndex; index++)
                    {
                        if (amountObjects[index].answer ==
                            GameController.instance.CurrentLevelGame.answers[index % countInRow]){/*&& !win)
                        {*/
                            if (currentIndex + index % countInRow < amountObjects.Length - amountObjectReward)
                            {
                                GameController.instance.BoxManager.Boxes[index % countInRow].ShowBox(amountObjects[index].currentItem);
                                /*amountObjects[currentIndex + index % countInRow].answer = amountObjects[index].answer;
                                amountObjects[currentIndex + index % countInRow].IsNone = false;*/
                                /*amountObjects[currentIndex + index % countInRow].currentItem =
                                    PoolingManager.Spawn(amountObjects[index].currentItem,
                                        new Vector3(
                                            amountObjects[currentIndex + index % countInRow].transform.position.x,
                                            amountObjects[currentIndex + index % countInRow].transform.position.y - 1f,
                                            amountObjects[currentIndex + index % countInRow].transform.position.z),
                                        new Quaternion(0f,180f,0f,0f));
                                amountObjects[currentIndex + index % countInRow].currentItem
                                    .DOLocalMoveY(amountObjects[currentIndex + index % countInRow].transform.position.y,
                                        0.5f);*/
                                /*bonus++;*/
                            }
                        } /*else if (answers.Contains(amountObjects[index].answer))
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
                        else Debug.Log("Lose");
                    }

                /*DOVirtual.DelayedCall(0.6f, delegate
                {
                    
                });*/
            }
            else
            {
                GameController.instance.CanClick = true;
            }
        }

        void Update()
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
        }
        private void OnDestroy()
        {
            GameController.instance.CanClick = true;
        }
    }
}
