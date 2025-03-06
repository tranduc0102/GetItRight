using System;
using UnityEngine;
using DG.Tweening; // Thêm namespace DOTween

public class SmoothCameraController : MonoBehaviour
{
    public float moveDuration = 2f;   
    public Vector3 positionOffset;
    public Vector3 rotationOffset;

    public Vector3 originPosition;
    public Vector3 originRotation;

    public void MoveAndRotateTo()
    {
        Vector3 targetPosition = positionOffset;
        Quaternion targetRotation = Quaternion.Euler(rotationOffset);

        transform.DOMove(targetPosition, moveDuration).SetEase(Ease.OutQuad);             
        transform.DORotateQuaternion(targetRotation, moveDuration).SetEase(Ease.OutQuad); 
    }

    public void ResetState(Action onDone)
    {
        Vector3 targetPosition = originPosition;
        Quaternion targetRotation = Quaternion.Euler(originRotation);
        transform.DOMove(targetPosition, moveDuration).SetEase(Ease.OutQuad);             
        transform.DORotateQuaternion(targetRotation, moveDuration).SetEase(Ease.OutQuad).OnComplete(delegate
        {
            onDone?.Invoke();
        }); 
    }
}