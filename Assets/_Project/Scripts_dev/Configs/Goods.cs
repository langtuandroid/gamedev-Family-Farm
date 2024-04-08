using UnityEngine;

namespace _Project.Scripts_dev.Configs
{
    [CreateAssetMenu (fileName ="New Product",menuName ="Product")]
    public class Goods : ScriptableObject
    {
        public int id;
        public float income;
        public string productName;
        public GameObject prefab;
        public float exp;
    }
}
