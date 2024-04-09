using UnityEngine;

namespace _Project.Scripts_dev.Additional
{
    public class DestroyTimer : MonoBehaviour
    {
        public float time;
        private void Start()
        {
            Destroy(gameObject, time);
        }
    }
}
