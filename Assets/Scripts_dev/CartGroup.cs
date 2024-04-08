using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartGroup : MonoBehaviour
{
    [SerializeField] Transform partent;

    private void Update()
    {
        transform.localRotation = partent.rotation;
    }
}
