using System;
using DG.Tweening;
using pooling;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class HolderBox : MonoBehaviour
    {
        [SerializeField] private GameObject box;
        [SerializeField] private Transform holderAnswer;
        [SerializeField] private Transform explode;
        public Transform napThung;
        private bool canMove;

        public void ShowBox(Transform answer)
        {
            if (GameController.Instance.IsTest)
            {
                if(canMove)return;
                canMove = true;
                napThung.DOLocalRotate(new Vector3(-180f, -180f, 0f), .2f).OnComplete(delegate
                {
                    float high = 0f;
                    if (!answer.name.Contains("Can"))
                    {
                    }
                    Transform t = PoolingManager.Spawn(answer, holderAnswer.position, Quaternion.Euler(0f, 180f, 0f), holderAnswer);
                    t.DOLocalMoveY(0.005f, .2f).SetEase(Ease.OutBounce);
                });
            }
            else
            {
                if(!box.activeSelf) return;
                float high = 0f;
                if (!answer.name.Contains("Can"))
                {
                    high = 0.5f;
                }
                PoolingManager.Spawn(answer, holderAnswer.position + Vector3.up*high, Quaternion.Euler(0f, 180f, 0f), holderAnswer);
                box.SetActive(false);
                Transform t = Instantiate(explode, transform.position, Quaternion.identity);  
                EffectRewardCoin x = PoolingManager.Spawn(GameController.Instance.EffectCoin,holderAnswer.position + Vector3.up, Quaternion.identity, UIController.instance.coinParent);
                Destroy(t.gameObject, 2f);   
            }
        }
        public void ResetBox()
        {
            if (holderAnswer.childCount > 0)
            {
                foreach (Transform child in holderAnswer)
                {
                    if(child.gameObject.activeSelf) PoolingManager.Despawn(child.gameObject);
                }
            }
            napThung.localRotation = Quaternion.Euler(0f, -180f, 0f);
            canMove = false;
            box.SetActive(true);
        }
    }
}
