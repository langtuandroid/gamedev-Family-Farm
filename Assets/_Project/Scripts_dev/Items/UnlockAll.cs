using UnityEngine;
using Zenject;

namespace _Project.Scripts_dev.Items
{
    public class UnlockAll : MonoBehaviour //TODO Remove
    {
        [Inject] private GameManager _gameManager;
        public bool unlock;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _gameManager.money += 10000;
                if (!unlock) return;
                foreach (GameObject go in _gameManager.unlockOrder)
                {
                    go.SetActive(true);
                }
                foreach (Unlock go in FindObjectsOfType<Unlock>())
                {
                    go.remain = 0;
                }
            }
        }
    
    }
}
