using UnityEngine;

namespace _Project.Scripts_dev.Classes
{
    public class Rotation : MonoBehaviour
    {
        private float _rotationsSpeed = 200f; 
        private Vector3 _startRotation;
        private Transform _transform;
        private void Start()
        {
            _transform = transform;
            _startRotation = _transform.eulerAngles;
        }
        private void Update()
        {
            _startRotation.x += _rotationsSpeed * Time.deltaTime;
            var eulerAngles = _transform.parent.eulerAngles;
            _startRotation.y = eulerAngles.y;
            _startRotation.z = eulerAngles.z;
            _transform.eulerAngles = _startRotation;
        }
        
    }
}
