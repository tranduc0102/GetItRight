using System;
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
        
        public void ShowBox(Transform answer)
        {
            if(!box.activeSelf) return;
            float hight = 0f;
            if (!answer.name.Contains("Can"))
            {
                hight = 0.5f;
            }
            PoolingManager.Spawn(answer, holderAnswer.position + Vector3.up*hight, Quaternion.Euler(0f, 180f, 0f), holderAnswer);
            box.SetActive(false);
            Transform t = Instantiate(explode, transform.position, Quaternion.identity);  
            EffectRewardCoin x = PoolingManager.Spawn(GameController.Instance.EffectCoin,holderAnswer.position + Vector3.up*hight, Quaternion.identity, UIController.instance.coinParent);
            Destroy(t.gameObject, 2f);
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
            box.SetActive(true);
        }
    }
}
