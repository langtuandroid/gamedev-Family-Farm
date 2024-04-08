using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinTransform : MonoBehaviour
{
    public float rotationSpeed = 10f; // Tốc độ xoay của đối tượng

    private Vector3 v3;

    void Update()
    {
        v3.x += rotationSpeed * Time.deltaTime;
        v3.y = transform.parent.eulerAngles.y;
        v3.z = transform.parent.eulerAngles.z;
        transform.eulerAngles = v3;
    }
    private void Start()
    {
        v3 = transform.eulerAngles;
        
    }
}
