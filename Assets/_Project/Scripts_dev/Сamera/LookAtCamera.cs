using UnityEngine;

namespace _Project.Scripts_dev.Сamera
{
    public class LookAtCamera : MonoBehaviour
    {
        private Transform _cameraTransform;

        private void Start()
        {
            _cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            transform.LookAt(_cameraTransform);
        }
    }
}
