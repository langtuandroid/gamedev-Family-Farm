using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockAll : MonoBehaviour
{
    public bool unlock;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.money += 10000;
            if (!unlock) return;
            foreach (GameObject go in GameManager.instance.unlockOrder)
            {
                go.SetActive(true);
            }
            foreach (Unlock go in FindObjectsOfType<Unlock>())
            {
                go.remain = 0;
            }
        }
    }
    
}
