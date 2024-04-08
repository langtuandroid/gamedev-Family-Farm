using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public static CamFollow instance;
    private void Awake()
    {
        instance = this;
    }
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("UpdatePlayer",-1,1);
    }

    // Update is called once per frame
    void Update()
    {
        if(player.activeInHierarchy)
            transform.position = player.transform.position;
    }
    void UpdatePlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
}
