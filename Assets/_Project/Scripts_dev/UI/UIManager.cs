using System;
using System.Collections;
using _Project.Scripts_dev.Classes;
using _Project.Scripts_dev.Control;
using _Project.Scripts_dev.Items;
using _Project.Scripts_dev.Language;
using _Project.Scripts_dev.Managers;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts_dev.UI
{
    public class UIManager : MonoBehaviour
    {
        [Inject] private LanguageManager _languageManager;
        [Inject] private DataManager _dataManager;
        [Inject] private GameManager _gameManager;

        public bool sound, music;
        public TextMeshProUGUI moneyText;
        [SerializeField] Image EXPBarImage;
        [SerializeField] TextMeshProUGUI levelText;
        public TextMeshProUGUI expText;
        [SerializeField] TextMeshProUGUI cartText;
        [SerializeField] TextMeshProUGUI bonusMoneyText;
        [SerializeField] Cart playerCart;
        [SerializeField] Cart truckCart;
        [SerializeField] GameObject[] tabs;
        [SerializeField] GameObject[] tabBtnImages;
        [SerializeField] TextMeshProUGUI oldCart, newCart, reward,expBonus;
        [SerializeField] Transform musicUI, soundUI;
        [SerializeField] GameObject UpgradeIcon;
        [SerializeField] GameObject SpinNotice;

        [SerializeField] GameObject FirstSpinWheel;
        [SerializeField] GameObject unlockRewardUI;
        [SerializeField] GameObject adsUI,adsBtn;
        [SerializeField] TextMeshProUGUI price1,price2;
        [SerializeField] TextMeshProUGUI incomeRemain, speedRemain;
        [SerializeField] GameObject vnmeseImage;
        public GameObject BottomUI;
        public GameObject UILevelUp;
        public GameObject CarUI;

        public GameObject timerPrefab;

        public bool shouldShowUnlockReward;

        private float oldReward;
        private void Awake()
        {
            music = PlayerPrefs.GetFloat("music", 1) == 1;
            sound = PlayerPrefs.GetFloat("sound", 1) == 1;
            vnmeseImage.SetActive(PlayerPrefs.GetFloat("language", 1) == 0);
        }

        private void Update()
        {

            if (!_gameManager.firstRolled&&_dataManager.gameData==null)
            {
                StartCoroutine(Delay(() => FirstSpinWheel.SetActive(true),1.5f));
                BottomUI.SetActive(false);
                _gameManager.firstRolled = true;
            }
       
            if (FindObjectOfType<FarmSlot>() != null)
            {
                UpgradeIcon.SetActive(true);
            }
            moneyText.text =FormatNumber(Mathf.Ceil(_gameManager.money));
            EXPBarImage.fillAmount = _gameManager.currentExp / _gameManager.maxExp;
            levelText.text = _gameManager.level.ToString();
            expText.text = Mathf.Ceil(_gameManager.currentExp) + "/" + Mathf.Ceil(_gameManager.maxExp);
            if(_gameManager.truckTime<=0)
                cartText.text = playerCart.inCart + "/" + _gameManager.maxCart;
            else
            {
                cartText.text = truckCart.inCart + "/" + _gameManager.maxCart;
            }

            UpdateSoundIcons();
            if (_gameManager.freeSpinTimer <= 0)
            {
                SpinNotice.SetActive(true);
            }
            else
            {
                SpinNotice.SetActive(false);
            }
            UpdateBoostButton();
            ShowUnlockReward();
        }
        void ShowUnlockReward()
        {
            if (shouldShowUnlockReward&&_gameManager.money==0&&FindObjectOfType<Unlock>().remain>=1&&_gameManager.currentUnlocked!=0)
            {
                StartCoroutine(DelayUnlock());
                shouldShowUnlockReward = false;
            }
        }
        IEnumerator DelayUnlock()
        {
            yield return null;
            if (_gameManager.money == 0 &&(FindObjectOfType<Unlock>().unlockTime > 1 || FindObjectOfType<Unlock>().remain != FindObjectOfType<Unlock>().price))
            {
                unlockRewardUI.SetActive(true);
           
            }
      
        }
        void UpdateBoostButton()
        {
            Button incomeButton = incomeRemain.transform.parent.GetComponent<Button>();
            Button speedButton = speedRemain.transform.parent.GetComponent<Button>();

            float incomeTime = _gameManager.incomeBoostTime;
            float speedTime = _gameManager.speedBoostTime;
            incomeRemain.text =incomeTime>0? FormatTime(incomeTime):"3m";
            speedRemain.text = speedTime>0?FormatTime(speedTime):"3m";
            if (_gameManager.incomeBoostTime>0)
            {
                incomeButton.enabled = false;
                incomeButton.transform.GetChild(2).gameObject.SetActive(false);
                incomeButton.transform.GetChild(1).gameObject.SetActive(false);
                incomeButton.transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                incomeButton.enabled = true;
                incomeButton.transform.GetChild(2).gameObject.SetActive(true);
                incomeButton.transform.GetChild(1).gameObject.SetActive(true);
                incomeButton.transform.GetChild(0).gameObject.SetActive(true);

            }
            if (_gameManager.speedBoostTime>0)
            {
                speedButton.enabled = false;
                speedButton.transform.GetChild(2).gameObject.SetActive(false);
                speedButton.transform.GetChild(1).gameObject.SetActive(false);
                speedButton.transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                speedButton.enabled = true;
                speedButton.transform.GetChild(2).gameObject.SetActive(true);
                speedButton.transform.GetChild(1).gameObject.SetActive(true);
                speedButton.transform.GetChild(0).gameObject.SetActive(true);

            }
        }
        public void BoostReward(int type)
        {
            if (type == 1)
            {
                StartCoroutine(Delay(() =>
                {
                    _gameManager.speedBoostTime = 150;
                    _gameManager.interTimer = 0;
                }, 0.2f));
            }
            if (type == 0)
            {
                StartCoroutine(Delay(() =>
                {
                    _gameManager.incomeBoostTime = 150;
                    _gameManager.interTimer = 0;
                }, 0.2f));
            }

        }
        void UpdateSoundIcons()
        {
            if (music)
            {
                musicUI.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                musicUI.GetChild(1).gameObject.SetActive(true);
            }
            if (sound)
            {
                soundUI.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                soundUI.GetChild(1).gameObject.SetActive(true);
            }
        }
        public void  UpdateLevelUpUI(int oldCart, int newCart,float reward,float bonusExp) {
            this.oldCart.text = oldCart.ToString();
            this.newCart.text = newCart.ToString();
            this.reward.text ="+"+ FormatNumber(Mathf.Ceil(reward));
            this.expBonus.text ="+"+ FormatNumber(Mathf.Ceil(bonusExp));
            bonusMoneyText.text= "+" + FormatNumber(Mathf.Ceil(reward*2));
            oldReward = reward;
        }
        public void GetLevelUpRewardAds(RectTransform canvas)
        {
            StartCoroutine(Delay(() =>
            {
                StartCoroutine(_gameManager.GetLevelUpRewardAdsCoroutine(oldReward, 1.3f * _gameManager.maxExp / 10, true));
                _gameManager.interTimer = 0;
                TurnOffPopUp(canvas);
            }, 0.2f));
        } 
        public void GetLevelUpRewardFree(RectTransform canvas)
        {
            StartCoroutine(_gameManager.GetLevelUpRewardAdsCoroutine(oldReward, 1.3f * _gameManager.maxExp / 10, false));
            TurnOffPopUp(canvas);
        }
   
        public string FormatNumber(float number)
        {
            string suffix = "";
            float formattedNumber = number;

            if (number >= 1000f && number < 1000000f)
            {
                suffix = "k";
                formattedNumber = number / 1000f;
            }
            else if (number >= 1000000f && number < 1000000000f)
            {
                suffix = "m";
                formattedNumber = number / 1000000f;
            }
            else if (number >= 1000000000f)
            {
                suffix = "b";
                formattedNumber = number / 1000000000f;
            }

            return formattedNumber.ToString("0.##") + suffix;
        }
        public void OpenPopUp(RectTransform rect)
        {
            Vector2 oldPos = rect.anchoredPosition;
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -Screen.height);
            rect.DOAnchorPosY(oldPos.y+Screen.height/5, 0.5f).SetDelay(0.5f).SetEase(Ease.OutQuad).OnComplete(()=> {
                rect.DOAnchorPosY(oldPos.y, 0.5f).SetEase(Ease.OutQuad);
            });
        }
        public void TurnOffPopUp(RectTransform rect)
        {
            if (DOTween.IsTweening(rect.transform)) return;
            StartCoroutine(TurnOffPopUpCr(rect));
        }
        IEnumerator TurnOffPopUpCr(RectTransform rect)
        {
            yield return new WaitForSeconds(0.2f);
     
            GameObject canvasParent = rect.transform.parent.gameObject;
            Vector2 oldPos = rect.anchoredPosition;
            Color color = rect.GetComponent<Image>().color;
            float originalAlpha = color.a;
            color.a = 0;
            rect.GetComponent<Image>().color = color;
            color.a = originalAlpha;
            //rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -Screen.height);
            rect.DOAnchorPosY(oldPos.y + Screen.height / 5, 0.5f).SetEase(Ease.OutQuad).SetUpdate(true).OnComplete(() => {
                rect.DOAnchorPosY(oldPos.y - Screen.height * 1.5f, 1).SetEase(Ease.OutQuad).SetUpdate(true).OnComplete(() =>
                {
                    canvasParent.SetActive(false); rect.GetComponent<Image>().color = color;
                    rect.anchoredPosition = oldPos;
                    Delay(() => Time.timeScale = 1, 0.1f);
                    _gameManager.gainExp = true;

                });
            });
        }
        public void OpenTab(int tabIndex)
        {
            foreach (GameObject t in tabs)
            {
                t.SetActive(false);
            }
            foreach (GameObject t in tabBtnImages)
            {
                t.SetActive(false);
                t.transform.parent.GetComponent<Button>().enabled = true;
            }
            tabs[tabIndex].SetActive(true);
            tabs[tabIndex].GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;

            tabBtnImages[tabIndex].SetActive(true);
            tabBtnImages[tabIndex].transform.parent.GetComponent<Button>().enabled = false;
        }
        public void PauseGame()
        {
            Time.timeScale = 0;
       
        }
        public void ResumeGame()
        {
            Time.timeScale = 1;

        }
        public void ButtonMusicClick(Transform transform)
        {
            music = !music;
            PlayerPrefs.SetFloat("music", music ? 1 : 0);
      
        } public void ButtonSoundClick(Transform transform)
        {
            sound = !sound;
            PlayerPrefs.SetFloat("sound", sound ? 1 : 0);
        }
        public void QuitGame()
        {
            Time.timeScale = 1;
       
            Application.Quit();
        }
        IEnumerator Delay(Action Action , float time)
        {
            yield return new WaitForSeconds(time);
            Action();
        }
        public void OnClickInter()
        {
            if (_gameManager.interTimer >= 45&&_gameManager.totalPlayTime>60)
            {
            
            }
        }
        public void RewardUnlock()
        {
            StartCoroutine(Delay(() =>
            {
                FindObjectOfType<Unlock>().remain=0;
                unlockRewardUI.SetActive(false);
                _gameManager.interTimer = 0;
            }, 0.2f));
        } public void RewardCar()
        {
            StartCoroutine(Delay(() =>
            {
                GameObject.Find("Player").GetComponent<PlayerControl>().GetCar(GameObject.Find("PlayerWithCar").GetComponent<Collider>());
                _gameManager.interTimer = 0; 
            }, 0.2f));
        }

        public string FormatTime(float seconds)
        {
            int minutes = (int)(seconds / 60);
            int remainingSeconds = (int)(seconds % 60);
            return string.Format("{0:00}:{1:00}", minutes, remainingSeconds);
        }
        public void ReloadGame()
        {
            SceneManager.LoadScene(0);
        }
    
        public void ChangeLanguage()
        {
            vnmeseImage.SetActive(!vnmeseImage.activeInHierarchy);
            if (vnmeseImage.activeInHierarchy)
            {
                PlayerPrefs.SetFloat("language", 0);
                _languageManager.language = 0;
            }
            else { PlayerPrefs.SetFloat("language", 1);
                _languageManager.language = 1;
            }
            _languageManager.SetTextAll();
        }
    }
}
