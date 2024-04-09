using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts_dev.Configs
{
    [CreateAssetMenu (fileName ="New Product",menuName ="Product")]
    public class Goods : ScriptableObject
    {
        [FormerlySerializedAs("id")] public int Id;
        [FormerlySerializedAs("income")] public float Income;
        [FormerlySerializedAs("productName")] public string Name;
        [FormerlySerializedAs("prefab")] public GameObject ItemPrefab;
        [FormerlySerializedAs("exp")] public float ExpNum;
    }
}
