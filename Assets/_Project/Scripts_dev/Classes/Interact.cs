using DG.Tweening;
using UnityEngine;
using Zenject;

namespace _Project.Scripts_dev.Classes
{
    public class Interact : MonoBehaviour
    {
        [Inject] private GameManager _gameManager;
        [SerializeField] RectTransform border;
        public bool noMoney;
        private bool imPoor;

        private void Update()
        {
            if (_gameManager.money <= 0&&!noMoney&&!imPoor)
            {
                DOTween.Kill(border, true);
                border.DOScale(Vector2.one, 0.1f);
                imPoor = true;
            }
            if (_gameManager.money > 0) imPoor = false;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!noMoney) {
                if (other.CompareTag("Player") && !DOTween.IsTweening(border))
                {
                    if (_gameManager.money > 0)
                    {
                
                        border.DOScale(1.2f * Vector2.one, 0.15f).OnComplete(() =>
                        {
                            border.DOScale(Vector2.one *1f, 0.15f);
                        }).SetLoops(-1, LoopType.Restart);
                    }
                    else
                    {
                        border.DOScale(1.2f * Vector2.one, 0.15f);
                    }
                }
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (!noMoney) return;
            if ((other.CompareTag("Player")) && !DOTween.IsTweening(border))
            {
                border.DOScale(1.2f * Vector2.one, 0.15f);

            }
        }

        private void OnTriggerExit(Collider other)
        {
            if ((other.CompareTag("Player")))
            {
                DOTween.Kill(border, true);
                border.DOScale(Vector2.one, 0.15f);
            }
        }
    }
}
