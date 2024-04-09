using _Project.Scripts_dev.Managers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace _Project.Scripts_dev.Classes
{
    public class Interact : MonoBehaviour
    {
        [Inject] private GameManager _gameManager;
        [FormerlySerializedAs("border")] [SerializeField] private RectTransform _border;
        [FormerlySerializedAs("noMoney")] [SerializeField] private bool _isNoMoney;
        private bool _isPoor;

        private void Update()
        {
            switch (_gameManager.Money)
            {
                case <= 0 when !_isNoMoney && !_isPoor:
                    DOTween.Kill(_border, true);
                    _border.DOScale(Vector2.one, 0.1f);
                    _isPoor = true;
                    break;
                case > 0:
                    _isPoor = false;
                    break;
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (_isNoMoney) return;
            if (!other.CompareTag("Player") || DOTween.IsTweening(_border)) return;
            if (_gameManager.Money > 0)
            {
                _border.DOScale(1.2f * Vector2.one, 0.15f).OnComplete(() =>
                {
                    _border.DOScale(Vector2.one *1f, 0.15f);
                }).SetLoops(-1, LoopType.Restart);
            }
            else
            {
                _border.DOScale(1.2f * Vector2.one, 0.15f);
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (!_isNoMoney) return;
            if ((other.CompareTag("Player")) && !DOTween.IsTweening(_border))
            {
                _border.DOScale(1.2f * Vector2.one, 0.15f);

            }
        }

        private void OnTriggerExit(Collider other)
        {
            if ((other.CompareTag("Player")))
            {
                DOTween.Kill(_border, true);
                _border.DOScale(Vector2.one, 0.15f);
            }
        }
    }
}
