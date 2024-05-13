using System.Collections;
using System.Collections.Generic;
using _Project.Scripts_dev.Control;
using _Project.Scripts_dev.UI;
using _Project.Scripts_dev.Сamera;
using Integration;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Zenject;

namespace _Project.Scripts_dev.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Inject] private IAPService _iapService;
        [Inject] private SoundManager _soundManager;
        [Inject] private EffectManager _effectManager;
        [Inject] private DataManager _dataManager;
        [Inject] private GameManager _gameManager;
        [Inject] private UIManager _uiManager;
        [Inject] private AdMobController _adMobController;
        [FormerlySerializedAs("unlockOrder")] public GameObject[] _unlockOrder;
        [FormerlySerializedAs("shops")] public List<GameObject> _shops;
        private float _inactiveTime;
        private float _reward;
        public float Money { get; set; }
        public int MoneyInPack => 10;
        public int CurrentUnlocked { get; set; }
        public int MaxCart { get; private set; } = 40;
        public int Level { get; private set; } = 0;
        public float Exp { get; set; } 
        public float MaxExp { get; private set; }
        public bool IsExpGet { get; set; } 
        public float InterTimer { get; set; }
        public bool IsDraggable { get; set; }
        public float IncomeBoostTime { get; set; }
        public float SpeedBoostTime { get; set; }
        public float TruckTime { get; set; }
        public float IncomeBoost { get; private set; }
        public float SpeedBoost { get; private set; }
        public float FreeSpinTime{ get; set; }
        public bool FirstRolled { get; set; }
        public float TotalPlayTime { get; private set; }
        public bool IsLoad { get; private set; }
        public int SomeTaking { get; set; }
        public float PlayTime { get; private set; }
   
        private void Awake()
        {
            _iapService.OnMoneyBuy += GetMoney;
            _adMobController.ShowBanner(true);
            FreeSpinTime = 0;
            _shops = new List<GameObject>();
        }

        private void GetMoney(int money)
        {
            Money += money;
        }
        private void Start()
        {
            Level = 1;
            IsExpGet = true;
            InterTimer = 50;
            StatUpdate();
            IsDraggable = true;
            UpdateCurrentShops();
            foreach (var order in _unlockOrder)
            {
                order.transform.GetChild(0).gameObject.SetActive(false);
            }
            StartCoroutine(LoadDelay());
        
            if (!IsLoad)
            {
                LoadGame();
            }
        }
        
        private void Update()
        {
            TimerUpdate();
            if (Exp >= MaxExp&&IsDraggable)
            {
                LevelUp();
            }
            if(!IsExpGet) Exp =MaxExp;
            if (TruckTime <= 0)
            {
                MaxCart = (Level - 1) + 10;
            }
            else
            {
                MaxCart = ((Level - 1) + 10) >= 30 ? 80 : 40;
            }
        }

        private void TimerUpdate()
        {
            PlayTime += Time.deltaTime;
            TotalPlayTime += Time.deltaTime;

            InterTimer += Time.deltaTime;
            if (IsDraggable&& TotalPlayTime>60 &&  (InterTimer > 15))
            {
                _inactiveTime += Time.deltaTime;
            }
           
            if (_inactiveTime > 30) _inactiveTime = 30;
            if (Input.GetMouseButton(0))
            {
                _inactiveTime = 0;
            }
            if (IncomeBoostTime > 0)
            {
                IncomeBoostTime -= Time.deltaTime;
                IncomeBoost = 2;
            } if (TruckTime > 0)
            {
                TruckTime -= Time.deltaTime;
          
            }
            else { IncomeBoost = 1; }
            if (SpeedBoostTime > 0)
            {
                SpeedBoostTime -= Time.deltaTime;
                SpeedBoost = 2;
            }
            else
            {
                SpeedBoost = 1;
            }
            if (FreeSpinTime > 0)
            {
                FreeSpinTime -= Time.deltaTime;
            }
        }

        private IEnumerator LoadDelay()
        {
            yield return new WaitForSeconds(2);
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
        }

        private void LoadGame()
        {
            if (_dataManager.GameData == null)
            {
                for (int i = _unlockOrder.Length - 1; i > CurrentUnlocked; i--)
                {
                    _unlockOrder[i].SetActive(false);
                    FirstRolled = false;
                }
             
                if (CurrentUnlocked == 0 && FirstRolled)
                {
                    if (Money < 150) Money = 150;
                }
                IsLoad = true;
                return;
            }
            

            GameData gameData = _dataManager.GameData;
            CurrentUnlocked = gameData.currentUnlocked;
            Money = gameData.money;
            Level = gameData.level;
            Exp = gameData.exp;
            System.DateTime datequit=System.DateTime.Now;
            System.DateTime.TryParse(gameData.timeQuit, out datequit);
            FreeSpinTime = gameData.freeSpinTime-(float)(System.DateTime.Now.Subtract(datequit)).TotalSeconds;
            SpeedBoostTime = gameData.speedTime;
            IncomeBoostTime = gameData.incomeTime;
            FirstRolled = gameData.firstRolled;
            TotalPlayTime = gameData.totalPlayTime;
            StatUpdate();

            for (int i = _unlockOrder.Length - 1; i > CurrentUnlocked; i--)
            {
                _unlockOrder[i].SetActive(false);
                _unlockOrder[i].transform.GetChild(0).gameObject.SetActive(false);
            }
            for (int i = 0; i < CurrentUnlocked; i++)
            {
                _unlockOrder[i].transform.GetChild(0).gameObject.SetActive(true);
                _unlockOrder[i].transform.GetChild(1).gameObject.SetActive(false);
            }
            if (!(CurrentUnlocked > _unlockOrder.Length - 1))
            {
                _unlockOrder[CurrentUnlocked].transform.GetChild(0).gameObject.SetActive(false);
                _unlockOrder[CurrentUnlocked].transform.GetChild(1).gameObject.SetActive(true);
            }

            UpdateLocked();
            if (_dataManager.GameData == null)
            {
                for (int i = _unlockOrder.Length - 1; i > CurrentUnlocked; i--)
                {
                    _unlockOrder[i].SetActive(false);
                    FirstRolled = false;
                }
            }
            IsLoad = true;
            if (CurrentUnlocked == 0&& FirstRolled)
            {
                if (Money < 150) Money = 150;
            }
        }

        private void LevelUp()
        {
            _soundManager.PlaySound(_soundManager.Clips[2]);
            IsExpGet = false;
            float bonusExp = (MaxExp * 1.3f) / 10;
            _uiManager.UILevelUp.SetActive(true);
            _uiManager.UpdateLevelUpUI((Level - 1) + 10, (Level - 1) + 10 + 1, _reward, bonusExp);
        }

        public IEnumerator LevelUpRoutine(float money, float exp, bool reward)
        {
            IsExpGet = true;
            Exp = 0;
            Level++;
            if (Level == 3)
            {
                StartCoroutine(FocusRoutine(GameObject.Find("PlayerWithCar")));
            }
            StatUpdate();
            for (int i = 0; i < 5; i++)
            {
                _effectManager.MoneyEffect(money / 5, FindObjectOfType<PlayerControl>().transform);
                _effectManager.ExperienceEffect(exp / 5, FindObjectOfType<PlayerControl>().transform);
            
                yield return null;
            }

            if (!reward) yield break;
            {
                for (int i = 0; i < 10; i++)
                {
                    _effectManager.MoneyEffect(money / 5, FindObjectOfType<PlayerControl>().transform);

                    yield return null;
                }
            }
        }
        
        private void StatUpdate()
        {
            MaxCart = (Level - 1) +10;
            if (MaxCart > 40) MaxCart = 40;
            MaxExp =  Level<5? 100 * Mathf.Pow(1.3f, Level - 1): 100 * Mathf.Pow(1.3f, 4 - 1) * Mathf.Pow(1.3f, Level - 4);
            _reward=  0.3f*MaxExp;
        }

        public void UpdateCurrentShops()
        {
            _shops.Clear();
            GameObject[] temp;
            temp = GameObject.FindGameObjectsWithTag("Shelf");
            foreach (GameObject go in temp)
            {
                _shops.Add(go);
            }
        }
        public void UpdateLocked()
        {
            GameObject go = CurrentUnlocked >_unlockOrder.Length-1? null : _unlockOrder[CurrentUnlocked];
            if (go == null) return;
            go.SetActive(true);// 
            GameObject focus=null;
            foreach(Transform child in go.transform)
            {
                if (child.CompareTag("Unlock"))
                {
                    focus = child.gameObject;
                    break;
                }
            }
            if (!_unlockOrder[CurrentUnlocked].transform.GetChild(0).CompareTag("Area")) 
                StartCoroutine(FocusRoutine(focus));
        }
        public IEnumerator FocusRoutine(GameObject focus)
        {
            yield return new WaitForSeconds(1f);
            CameraFocus.instance.StartFocus(focus);
        }
    }
}
