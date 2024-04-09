using System;
using System.Collections;
using _Project.Scripts_dev.Animation;
using _Project.Scripts_dev.Items;
using _Project.Scripts_dev.Language;
using _Project.Scripts_dev.Managers;
using _Project.Scripts_dev.UI;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts_dev.Additional
{
    public class WheelManager : MonoBehaviour
    {
        [Inject] private SoundManager _soundManager;
        [Inject] private LanguageManager _languageManager;
        [Inject] private UIManager _uiManager;
        [Inject] private GameManager _gameManager;
        [FormerlySerializedAs("prizeText1")] [SerializeField] TextMeshProUGUI _prize1;
        [FormerlySerializedAs("prizeText2")] [SerializeField] TextMeshProUGUI _prize2;
        [FormerlySerializedAs("prizeText3")] [SerializeField] TextMeshProUGUI _prize3;
        [FormerlySerializedAs("prizeText4")] [SerializeField] TextMeshProUGUI _prize4;
        [FormerlySerializedAs("prizeText5")] [SerializeField] TextMeshProUGUI _prize5;
        [FormerlySerializedAs("prizeText6")] [SerializeField] TextMeshProUGUI _prize6;
        [FormerlySerializedAs("prizeText7")] [SerializeField] TextMeshProUGUI _prize7;
        [FormerlySerializedAs("prizeText8")] [SerializeField] TextMeshProUGUI _prize8;
        [FormerlySerializedAs("money")] [SerializeField] GameObject _moneyPrefab;
        [FormerlySerializedAs("exp")] [SerializeField] GameObject _expiriencePrefab;
        [FormerlySerializedAs("moneyPos")] [SerializeField] GameObject _moneyMovePos;
        [FormerlySerializedAs("expPos")] [SerializeField] GameObject _expirienceMovePos;
        private float _spinPrice;
        private float _maxExperience;
       

        public bool first;
        private void Start()
        {
            if (first) SetFiresPrizes();
        }
        
        private void OnDisable()
        {
            if (first)
            {
                _gameManager.UpdateLocked();
                _uiManager.BottomUI.SetActive(true);
            }
        }
        private void OnEnable()
        {
            SetPrizes();
       
        }
        private void SetPrizes()
        {
            _spinPrice = _gameManager._unlockOrder[_gameManager.CurrentUnlocked].transform.GetChild(1).GetComponent<Unlock>().price;
            _maxExperience = _gameManager.MaxExp;
            _prize1.text = "+" + _uiManager.FormatNumber(Mathf.Ceil(_maxExperience / 10));
            _prize2.text = "+" + _uiManager.FormatNumber(Mathf.Ceil(_spinPrice / 5));
            _prize3.text = "+" + _uiManager.FormatNumber(Mathf.Ceil(_maxExperience *0.3f));
            _prize4.text = "+" + _uiManager.FormatNumber(Mathf.Ceil(_spinPrice / 2));
            _prize5.text = "+" + _uiManager.FormatNumber(Mathf.Ceil(_maxExperience / 2));
            _prize6.text = _languageManager.GetText(55)+" + 150s";
            _prize7.text = _languageManager.GetText(56) + " +150s";
            _prize8.text = _languageManager.GetText(34);

        }
        private void SetFiresPrizes()
        {
            _prize1.text = "+" + _uiManager.FormatNumber(Mathf.Ceil(_maxExperience / 10));
            _prize2.text = "+" + _uiManager.FormatNumber(Mathf.Ceil(_spinPrice / 2)); ;
            _prize3.text = "+" + _uiManager.FormatNumber(Mathf.Ceil(_maxExperience * 0.3f));
            _prize4.text = "+150";
            _prize5.text = "+" + _uiManager.FormatNumber(Mathf.Ceil(_maxExperience / 2));
            _prize6.text = _languageManager.GetText(55) + " +150s";
            _prize7.text = _languageManager.GetText(56) + " +150s";
            _prize8.text = _languageManager.GetText(34);
        }
    
        public Action TakePrize(float prize)
        {
            Action prizeAction;
            switch (prize)
            {
                case 1: prizeAction = () => StartCoroutine(RealizePrize(_expiriencePrefab,_expirienceMovePos.GetComponent<RectTransform>(),5,_gameManager.MaxExp/10,1)); break;
                case 2: prizeAction = () => StartCoroutine(RealizePrize(_moneyPrefab,_moneyMovePos.GetComponent<RectTransform>(),10,_spinPrice/5,0)); break;
                case 3: prizeAction = () => StartCoroutine(RealizePrize(_expiriencePrefab, _expirienceMovePos.GetComponent<RectTransform>(),15,_gameManager.MaxExp*0.3f,1)); break;
                case 4: prizeAction = () => StartCoroutine(RealizePrize(_moneyPrefab, _moneyMovePos.GetComponent<RectTransform>(), 20, _spinPrice / 2, 0)); break;
                case 5: prizeAction = () => StartCoroutine(RealizePrize(_expiriencePrefab, _expirienceMovePos.GetComponent<RectTransform>(), 25, _gameManager.MaxExp / 2, 1)); break;
                case 6: prizeAction = () => _gameManager.IncomeBoostTime = 150; break;
                case 7: prizeAction = () => _gameManager.SpeedBoostTime = 150; break;
                case 8: prizeAction = () => _gameManager.FreeSpinTime = 0; break;
                default: prizeAction = () => _gameManager.FreeSpinTime = 0; break;
            }
            return prizeAction;
        }
        
        public Action TakeFirstPrize(float prize)
        {
            Action prizeAction;
            switch (prize)
            {
                case 1: prizeAction = () => StartCoroutine(RealizePrize(_expiriencePrefab,_expirienceMovePos.GetComponent<RectTransform>(),5,_gameManager.MaxExp/10,1)); break;
                case 2: prizeAction = () => StartCoroutine(RealizePrize(_moneyPrefab,_moneyMovePos.GetComponent<RectTransform>(),10,120,0)); break;
                case 3: prizeAction = () => StartCoroutine(RealizePrize(_expiriencePrefab, _expirienceMovePos.GetComponent<RectTransform>(),15,_gameManager.MaxExp*0.3f,1)); break;
                case 4: prizeAction = () => StartCoroutine(RealizePrize(_moneyPrefab, _moneyMovePos.GetComponent<RectTransform>(), 20, 150, 0)); break;
                case 5: prizeAction = () => StartCoroutine(RealizePrize(_expiriencePrefab, _expirienceMovePos.GetComponent<RectTransform>(), 25, _gameManager.MaxExp / 2, 1)); break;
                case 6: prizeAction = () => _gameManager.IncomeBoostTime = 150; break;
                case 7: prizeAction = () => _gameManager.SpeedBoostTime = 150; break;
                case 8: prizeAction = () => _gameManager.FreeSpinTime = 0; break;
                default: prizeAction = () => _gameManager.FreeSpinTime = 0; break;
            }
            return prizeAction;
        }
    

        private IEnumerator RealizePrize(GameObject prefab,RectTransform destination, int times, float amount,int type)
        {
            for (int i=0; i < times; i++)
            {
                yield return null;
                GameObject go = Instantiate(prefab, transform.parent);
           
                go.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().position+new Vector3(0,350,0);
                PizeMovement(go, go.transform.position, destination.position, UnityEngine.Random.Range(1, 1.5f), () => {
                    Destroy(go);
                    if(type==1)
                        DotweenCodes.instance.ScaleBig(_expirienceMovePos, 1.2f, 0.1f);
                    if (type == 1)
                        _gameManager.Exp += amount / times;
                    if (type == 0)
                        _gameManager.Money += amount / times;
                    int r = UnityEngine.Random.Range(0, 2);
                    if (r == 0 && type == 0)
                        _soundManager.CreateSound(_soundManager.Clips[5], transform.position, 0.5f);
                    else if (r == 0)
                    {
                        _soundManager.CreateSound(_soundManager.Clips[9], transform.position, 0.5f);
                    }
                });
            }
        }

        private void PizeMovement(GameObject go, Vector3 start, Vector3 targetPosition, float duration, TweenCallback OnComplete)
        {
            Vector3[] path = new Vector3[3];
            path[0] = start;
            Vector3 tempPos = (Vector2) start + UnityEngine.Random.insideUnitCircle * 300;
            path[1] = tempPos;
            path[2] = targetPosition;

            go.GetComponent<RectTransform>().DOPath(path, duration, PathType.CatmullRom, PathMode.Sidescroller2D, 10, Color.red)
                .SetEase(Ease.InQuad)
                .OnComplete(OnComplete);
        }
    }
}
