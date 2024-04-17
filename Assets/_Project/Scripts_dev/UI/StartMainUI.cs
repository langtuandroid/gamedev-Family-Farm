using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts_dev.UI
{
    public class StartMainUI : MonoBehaviour
    {
        private List<Vector2> _oldPos;
        [FormerlySerializedAs("rects")] [SerializeField] private RectTransform[] _rects;
        private float _speed = 0.5f;
        private void Start()
        {
            _oldPos = new List<Vector2>() ;
      
            SetUp();
            FirstSetUp(0.2f);
            SecondStep(0.4f);
            ThirdStep(0.6f);
            FourthStep(0.8f);
        }

        private void SetUp()
        {
            foreach (var rect in _rects)
            {
                _oldPos.Add(rect.position);
            }
            _rects[0].anchoredPosition = new Vector2(_rects[0].anchoredPosition.x - 350, _rects[0].anchoredPosition.y);
            _rects[1].anchoredPosition = new Vector2(_rects[1].anchoredPosition.x + 350, _rects[1].anchoredPosition.y);
            _rects[2].anchoredPosition = new Vector2(_rects[2].anchoredPosition.x + 350, _rects[2].anchoredPosition.y);
            _rects[3].anchoredPosition = new Vector2(_rects[3].anchoredPosition.x + 350, _rects[3].anchoredPosition.y);
            _rects[4].anchoredPosition = new Vector2(_rects[4].anchoredPosition.x + 350, _rects[4].anchoredPosition.y);
            _rects[5].anchoredPosition = new Vector2(_rects[5].anchoredPosition.x, _rects[5].anchoredPosition.y+350);
            _rects[6].anchoredPosition = new Vector2(_rects[6].anchoredPosition.x - 350, _rects[6].anchoredPosition.y);
        }

        private void FirstSetUp(float delay)
        {
            _rects[0].DOMove(_oldPos[0]+new Vector2(20,0), _speed).SetEase(Ease.OutQuad).SetDelay(delay).OnComplete(()=> {
                _rects[0].DOMove(_oldPos[0], _speed/2).SetEase(Ease.InQuad);
            }); 
        }

        private void SecondStep(float delay)
        {
            _rects[1].DOMove(_oldPos[1] - new Vector2(20, 0), _speed).SetEase(Ease.OutQuad).SetDelay(delay).OnComplete(() => {
                _rects[1].DOMove(_oldPos[1], _speed/2).SetEase(Ease.InQuad);
            }); 
        }

        private void ThirdStep(float delay)
        {
            _rects[2].DOMove(_oldPos[2] - new Vector2(20, 0), _speed).SetEase(Ease.OutQuad).SetDelay(delay).OnComplete(() => {
                _rects[2].DOMove(_oldPos[2], _speed/2).SetEase(Ease.InQuad);
            });
        }

        private void FourthStep(float delay)
        {
            _rects[3].DOMove(_oldPos[3] - new Vector2(20, 0), _speed).SetEase(Ease.OutQuad).SetDelay(delay).OnComplete(() => {
                _rects[3].DOMove(_oldPos[3], _speed/2).SetEase(Ease.InQuad);
            });
            _rects[4].DOMove(_oldPos[4] - new Vector2(20, 0), _speed).SetEase(Ease.OutQuad).SetDelay(delay).OnComplete(() => {
                _rects[4].DOMove(_oldPos[4], _speed/2).SetEase(Ease.InQuad);
            });
            _rects[5].DOMove(_oldPos[5] - new Vector2(0, 20), _speed).SetEase(Ease.OutQuad).SetDelay(delay).OnComplete(() => {
                _rects[5].DOMove(_oldPos[5], _speed/2).SetEase(Ease.InQuad);

            });
            _rects[6].DOMove(_oldPos[6] + new Vector2(20, 0), _speed).SetEase(Ease.OutQuad).SetDelay(delay).OnComplete(() => {
                _rects[6].DOMove(_oldPos[6], _speed / 2).SetEase(Ease.InQuad);
            });
        }

    }
}
