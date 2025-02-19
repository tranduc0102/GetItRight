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

        public void ShowBox(Transform answer)
        {
            if(canOpen)return;
            GameController.Instance.PlayerManager.PlayAnim("DoanDung");
            canOpen = true;
            napThung.DOLocalRotate(new Vector3(-180f, -180f, 0f), .5f).OnComplete(delegate
            {
                float high = 0f;
                if (answer.name.Contains("Egg"))
                {
                    high = 0.004f;
                }
                effectOpen.Play();
                DOVirtual.DelayedCall(0.2f, delegate {
                    Transform t = PoolingManager.Spawn(answer, holderAnswer.position, Quaternion.Euler(0f, 180f, 0f), holderAnswer);
                    t.DOLocalMoveY(0.005f + high, .2f).SetEase(Ease.OutBounce); 
                });
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
            napThung.localRotation = Quaternion.Euler(0f, -180f, 0f);
            canOpen = false;
        }
    }
}
