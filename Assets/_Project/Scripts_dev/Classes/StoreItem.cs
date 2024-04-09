using System.Collections;
using UnityEngine;

namespace _Project.Scripts_dev.Classes
{
    public class StoreItem : MonoBehaviour
    {
        private Vector3 _position;
        private float _delayTime;
        private void Update()
        {
            _delayTime += Time.deltaTime;
            if (_delayTime > 1 && _delayTime < 6)
            {
                transform.GetChild(0).localPosition = _position;
                Destroy(this);

            }
        }
    }
}
