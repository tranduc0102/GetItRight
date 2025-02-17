using UnityEngine;

public class CameraFitter : MonoBehaviour
{
    public float referenceWidth = 1080f; 
    public float referenceFOV = 60f;    
    public float maxFOV = 111f;          

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        AdjustCameraFOV();
    }

    void AdjustCameraFOV()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float aspectRatio = screenWidth / screenHeight;

        float horizontalFOV = referenceFOV * (referenceWidth / screenWidth);
        float verticalFOV = 2f * Mathf.Atan(Mathf.Tan(horizontalFOV * Mathf.Deg2Rad / 2f) / aspectRatio) * Mathf.Rad2Deg;
        
        cam.fieldOfView = Mathf.Max(verticalFOV, maxFOV);
    }
}