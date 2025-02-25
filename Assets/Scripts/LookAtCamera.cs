using System;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Camera cam;
    private void Start()
    {
        cam = Camera.main;
    }
    void Update()
    {
        Vector3 lookAtPos = cam.transform.position;
        lookAtPos.x = transform.position.x;
        transform.LookAt(lookAtPos);
    }
}