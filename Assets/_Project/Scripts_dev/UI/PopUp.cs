using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts_dev.UI
{
    public class PopUp : MonoBehaviour
    {
        private RectTransform rect;
        public float delay;
        public bool inplace;
        public float threshold=1.2f;
        void OnEnable()
        {
            rect = GetComponent<RectTransform>();

            StartCoroutine(DelayPopOut());
        }
        IEnumerator DelayPopOut()
        {
            Vector2 oldPos=Vector2.zero;
            if (inplace) transform.localScale = Vector3.zero; else
            {
                oldPos = rect.anchoredPosition;
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -Screen.height * 1.5f);
            }
            yield return new WaitForSeconds(delay);
            if (!inplace)
            {
          
                rect.DOAnchorPosY(oldPos.y + Screen.height / 5, 0.5f).SetEase(Ease.OutQuad).SetUpdate(true).OnComplete(() => {
                    rect.DOAnchorPosY(oldPos.y, 0.5f).SetEase(Ease.OutQuad).SetUpdate(true);
                });
            }
            else
            {
                transform.localScale = Vector3.one * 1 / threshold;
                transform.DOScale(Vector3.one * threshold, 0.5f).SetEase(Ease.OutQuad).SetUpdate(true).OnComplete(() => {
                    transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutQuad);
                });
            }

     
        }
   
    }
}
