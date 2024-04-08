using UnityEngine;
using Zenject;

namespace _Project.Scripts_dev.Additional
{
    public class DisableAtLevel : MonoBehaviour
    {
        [Inject] private GameManager _gameManager;
        public int level;

        private void Update()
        {
            if (_gameManager.level >= level) gameObject.SetActive(false);
        }
    }
}
