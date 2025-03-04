using _Project.Scripts_dev.Managers;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace _Project.Scripts_dev.Additional
{
    public class DisableAtLevel : MonoBehaviour
    {
        [Inject] private GameManager _gameManager;
        [FormerlySerializedAs("level")] [SerializeField] private int _level;

        private void Update()
        {
            if (_gameManager.Level >= _level) gameObject.SetActive(false);
        }
    }
}
