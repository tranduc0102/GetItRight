using UnityEngine;
using System.Collections;

public class SmoothCameraController : MonoBehaviour
{
    public float moveDuration = 2f;   
    public Vector3 positionOffset;
    public Vector3 rotationOffset;
    
    public void MoveAndRotateTo()
    {
        StartCoroutine(MoveAndRotateCoroutine(positionOffset, Quaternion.Euler(rotationOffset)));
    }

    private IEnumerator MoveAndRotateCoroutine(Vector3 targetPosition, Quaternion targetRotation)
    {
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }
}