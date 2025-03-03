using System;
using _Scripts;
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
        public bool ShowBox(Transform answer)
        {
            if(canOpen) return false;
            GameController.Instance.PlayerManager.PlayAnim(StateFace.DoanDung);
            AudioManager.instance.PlaySoundDoanDung();
            canOpen = true;
            napThung.DOLocalRotate(new Vector3(-180f, 0f, 0f), 0.8f).OnComplete(delegate
            {
                float high = 0.0055f;
                effectOpen.Play();
                DOVirtual.DelayedCall(0.2f, delegate {
                    Transform t = PoolingManager.Spawn(answer, holderAnswer.position, answer.rotation, holderAnswer);
                    t.localScale = Vector3.one * 1.2f;
                    t.DOLocalMoveY(0.006f + high, .2f).SetEase(Ease.OutBack).OnComplete(delegate
                    {
                        if (t.TryGetComponent(out Item item))
                        {
                            item.CanMove = false;
                        }
                    }); 
                });
                napThung.gameObject.SetActive(false);
            });
            return true;
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
            transform.DOLocalMoveY(transform.localPosition.y - 0.3f, duration).SetEase(Ease.InQuad).OnComplete(delegate
            {
                AudioManager.instance.PlaySoundBoxFall();
            });
        }
    }
}
