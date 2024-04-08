using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaInfo : MonoBehaviour
{
    public Vector3 position;
    public Shelf shelf;
    private void Awake()
    {
        position = transform.position;
    }
}
