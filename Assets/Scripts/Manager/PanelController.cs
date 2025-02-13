using System;
using System.Collections.Generic;
using DG.Tweening;
using pooling;
using UnityEngine;

namespace Game
{
    public enum Theme
    {
        Lon = 0,
        Egg = 1
    }
    public class PanelController : MonoBehaviour
    { 
        [SerializeField] private List<Transform> posObject;
        [SerializeField] private DataThemeObject dataThemeObject;
       private void OnEnable()
       {
           DOVirtual.DelayedCall(0.1f, delegate
           {
               transform.position = new Vector3(10f, transform.position.y, transform.position.z);
               transform.DOLocalMoveX(0f, 1f);
               UpdateChangeTheme(GameController.instance.CurrentTheme);
           });
       }
       public void UpdateChangeTheme(int id, bool enable = false)
       {
           if (enable)
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
           List<Transform> themeObjects = new List<Transform>(dataThemeObject.themeData[id].transforms);
           ShuffleList(themeObjects);
           for (int i = 0; i < posObject.Count; i++)
           {
               PoolingManager.Spawn(themeObjects[i], posObject[i].position, posObject[i].rotation, posObject[i].transform);
           }
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

       void Update()
       {
           if (Input.GetMouseButtonDown(0))
           {
               Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

               if (Physics.Raycast(ray, out RaycastHit hit) && GameController.instance.CanClick)
               {
                   GameController.instance.CanClick = false;
                   Item component;
                   if (hit.transform.TryGetComponent(out component))
                   {
                       for (int i = 0; i < GameController.instance.Board.amountObjects.Length; i++)
                       {
                           if (GameController.instance.Board.amountObjects[i].IsNone)
                           {
                               GameController.instance.Board.amountObjects[i].answer = component.Answer;
                               Transform t =  PoolingManager.Spawn(component.transform, component.transform.position, Quaternion.identity);
                                
                               GameController.instance.Board.amountObjects[i].currentItem = component.transform;
                               MoveAndRotateToPosition(t,  GameController.instance.Board.amountObjects[i], i);
                               break;
                           }
                       }
                   }
               }
           }
       }

       private void MoveAndRotateToPosition(Transform objToMove, HolderObject targetPos, int i)
       {
           float moveDuration = 0.5f;
           targetPos.IsNone = false;
           objToMove.DOJump(targetPos.transform.position + Vector3.up * 0.3f, 1f, 1, moveDuration)
                    .SetEase(Ease.Linear)
                    .OnComplete(delegate
                    {
                        objToMove.SetParent(targetPos.transform);
                        GameController.instance.Board.NextLine();
                    });

           objToMove.DORotate(new Vector3(360f, 180f, 0f), moveDuration, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear);
       }

       private void ReturnPos(Transform objToMove, Transform targetPos){
           float moveDuration = 1f;

           objToMove.DOMove(targetPos.transform.position, moveDuration).SetEase(Ease.Linear).OnComplete(() =>
           {
               GameController.instance.CanClick = true;
               objToMove.SetParent(targetPos);

               DOVirtual.DelayedCall(0.4f, () =>
               {
                   if (objToMove != null)
                   {
                       Destroy(objToMove.gameObject);
                   }
               });
           });
       }
    }
}
