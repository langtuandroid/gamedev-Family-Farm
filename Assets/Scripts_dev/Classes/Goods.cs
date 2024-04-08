using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (fileName ="New Product",menuName ="Product")]
public class Goods : ScriptableObject
{
    public int id;
    public float income;
    public string productName;
    public GameObject prefab;
    public float exp;
}
