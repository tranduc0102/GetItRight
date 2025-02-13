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
            PoolingManager.Spawn(answer, holderAnswer.position, Quaternion.Euler(0f, 180f, 0f), holderAnswer);
            box.SetActive(false);
            Transform t = Instantiate(explode, transform.position, Quaternion.identity);
            Destroy(t.gameObject, 5f);
        }
        public void ResetBox()
        {
            /*if (holderAnswer.childCount > 0)
            {
                foreach (Transform obj in holderAnswer)
                {
                    Destroy(obj.gameObject);
                }
            }*/
            box.SetActive(true);
        }
    }
}
