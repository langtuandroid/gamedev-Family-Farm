using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts_dev.UI
{
    public class PopUp : MonoBehaviour
    { 
        private RectTransform _thisRectTransform;
        [FormerlySerializedAs("delay")] [SerializeField] private float _delay;
        [FormerlySerializedAs("inplace")] [SerializeField] private bool _isInaplace;
        [FormerlySerializedAs("threshold")] [SerializeField] private float _thresHold = 1.2f;

        private void OnEnable()
        {
            _thisRectTransform = GetComponent<RectTransform>();
            StartCoroutine(DelayPopOut());
        }

        private IEnumerator DelayPopOut()
        {
            Vector2 oldPos=Vector2.zero;
            if (_isInaplace) transform.localScale = Vector3.zero; else
            {
                oldPos = _thisRectTransform.anchoredPosition;
                _thisRectTransform.anchoredPosition = new Vector2(_thisRectTransform.anchoredPosition.x, -Screen.height * 1.5f);
            }
            yield return new WaitForSeconds(_delay);
            if (!_isInaplace)
            {
          
                _thisRectTransform.DOAnchorPosY(oldPos.y + Screen.height / 5, 0.5f).SetEase(Ease.OutQuad).SetUpdate(true).OnComplete(() => {
                    _thisRectTransform.DOAnchorPosY(oldPos.y, 0.5f).SetEase(Ease.OutQuad).SetUpdate(true);
                });
            }
            else
            {
                transform.localScale = Vector3.one * 1 / _thresHold;
                transform.DOScale(Vector3.one * _thresHold, 0.5f).SetEase(Ease.OutQuad).SetUpdate(true).OnComplete(() => {
                    transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutQuad);
                });
            }

     
        }
   
    }
}
