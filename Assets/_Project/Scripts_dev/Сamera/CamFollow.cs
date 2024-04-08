using UnityEngine;

namespace _Project.Scripts_dev.Сamera
{
    public class CamFollow : MonoBehaviour
    {
        public GameObject player;
        private void Start()
        {
            InvokeRepeating("UpdatePlayer",-1,1);
        }
    
        private void Update()
        {
            if(player.activeInHierarchy)
                transform.position = player.transform.position;
        }
        private void UpdatePlayer()
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }
}
