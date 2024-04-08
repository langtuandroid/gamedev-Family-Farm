using UnityEngine;

namespace _Project.Scripts_dev.Classes
{
    public class KeepInplace : MonoBehaviour
    {
        public Vector3 pos;
        public float delayTime;
        private void Update()
        {
            delayTime += Time.deltaTime;
            if (delayTime > 1 && delayTime < 6)
            {
                transform.GetChild(0).localPosition = pos;
                Destroy(this);

            }
        }
    }
}
