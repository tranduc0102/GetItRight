using System;
using UnityEngine;
using DG.Tweening; // Thêm namespace DOTween

public class SmoothCameraController : MonoBehaviour
{
    public float moveDuration = 2f;   
    public Vector3 positionOffset;
    public Vector3 rotationOffset;

    public void MoveAndRotateTo()
    {
        Vector3 targetPosition = positionOffset;
        Quaternion targetRotation = Quaternion.Euler(rotationOffset);

        transform.DOMove(targetPosition, moveDuration).SetEase(Ease.OutQuad);             
        transform.DORotateQuaternion(targetRotation, moveDuration).SetEase(Ease.OutQuad); 
    }
}