using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAtLevel : MonoBehaviour
{
    public int level;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.level >= level) gameObject.SetActive(false);
    }
}
