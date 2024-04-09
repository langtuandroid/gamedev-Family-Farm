using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts_dev.Farm
{
    public class CartGroup : MonoBehaviour
    {
        [FormerlySerializedAs("partent")] [SerializeField] private Transform _cartTransform;

        private void Update()
        {
            transform.localRotation = _cartTransform.rotation;
        }
    }
}
