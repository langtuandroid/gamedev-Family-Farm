using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts_dev.Animation
{
    public class SpinAnimation : MonoBehaviour
    {
        public float spinDuration = 1f;
        public Vector3 spinAxis = Vector3.up;
        public Ease spinEase = Ease.Linear;
        public bool isLooping;

        private void Start()
        {
            StartSpinAnimation();
        }
        private void OnEnable()
        {
            StartSpinAnimation();
        }

        private void StartSpinAnimation()
        {
            Tweener tweener = transform.DORotate(transform.rotation.eulerAngles + spinAxis * 360f, spinDuration, RotateMode.FastBeyond360)
                .SetEase(spinEase).SetUpdate(true);

            if (isLooping)
            {
                tweener.SetLoops(-1, LoopType.Restart);
            }
        }
    }
}
