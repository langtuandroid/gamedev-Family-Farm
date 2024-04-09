using _Project.Scripts_dev.Managers;
using UnityEngine;
using Zenject;

namespace _Project.Scripts_dev.UI
{
    public class ActivateUI : MonoBehaviour
    {
        [Inject] private GameManager _gameManager;
        private void OnEnable()
        {
            _gameManager.IsDraggable = false;
        }
        private void OnDisable()
        {
            _gameManager.IsDraggable = true;
        }
    }
}
