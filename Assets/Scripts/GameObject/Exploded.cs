using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class Exploded : MonoBehaviour
    {
        [SerializeField] private Transform box;
        void Start()
        {
            ExplodeBox();
        }

        private void ExplodeBox()
        {
            float epxlosionMinForce = 50f;
            float epxlosionMaxForce = 100f;
            float explosionForceRadius = 15f;
            Vector3 explosionPosition = box.position;
            foreach (Transform obj in box)
            {
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.AddExplosionForce(Random.Range(epxlosionMinForce, epxlosionMaxForce), explosionPosition, explosionForceRadius);
            }
        }
    }
}
