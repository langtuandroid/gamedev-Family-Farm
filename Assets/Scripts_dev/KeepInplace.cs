using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepInplace : MonoBehaviour
{
    public Vector3 pos;
    public float delayTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        delayTime += Time.deltaTime;
        if (delayTime > 1 && delayTime < 6)
        {
            transform.GetChild(0).localPosition = pos;
            Destroy(this);

        }
    }
}
