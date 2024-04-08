using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts_dev.Animation
{
    public class DotweenCodes : MonoBehaviour
    {
        public static DotweenCodes instance;
        private void Awake()
        {
            instance = this;
        }
        public void PopOut(Transform targetTransform,Vector3 maxSize,float duration)
        {
            targetTransform.localScale =  Vector3.one;
            targetTransform.DOScale(maxSize, duration*0.7f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    targetTransform.DOScale(Vector3.one, duration * 0.7f);
                });
            Vector3 originalPosition = targetTransform.position;

            targetTransform.DOJump(originalPosition + Vector3.up * 0.5f, 1, 1, duration * 1.5f)
                .SetEase(Ease.OutQuad);
            targetTransform.DOMove(originalPosition, duration).SetDelay(duration * 1.2f);
        }
        public void ScaleBig(GameObject go,float amount, float duration)
        {
            Vector3 originalScale = go.transform.localScale;
            go.transform.DOScale(Vector3.one * amount, duration).SetEase(Ease.OutQuad).OnComplete(()=>{ go.transform.DOScale(Vector3.one , duration).SetEase(Ease.OutQuad); });
        }
    }
}
