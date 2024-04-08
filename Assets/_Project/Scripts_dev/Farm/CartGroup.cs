using UnityEngine;

namespace _Project.Scripts_dev.Farm
{
    public class CartGroup : MonoBehaviour
    {
        [SerializeField] Transform partent;

        private void Update()
        {
            transform.localRotation = partent.rotation;
        }
    }
}
