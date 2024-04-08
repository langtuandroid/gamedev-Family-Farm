using UnityEngine;
using Zenject;

namespace _Project.Scripts_dev.UI
{
    public class UIOpen : MonoBehaviour
    {
        [Inject] private GameManager _gameManager;
        private void OnEnable()
        {
            _gameManager.draggable = false;
        }
        private void OnDisable()
        {
            _gameManager.draggable = true;
        }
    }
}
