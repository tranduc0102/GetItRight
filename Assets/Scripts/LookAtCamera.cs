using System;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Camera cam;
    private void Start()
    {
        cam = Camera.main;
        if (cam)
        {
            LookAt();
        }
    }
    private void OnEnable()
    {
        if (cam)
        {
            LookAt();
        }
    }
    public void LookAt()
    {
        Vector3 lookAtPos = cam.transform.position;
        lookAtPos.x = transform.position.x;
        transform.LookAt(lookAtPos);
    }
}