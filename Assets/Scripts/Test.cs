using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Test : MonoBehaviour
{
   [SerializeField] private Vector3 startPosition;
   [SerializeField] private float direction;
   [SerializeField] private float duration;
   private void Start()
   {
      startPosition = transform.position;
      transform.position = new Vector3(startPosition.x + direction, startPosition.y, startPosition.z);
      transform.DOMove(startPosition, duration);
   }
}
