using System.Collections;
using _Project.Scripts_dev.Animation;
using _Project.Scripts_dev.Managers;
using _Project.Scripts_dev.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace _Project.Scripts_dev.Items
{
    public class Unlock : MonoBehaviour
    {
        [Inject] private SoundManager _soundManager;
        [Inject] private EffectManager _effectManager;
        [Inject] private DataManager _dataManager;
        [Inject] private GameManager _gameManager;
        [Inject] private UIManager _uiManager;
        [FormerlySerializedAs("priceText")] [SerializeField] private TextMeshProUGUI _priceText;
        [FormerlySerializedAs("unlock")] [SerializeField] private GameObject _unlockObject;
        [FormerlySerializedAs("exp")] [SerializeField] private float _expirience;
        [FormerlySerializedAs("minLevel")] [SerializeField] private int _minUnlockLevel;
        public float price;
        public float remain;
        public float unlockTime;
        
        private GameObject _virtualCamera;
        private bool _isUnlocked;
        private bool _isLoaded;
        private Collider _box;
        private void Start()
        {
            _box = GetComponent<Collider>();
            remain = price;
            transform.rotation = Quaternion.identity;
            transform.Rotate(Vector3.up, 90f);
        }
        private void Update()
        {
            if (remain < 1) remain = 0;
            if(unlockTime<2)
                unlockTime += Time.deltaTime;
            if (!_isLoaded)
            {
                if (_gameManager.IsLoad)
                {
                    if (_gameManager.PlayTime < 3)
                    {
                   
                        if (_dataManager.GameData != null)
                        {
                            remain = _dataManager.GameData.unlockMoney;
                            _isLoaded = true;
                        }
                    }
                    else
                    {
                        _isLoaded = true;
                        remain = price;
                    }

                    if (_dataManager.GameData == null) {

                        _isLoaded = true;
                    } 
                }
            
                return;
            }
            if (_isUnlocked) return;
            if (_minUnlockLevel>0)
            {
                if (_gameManager.Level < _minUnlockLevel)
                {
                    transform.GetChild(0).gameObject.SetActive(false);
                    _box.enabled = false;
                }
                else
                {
                    transform.GetChild(0).gameObject.SetActive(true);
                    _box.enabled = true;
                    _minUnlockLevel = 0;
                    StartCoroutine(_gameManager.FocusRoutine(transform.gameObject));
                }
            }
            _priceText.text =  _uiManager.FormatNumber(Mathf.Floor( remain));
            if (remain <= 0)
            {
                StartCoroutine(UnlockRoutine());
            }
            else
            {
                _unlockObject.SetActive(false);
           
            }
        }
        private IEnumerator UnlockRoutine()
        {
            if (_unlockObject.CompareTag("Area"))
            {
                _soundManager.PlaySound(_soundManager.Clips[2]);
            }
            else
                _soundManager.PlaySound(_soundManager.Clips[1]);
            yield return new WaitForSeconds(0.3f);
        
            _unlockObject.SetActive(true);
            _isUnlocked = true;
            DotweenCodes.instance.PopOut(_unlockObject.transform, new Vector3(1, 2f, 1), 0.4f);
            Instantiate(_effectManager.RainbowUnlock, transform.position, _effectManager.RainbowUnlock.transform.rotation);
            if (_gameManager.CurrentUnlocked < _gameManager._unlockOrder.Length-1)
                _gameManager.CurrentUnlocked++;
            _gameManager.UpdateLocked();
            _gameManager.Exp += _expirience;
            _virtualCamera = GameObject.FindGameObjectWithTag("FocusCamera");
            if (_virtualCamera != null)
                _virtualCamera.SetActive(false);

            gameObject.SetActive(false);
        }

    
    }
}
