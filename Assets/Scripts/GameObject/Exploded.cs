using System.Collections;
using System.Collections.Generic;
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
            float epxlosionMinForce = 10f;
            float epxlosionMaxForce = 50f;
            float explosionForceRadius = 10f;
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
