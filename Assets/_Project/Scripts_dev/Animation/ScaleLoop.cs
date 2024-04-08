using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts_dev.Animation
{
    public class ScaleLoop : MonoBehaviour
    {
        public Transform target;
        public float scaleDuration = 1f;
        public float scaleFactor = 2f;
        private Sequence _sequence;
 

        private void Start()
        {
            _sequence = DOTween.Sequence();
            _sequence.Append(target.DOScale(scaleFactor, scaleDuration).SetEase(Ease.InOutQuad)); 
            _sequence.Append(target.DOScale(1f, scaleDuration).SetEase(Ease.InOutQuad)); 
            _sequence.SetLoops(-1); 
        }
        public void OnClick()
        {
            DOTween.KillAll();
        }
    }
}
