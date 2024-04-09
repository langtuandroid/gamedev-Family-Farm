using System.Collections;
using _Project.Scripts_dev.Animation;
using _Project.Scripts_dev.Managers;
using _Project.Scripts_dev.UI;
using TMPro;
using UnityEngine;
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
        [SerializeField] TextMeshProUGUI priceText;
        [SerializeField] GameObject unlock;
        [SerializeField] float exp;
 
        private GameObject virtualCamera;

        public int minLevel;
        public float price;
        public float remain;
        bool unlocked,load;
        public float unlockTime;

        private Collider box;
        private void Start()
        {
            box = GetComponent<Collider>();
            remain = price;
            transform.rotation = Quaternion.identity;
            transform.Rotate(Vector3.up, 90f);
        }
        private void Update()
        {
            if (remain < 1) remain = 0;
            if(unlockTime<2)
                unlockTime += Time.deltaTime;
            if (!load)
            {
                if (_gameManager.load)
                {
                    if (_gameManager.playTime < 3)
                    {
                   
                        if (_dataManager.gameData != null)
                        {
                            remain = _dataManager.gameData.unlockMoney;
                            load = true;
                        }
                    }
                    else
                    {
                        load = true;
                        remain = price;
                    }

                    if (_dataManager.gameData == null) {

                        load = true;
                    } 
                }
            
                return;
            }
            if (unlocked) return;
            if (minLevel>0)
            {
                if (_gameManager.level < minLevel)
                {
                    transform.GetChild(0).gameObject.SetActive(false);
                    box.enabled = false;
                }
                else
                {
                    transform.GetChild(0).gameObject.SetActive(true);
                    box.enabled = true;
                    minLevel = 0;
                    StartCoroutine(_gameManager.DelayFocus(transform.gameObject));
                }
            }
            priceText.text =  _uiManager.FormatNumber(Mathf.Floor( remain));
            if (remain <= 0)
            {
                StartCoroutine(DelayUnlock());
            }
            else
            {
                unlock.SetActive(false);
           
            }
        }
        private IEnumerator DelayUnlock()
        {
            if (unlock.CompareTag("Area"))
            {
                _soundManager.PlaySound(_soundManager.sounds[2]);
            }
            else
                _soundManager.PlaySound(_soundManager.sounds[1]);
            yield return new WaitForSeconds(0.3f);
        
            unlock.SetActive(true);
            unlocked = true;
            DotweenCodes.instance.PopOut(unlock.transform, new Vector3(1, 2f, 1), 0.4f);
            Instantiate(_effectManager.rainbowUnlock, transform.position, _effectManager.rainbowUnlock.transform.rotation);
            if (_gameManager.currentUnlocked < _gameManager.unlockOrder.Length-1)
                _gameManager.currentUnlocked++;
            _gameManager.UpdateUnlocked();
            _gameManager.currentExp += exp;
            virtualCamera = GameObject.FindGameObjectWithTag("FocusCamera");
            if (virtualCamera != null)
                virtualCamera.SetActive(false);

            gameObject.SetActive(false);
        }

    
    }
}
