using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts_dev.Animation
{
    public class UpDown : MonoBehaviour
    {
        private RectTransform uiObject;
        public float movementDistance = 100f;
        public float movementDuration = 1f;

        private void Start()
        {
            uiObject = GetComponent<RectTransform>();
            Sequence sequence = DOTween.Sequence();
            sequence.Append(uiObject.DOAnchorPosY(uiObject.anchoredPosition.y + movementDistance, movementDuration).SetEase(Ease.OutQuad));
            sequence.Append(uiObject.DOAnchorPosY(uiObject.anchoredPosition.y, movementDuration).SetEase(Ease.OutQuad));
            sequence.SetLoops(-1); 
        }
   
    }
}
