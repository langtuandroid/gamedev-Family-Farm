using System.Collections;
using _Project.Scripts_dev.Animation;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts_dev.Managers
{
    public class EffectManager : MonoBehaviour
    {
        [Inject] private SoundManager _soundManager;
        [Inject] private GameManager _gameManager;
        [SerializeField] public GameObject _dustTrailPrefab;
        [SerializeField] private GameObject _rainbowUnlock;
        [SerializeField] private GameObject _starImage;
        [SerializeField] private GameObject _starExp; 
        [SerializeField] private GameObject _moneyImage;
        [SerializeField] private GameObject _moneyBar;

        public GameObject RainbowUnlock => _rainbowUnlock;
    
        private void MoveItemToUI(Transform startPos, RectTransform targetUI, GameObject obj, TweenCallback complete)
        {
            Vector3 startScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, startPos.position);
            obj.transform.position = new Vector3(startScreenPos.x, startScreenPos.y,0);
            ParabolicMovement(obj, startScreenPos, targetUI.position, UnityEngine.Random.Range(1, 1.5f), UnityEngine.Random.Range(-200f, 200f), complete);
        }
        public void ExperienceEffect(float exp, Transform startPos)
        {
            GameObject go= Instantiate(_starImage,GameObject.FindGameObjectWithTag("MainUI").transform);
            go.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            MoveItemToUI(startPos, _starExp.GetComponent<RectTransform>(), go,()=> {
                if (_gameManager.IsDraggable)
                    _soundManager.CreateSound(_soundManager.Clips[9], transform.position, 0.5f);
                StartCoroutine(Experience(exp));
                go.transform.GetChild(1).GetComponent<Image>().enabled = false;
                DotweenCodes.instance.ScaleBig(_starExp, 1.2f, 0.1f);
            });
        }
        public void MoneyEffect(float money, Transform startPos)
        {
            GameObject go= Instantiate(this._moneyImage,GameObject.FindGameObjectWithTag("MainUI2").transform);
            go.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            MoveItemToUI(startPos, _moneyBar.GetComponent<RectTransform>(), go,()=> {
                if (_gameManager.IsDraggable)
                    _soundManager.CreateSound(_soundManager.Clips[5], transform.position, 0.5f);
           
                _gameManager.Money += money;
                go.GetComponent<Image>().enabled = false;
           
            });
        }
        private IEnumerator Experience(float exp)
        {
            float part = exp / 20;
            while(exp>0)
            {
                yield return null;
                _gameManager.Exp += part>exp?exp:part;
                exp -= part;
            }
        }
        private void ParabolicMovement(GameObject go,Vector3 start, Vector3 targetPosition, float duration, float height, TweenCallback OnComplete)
        {
            Vector3[] path = new Vector3[3];
            path[0] = start;
            Vector3 tempPos = (Vector2) start + UnityEngine.Random.insideUnitCircle * 250;
            path[1] = tempPos;
            path[2] = targetPosition;

            go.GetComponent<RectTransform>().DOPath(path, duration, PathType.CatmullRom, PathMode.Sidescroller2D, 10, Color.red)
                .SetEase(Ease.InQuad)
                .OnComplete(OnComplete);
        }
    }
}
