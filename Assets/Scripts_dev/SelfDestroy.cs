using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    public float time;
    private void Start()
    {
        Destroy(gameObject, time);
    }
    private void Update()
    {
        
    }
}
