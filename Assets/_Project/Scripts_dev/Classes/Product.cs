using _Project.Scripts_dev.Configs;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts_dev.Classes
{
    public class Product : MonoBehaviour
    {
        [FormerlySerializedAs("goods")] [SerializeField] private Goods _goods;
        public Goods Goods => _goods;
    }

}
