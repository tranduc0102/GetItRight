using System;
using DG.Tweening;
using pooling;
using UnityEngine;

namespace Game
{
    public class MovementObject : MonoBehaviour
    {
        [SerializeField] private Vector3 startPosition;
        [SerializeField] private float direction;
        [SerializeField] private float duration;
        [SerializeField] private bool isTable;
        private void Start()
        {
            startPosition = transform.position;
            transform.position = new Vector3(startPosition.x + direction, startPosition.y, startPosition.z);
            transform.DOMove(startPosition, duration);
        }

        public void ResetState()
        {
            transform.DOMove(new Vector3(-direction * 1.5f, startPosition.y, startPosition.z), duration)
                .OnComplete(delegate
                {
                    if (isTable)
                    {
                        gameObject.SetActive(false);
                    }
                    else
                    {
                       Destroy(transform.parent.gameObject);
                    }
                });
        }
    }
}