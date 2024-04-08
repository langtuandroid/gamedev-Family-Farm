using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOpen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnEnable()
    {
        GameManager.instance.draggable = false;
    }
    private void OnDisable()
    {
        GameManager.instance.draggable = true;
    }
}
