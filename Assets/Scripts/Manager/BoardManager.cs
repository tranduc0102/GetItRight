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
                    if(index < 0)continue;
                    if (amountObjects[index].answer != GameController.instance.CurrentLevelGame.answers[index % countInRow])
                    {
                        win = false;
                        break;
                    }
                }
                for (int index = currentIndex - 3; index < currentIndex; index++)
                {
                        if (amountObjects[index].answer == GameController.instance.CurrentLevelGame.answers[index % countInRow] && !win)
                        {
                            
                        }
                }
                checkWinLose -= 1;
                if (win)
                {
                     Debug.Log("Win");
                     GameController.instance.IsWin = true;
                     return;
                }
                GameController.instance.ClickAction?.Invoke(true);
                GameController.instance.CanClick = true;
                if (checkWinLose == 0)
                {
                     GameController.instance.CanClick = false;
                     if (win) Debug.Log("Win");
                     else
                     {
                         Debug.Log("Lose");
                     }
                }
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
