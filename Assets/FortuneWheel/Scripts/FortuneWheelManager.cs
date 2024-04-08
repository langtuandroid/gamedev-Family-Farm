using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.Events;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class FortuneWheelManager : MonoBehaviour
{
   
    [Header("Game Objects for some elements")]
    public Button PaidTurnButton;               // This button is showed when you can turn the wheel for coins
    public Button FreeTurnButton;               // This button is showed when you can turn the wheel for free
    public Button btnClose;
    public GameObject Circle;                   // Rotatable GameObject on scene with reward objects
    private bool _isStarted;                    // Flag that the wheel is spinning

    [Header("Params for each sector")]
    public FortuneWheelSector[] Sectors;        // All sectors objects

    private float _finalAngle;                  // The final angle is needed to calculate the reward
    private float _startAngle;                  // The first time start angle equals 0 but the next time it equals the last final angle
    private float _currentLerpRotationTime;     // Needed for spinning animation


    // Flag that player can turn the wheel for free right now
    private bool _isFreeTurnAvailable;

    private FortuneWheelSector _finalSector;
    [SerializeField] TextMeshProUGUI txtTime;
    int timeSpinFree = 600;
    public SpinRewardControler spinReward;
    private void Awake()
    {

        //// Show sector reward value in text object if it's set
        //foreach (var sector in Sectors)
        //{
        //    if (sector.ValueTextObject != null)
        //    {
        //        float moneyReward = sector.RewardValue;
        //        sector.ValueTextObject.GetComponent<Text>().text = CurrencyManager.Convert(moneyReward).ToString();
        //    }
        //}
       
    }
    private void Start()
    {
     
        UpdateSpinFree();
        if(spinReward.first) DisableButton(btnClose);
    }
    void UpdateSpinFree()
    {
       /* Observable.Interval(TimeSpan.FromSeconds(1)).TakeUntilDestroy(this).Subscribe(_ =>
        {
            if (_isFreeTurnAvailable)
            {
                EnableButton(FreeTurnButton);
                txtTime.text = LanguageManager.instance?.GetValueLanguage(64);
                UIController.instance.UIMain.iconNotiSpin.SetActive(true);
            }
            else
            {
                if (CurrencyManager.Offline(PlayerPrefSave.TimeSpinWheel) > timeSpinFree)
                {
                    _isFreeTurnAvailable = true;
                    EnableButton(FreeTurnButton);
                    txtTime.text = LanguageManager.instance?.GetValueLanguage(64);
                    UIController.instance.UIMain.iconNotiSpin.SetActive(true);
                }
                else
                {
                    _isFreeTurnAvailable = false;
                    DisableButton(FreeTurnButton);
                    txtTime.text = CurrencyManager.ConvertTime(timeSpinFree - CurrencyManager.Offline(PlayerPrefSave.TimeSpinWheel));
                    UIController.instance.UIMain.iconNotiSpin.SetActive(false);
                }
            }
        });*/
    }
    
    public void OpenSpin()
    {
        UpdateValueReward();
        //UIController.instance.UIMain.OnOffUIMachine(false);
    }
    public void ClosePopup()
    {

       // UIController.instance.UIMain.OnOffUIMachine(true);
    }
    void UpdateValueReward()
    {
      /*  foreach (var sector in Sectors)
        {
            if (sector.ValueTextObject != null)
            {
                double moneyReward = sector.RewardValue;
                if (sector.typeRewardSpin == TypeRewardSpin.Gem)
                {

                }
                else if (sector.typeRewardSpin == TypeRewardSpin.Gold)
                {
                    moneyReward = PlayerPrefSave.LevelCurrent * 2 * moneyReward - moneyReward;
                }
                sector.ValueTextObject.GetComponent<Text>().text = CurrencyManager.Convert(moneyReward).ToString();
            }
        }
        AnalyticManager.LogEventOpen_SpinLucky();*/

    }
    private void TurnWheelForFree()
    {
        TurnWheel(true);
       
    }
    private void TurnWheelForAds()
    {
        TurnWheel(false);
       
    }
    private void TurnWheel(bool isFree)
    {
        SoundManager.instance.CreateSound(SoundManager.instance.sounds[10], transform.position);
        _currentLerpRotationTime = 0f;

        // All sectors angles
        int[] sectorsAngles = new int[Sectors.Length];

        // Fill the necessary angles (for example if we want to have 12 sectors we need to fill the angles with 30 degrees step)
        // It's recommended to use the EVEN sectors count (2, 4, 6, 8, 10, 12, etc)
        for (int i = 1; i <= Sectors.Length; i++)
        {
            sectorsAngles[i - 1] = 360 / Sectors.Length * i;
        }

        //int cumulativeProbability = Sectors.Sum(sector => sector.Probability);

        double rndNumber = UnityEngine.Random.Range(0f, Sectors.Sum(sector => sector.Probability));

        // Calculate the propability of each sector with respect to other sectors
        float cumulativeProbability = 0;
        // Random final sector accordingly to probability
        int randomFinalAngle = sectorsAngles[0];
        _finalSector = Sectors[0];

        for (int i = 0; i < Sectors.Length; i++)
        {
            cumulativeProbability += Sectors[i].Probability;

            if (rndNumber <= cumulativeProbability)
            {
                // Choose final sector
                randomFinalAngle = sectorsAngles[i];
                _finalSector = Sectors[i];
                Debug.Log(Sectors.Length);
                break;
            }
        }

        int fullTurnovers = 5;

        // Set up how many turnovers our wheel should make before stop
        _finalAngle = fullTurnovers * 360 + randomFinalAngle - 20f;

        // Stop the wheel
        _isStarted = true;
        DisableButton(FreeTurnButton);
        DisableButton(PaidTurnButton);
        DisableButton(btnClose);
        // Decrease money for the turn if it is not free turn
        if (!isFree)
        {

            //done spin paid
        }
        else
        {

            // Restart timer to next free turn
            SetNextFreeTime();
        }
    }

    public void TurnWheelButtonClick()
    {
       
        if (GameManager.instance.freeSpinTimer<=0)
        {
            GameManager.instance.freeSpinTimer = 600;
            TurnWheelForFree();
        }
        else
        {
            TurnWheelForAds();
            GameManager.instance.interTimer = 0;
           
        }
    }

    public void SetNextFreeTime()
    {
        _isFreeTurnAvailable = false;
    }


    private void Update()
    {
        if (GameManager.instance.freeSpinTimer > 0)
        {
            FreeTurnButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = UIManager.instance.FormatTime(GameManager.instance.freeSpinTimer);
            DisableButton(FreeTurnButton);
        }
        else
        {
            EnableButton(FreeTurnButton);
            LanguageManager.instance.SetText(35, FreeTurnButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>());
        }
        if (!_isStarted)
            return;
       
        // Animation time
        float maxLerpRotationTime = 4f;

        // increment animation timer once per frame
        _currentLerpRotationTime += Time.deltaTime;

        // If the end of animation
        if (_currentLerpRotationTime > maxLerpRotationTime || Circle.transform.eulerAngles.z == _finalAngle)
        {
            _currentLerpRotationTime = maxLerpRotationTime;
            _isStarted = false;
            if (GameManager.instance.freeSpinTimer <= 0)
            {
                EnableButton(FreeTurnButton);
                LanguageManager.instance.SetText(35, FreeTurnButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>());
            }
            else
            {
                FreeTurnButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = UIManager.instance.FormatTime(GameManager.instance.freeSpinTimer);
                
            }
               
            EnableButton(PaidTurnButton);
            EnableButton(btnClose);
            _startAngle = _finalAngle % 360;

            //GiveAwardByAngle ();
            _finalSector.RewardCallback.Invoke();
        }
        else
        {
            // Calculate current position using linear interpolation
            float t = _currentLerpRotationTime / maxLerpRotationTime;

            // This formulae allows to speed up at start and speed down at the end of rotation.
            // Try to change this values to customize the speed
            t = t * t * t * (t * (6f * t - 15f) + 10f);

            float angle = Mathf.Lerp(_startAngle, _finalAngle, t);
            Circle.transform.eulerAngles = new Vector3(0, 0, angle);
        }
    }

    /// <summary>
    /// Sample callback for giving reward (in editor each sector have Reward Callback field pointed to this method)
    /// </summary>
    /// <param name="awardCoins">Coins for user</param>
    public void RewardCoins(float awardCoins)
    {
       Action action = spinReward.GetPrice(awardCoins);
        if (!spinReward.first)
        {
            action = spinReward.GetPrice(awardCoins);
        }
        else
        {
            action = spinReward.GetFirstPrice(awardCoins);
        }
        action();
        if(spinReward.first==true)
        StartCoroutine(DelayDisable());
    }
    IEnumerator DelayDisable()
    {
        yield return new WaitForSeconds(3);
        gameObject.SetActive(false);
    }
    private void EnableButton(Button button)
    {
        button.interactable = true;
    }

    private void DisableButton(Button button)
    {
        button.interactable = false;
    }


}

/**
 * One sector on the wheel
 */
[Serializable]
public class FortuneWheelSector : System.Object
{
    

    [Tooltip("Text object where value will be placed (not required)")]
    public GameObject ValueTextObject;

    [Tooltip("Value of reward")]
    public float RewardValue = 100;

    [Tooltip("Chance that this sector will be randomly selected")]
    [RangeAttribute(0, 100)]
    public float Probability = 100;

    [Tooltip("Method that will be invoked if this sector will be randomly selected")]
    public UnityEvent RewardCallback;
}