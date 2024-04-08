using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform cameraTransform;

    private void Start()
    {
        // Lấy transform của camera chính trong scene
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        // Đặt đối tượng luôn nhìn về hướng của camera
        transform.LookAt(cameraTransform);
    }
}
