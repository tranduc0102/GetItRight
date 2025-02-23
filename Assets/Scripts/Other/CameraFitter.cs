using UnityEngine;

public class CameraFit : MonoBehaviour
{
    public float referenceWidth = 1080f; 
    public float referenceHeight = 1920f; 
    public float referenceOrthoSize = 5f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        AdjustCamera();
    }

    void AdjustCamera()
    {
        float referenceAspectRatio = referenceWidth / referenceHeight;
        float currentAspectRatio = (float)Screen.width / Screen.height;

        if (cam.orthographic)
        { 
            cam.orthographicSize = referenceOrthoSize * (referenceAspectRatio / currentAspectRatio);
        }
        else
        {
            float targetFOV = CalculateFOV(currentAspectRatio);
            cam.fieldOfView = targetFOV;
        }
    }

    float CalculateFOV(float currentAspectRatio)
    {
        float referenceAspectRatio = referenceWidth / referenceHeight;

        if (currentAspectRatio > referenceAspectRatio)
        {
            return cam.fieldOfView;
        }
        else
        {
            return cam.fieldOfView * (referenceAspectRatio / currentAspectRatio);
        }
    }
}