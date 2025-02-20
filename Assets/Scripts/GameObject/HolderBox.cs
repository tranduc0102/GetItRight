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
        [SerializeField] private ParticleSystem effectOpen;
        public Transform napThung;
        private bool canOpen;
        [SerializeField] private float duration;
        public void ShowBox(Transform answer)
        {
            if(canOpen)return;
            GameController.Instance.PlayerManager.PlayAnim("DoanDung");
            canOpen = true;
            napThung.DOLocalRotate(new Vector3(-180f, 0f, 0f), 0.8f).OnComplete(delegate
            {
                float high = 0f;
                if (answer.name.Contains("Egg"))
                {
                    high = 0.004f;
                }
                effectOpen.Play();
                DOVirtual.DelayedCall(0.2f, delegate {
                    Transform t = PoolingManager.Spawn(answer, holderAnswer.position, Quaternion.Euler(-11.2f, 180f, 0f), holderAnswer);
                    t.DOLocalMoveY(0.006f + high, .2f).SetEase(Ease.OutBack); 
                });
                napThung.gameObject.SetActive(false);
            });
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
            napThung.gameObject.SetActive(true);
            napThung.localRotation = Quaternion.Euler(0f, 0f, 0f);
            canOpen = false;
            transform.localPosition += Vector3.up * 0.3f;
            transform.DOLocalMoveY(transform.localPosition.y - 0.3f, duration).SetEase(Ease.Linear);
        }
    }
}
