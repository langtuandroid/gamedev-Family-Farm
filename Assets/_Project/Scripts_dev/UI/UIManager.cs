using System;
using System.Collections;
using _Project.Scripts_dev.Classes;
using _Project.Scripts_dev.Control;
using _Project.Scripts_dev.Farm;
using _Project.Scripts_dev.Items;
using _Project.Scripts_dev.Managers;
using DG.Tweening;
using Integration;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts_dev.UI
{
    public class UIManager : MonoBehaviour
    {
        [Inject] private DataManager _dataManager;
        [Inject] private GameManager _gameManager;
        [Inject] private RewardedAdController _rewardedAdController;
        [Inject] private IAPService _iapService;
        public bool sound;
        public bool music;
        public TextMeshProUGUI moneyText;
        [SerializeField] private TextMeshProUGUI levelText;
        public TextMeshProUGUI expText;
        [SerializeField] private TextMeshProUGUI cartText;
        [SerializeField] private TextMeshProUGUI bonusMoneyText;
        [SerializeField] private Cart playerCart;
        [SerializeField] private Cart truckCart;
        [SerializeField] private GameObject[] tabs;
        
        [SerializeField] private TextMeshProUGUI oldCart;
        [SerializeField] private TextMeshProUGUI newCart;
        [SerializeField] private TextMeshProUGUI reward;
        [SerializeField] private TextMeshProUGUI expBonus;
        [SerializeField] private GameObject UpgradeIcon;
        [SerializeField] private GameObject SpinNotice;
        [SerializeField] private GameObject FirstSpinWheel;
        [SerializeField] private GameObject unlockRewardUI;
        [SerializeField] private GameObject adsUI;
        [SerializeField] private GameObject adsBtn;
        [SerializeField] private TextMeshProUGUI incomeRemain, speedRemain;
       
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
           
        }

        private void Update()
        {

            if (!_gameManager.FirstRolled&&_dataManager.GameData==null)
            {
                StartCoroutine(Delay(() => FirstSpinWheel.SetActive(true),1.5f));
                BottomUI.SetActive(false);
                _gameManager.FirstRolled = true;
            }
       
            if (FindObjectOfType<FarmSlot>() != null)
            {
                UpgradeIcon.SetActive(true);
            }
            moneyText.text =FormatNumber(Mathf.Ceil(_gameManager.Money));
            levelText.text = _gameManager.Level.ToString();
            expText.text = Mathf.Ceil(_gameManager.Exp) + "/" + Mathf.Ceil(_gameManager.MaxExp);
            if(_gameManager.TruckTime<=0)
                cartText.text = playerCart.inCart + "/" + _gameManager.MaxCart;
            else
            {
                cartText.text = truckCart.inCart + "/" + _gameManager.MaxCart;
            }
            
            if (_gameManager.FreeSpinTime <= 0)
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
            if (shouldShowUnlockReward&&_gameManager.Money==0&&FindObjectOfType<Unlock>().remain>=1&&_gameManager.CurrentUnlocked!=0)
            {
                StartCoroutine(DelayUnlock());
                shouldShowUnlockReward = false;
            }
        }
        IEnumerator DelayUnlock()
        {
            yield return null;
            if (_gameManager.Money == 0 &&(FindObjectOfType<Unlock>().unlockTime > 1 || FindObjectOfType<Unlock>().remain != FindObjectOfType<Unlock>().price))
            {
                unlockRewardUI.SetActive(true);
           
            }
      
        }
        void UpdateBoostButton()
        {
            Button incomeButton = incomeRemain.transform.parent.GetComponent<Button>();
            Button speedButton = speedRemain.transform.parent.GetComponent<Button>();

            float incomeTime = _gameManager.IncomeBoostTime;
            float speedTime = _gameManager.SpeedBoostTime;
            incomeRemain.text =incomeTime>0? FormatTime(incomeTime):"3m";
            speedRemain.text = speedTime>0?FormatTime(speedTime):"3m";
            if (_gameManager.IncomeBoostTime>0)
            {
                incomeButton.interactable = false;
            }
            else
            {
                incomeButton.interactable = true;
            }
            if (_gameManager.SpeedBoostTime>0)
            {
                speedButton.interactable = false;
            }
            else
            {
                speedButton.interactable = true;
            }
        }
        
        public void RequestX2Booster()
        {
            ShowRewardedAdd();
            _rewardedAdController.OnVideoClosed += CancelX2Booster;
            _rewardedAdController.GetRewarded += GetX2Booster;
        }
        
        private void CancelX2Booster()
        {
            _rewardedAdController.OnVideoClosed -= CancelX2Booster;
            _rewardedAdController.GetRewarded -= GetX2Booster;
        }
        private void GetX2Booster()
        {
            _gameManager.InterTimer = 0;
            StartCoroutine(Delay(() =>
            {
                _gameManager.IncomeBoostTime = 150;
                _gameManager.InterTimer = 0;
            }, 0.2f));
        }
        
        public void RequestSpeedBooster()
        {
            _rewardedAdController.ShowAd();
            _rewardedAdController.OnVideoClosed += CancelSpeedBooster;
            _rewardedAdController.GetRewarded += GetSpeedBooster;
        }
        private void CancelSpeedBooster()
        {
            _rewardedAdController.OnVideoClosed -= CancelSpeedBooster;
            _rewardedAdController.GetRewarded -= GetSpeedBooster;
        }
        private void GetSpeedBooster()
        {
            _gameManager.InterTimer = 0;
            StartCoroutine(Delay(() =>
            {
                _gameManager.SpeedBoostTime = 150;
                _gameManager.InterTimer = 0;
            }, 0.2f));
        }
        
        
        public void  UpdateLevelUpUI(int oldCart, int newCart,float reward,float bonusExp) {
            this.oldCart.text = oldCart.ToString();
            this.newCart.text = newCart.ToString();
            this.reward.text ="+"+ FormatNumber(Mathf.Ceil(reward));
            this.expBonus.text ="+"+ FormatNumber(Mathf.Ceil(bonusExp));
            bonusMoneyText.text = "+" + FormatNumber(Mathf.Ceil(reward*2));
            oldReward = reward;
        }
   
        public void GetLevelUpRewardFree(RectTransform canvas)
        {
            StartCoroutine(_gameManager.LevelUpRoutine(oldReward, 1.3f * _gameManager.MaxExp / 10, false));
            Invoke(nameof(ShowSubscription), 2f);
            TurnOffPopUp(canvas);
        }

        private void ShowSubscription()
        {
            _iapService.ShowSubscriptionPanel();
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
                    _gameManager.IsExpGet = true;

                });
            });
        }

        private int _currentOpenedTab = 0;
        public void OpenTab(int tabIndex)
        {
            if (tabIndex == _currentOpenedTab)
            {
                return;
            }
            _currentOpenedTab = tabIndex;
            foreach (GameObject t in tabs)
            {
                t.SetActive(false);
            }
            tabs[tabIndex].SetActive(true);
            tabs[tabIndex].GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
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
     
        public void NotEnoughMoney()
        {
            unlockRewardUI.SetActive(true);
        }
        public void RewardUnlock()
        {
            StartCoroutine(Delay(() =>
            {
                FindObjectOfType<Unlock>().remain=0;
                unlockRewardUI.SetActive(false);
                _gameManager.InterTimer = 0;
            }, 0.2f));
        } 
        
        public void RequestRewardCar()
        {
            ShowRewardedAdd();
            _rewardedAdController.OnVideoClosed += CancelCarReward;
            _rewardedAdController.GetRewarded += GetRewardCar;
        }

        private void CancelCarReward()
        {
            _rewardedAdController.OnVideoClosed -= CancelCarReward;
            _rewardedAdController.GetRewarded -= GetRewardCar;
        }

        private void GetRewardCar()
        {
            StartCoroutine(Delay(() =>
            {
                GameObject.Find("Player").GetComponent<PlayerControl>().SitInCar(GameObject.Find("PlayerWithCar").GetComponent<Collider>());
                _gameManager.InterTimer = 0; 
            }, 0.2f));
        }

        
        public void ShowRewardedAdd()
        {
            _rewardedAdController.ShowAd();
        }

        public string FormatTime(float seconds)
        {
            int minutes = (int)(seconds / 60);
            int remainingSeconds = (int)(seconds % 60);
            return string.Format("{0:00}:{1:00}", minutes, remainingSeconds);
        }
        public void ReloadGame()
        {
            _dataManager.SaveData();
            SceneManager.LoadScene(0);
        }
        
    }
}
