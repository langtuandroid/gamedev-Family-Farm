using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatPlace : MonoBehaviour
{
    public int currentQuantity;
    public GameObject[] pos;
    public void UpdateMat()
    {
        for (int i = 0; i < pos.Length; i++)
        {
            if (pos[i].transform.childCount == 0)
            {
                for (int j = i + 1; j < pos.Length; j++)
                {
                    if (pos[j].transform.childCount > 0)
                    {
                        GameObject child = pos[j].transform.GetChild(0).gameObject;
                        child.transform.parent = pos[i].transform;
                        child.transform.position = pos[i].transform.position;
                        break;
                    }
                }
            }
        }
        for (int i = 0; i < pos.Length; i++)
        {
            if (pos[i].transform.childCount > 0)
            {
                pos[i].transform.GetChild(0).localPosition = Vector3.zero;
            }
        }
    }
}
