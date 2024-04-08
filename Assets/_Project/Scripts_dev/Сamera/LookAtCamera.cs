using UnityEngine;

namespace _Project.Scripts_dev
{
    public class LookAtCamera : MonoBehaviour
    {
        private Transform cameraTransform;

        private void Start()
        {
            cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            transform.LookAt(cameraTransform);
        }
    }
}
