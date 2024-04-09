using UnityEngine;

namespace _Project.Scripts_dev.Сamera
{
    public class CameraFollowPlayer : MonoBehaviour
    {
        public GameObject PlayerObject { get; set; }
        private void Start()
        {
            InvokeRepeating("UpdatePlayer",-1,1);
        }
    
        private void Update()
        {
            if(PlayerObject.activeInHierarchy)
                transform.position = PlayerObject.transform.position;
        }
        private void UpdatePlayer()
        {
            PlayerObject = GameObject.FindGameObjectWithTag("Player");
        }
        
    }
}
