using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using _Project.Scripts_dev.Additional;
using _Project.Scripts_dev.Language;
using _Project.Scripts_dev.Managers;
using _Project.Scripts_dev.UI;
using UnityEngine.Events;
using TMPro;
using Zenject;
#if UNITY_EDITOR

#endif
public class FortuneWheelManager : MonoBehaviour
{
    [Inject] private SoundManager _soundManager;
    [Inject] private LanguageManager _languageManager;
    [Inject] private UIManager _uiManager;
    [Inject] private GameManager _gameManager;
    [Header("Game Objects for some elements")]
    public Button PaidTurnButton;              
    public Button FreeTurnButton;              
    public Button btnClose;
    public GameObject Circle;                  
    private bool _isStarted;                

    [Header("Params for each sector")]
    public FortuneWheelSector[] Sectors;       

    private float _finalAngle;                
    private float _startAngle;                  
    private float _currentLerpRotationTime;    
    
    private FortuneWheelSector _finalSector;
    [SerializeField] TextMeshProUGUI txtTime;
    public WheelManager spinReward;

    private void Start()
    {
        if(spinReward.first) 
            DisableButton(btnClose);
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
        _soundManager.CreateSound(_soundManager.Clips[10], transform.position);
        _currentLerpRotationTime = 0f;
        int[] sectorsAngles = new int[Sectors.Length];
        for (int i = 1; i <= Sectors.Length; i++)
        {
            sectorsAngles[i - 1] = 360 / Sectors.Length * i;
        }
        double rndNumber = UnityEngine.Random.Range(0f, Sectors.Sum(sector => sector.Probability));
        float cumulativeProbability = 0;
        int randomFinalAngle = sectorsAngles[0];
        _finalSector = Sectors[0];

        for (int i = 0; i < Sectors.Length; i++)
        {
            cumulativeProbability += Sectors[i].Probability;

            if (rndNumber <= cumulativeProbability)
            {
                randomFinalAngle = sectorsAngles[i];
                _finalSector = Sectors[i];
                Debug.Log(Sectors.Length);
                break;
            }
        }

        int fullTurnovers = 5;
        _finalAngle = fullTurnovers * 360 + randomFinalAngle - 20f;
        _isStarted = true;
        DisableButton(FreeTurnButton);
        DisableButton(PaidTurnButton);
        DisableButton(btnClose);
    }

    public void TurnWheelButtonClick()
    {
       
        if (_gameManager.FreeSpinTime<=0)
        {
            _gameManager.FreeSpinTime = 600;
            TurnWheelForFree();
        }
        else
        {
            TurnWheelForAds();
            _gameManager.InterTimer = 0;
           
        }
    }

 


    private void Update()
    {
        if (_gameManager.FreeSpinTime > 0)
        {
            FreeTurnButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _uiManager.FormatTime(_gameManager.FreeSpinTime);
            DisableButton(FreeTurnButton);
        }
        else
        {
            EnableButton(FreeTurnButton);
            _languageManager.SetText(35, FreeTurnButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>());
        }
        if (!_isStarted)
            return;

        float maxLerpRotationTime = 4f;
        _currentLerpRotationTime += Time.deltaTime;
        if (_currentLerpRotationTime > maxLerpRotationTime || Circle.transform.eulerAngles.z == _finalAngle)
        {
            _currentLerpRotationTime = maxLerpRotationTime;
            _isStarted = false;
            if (_gameManager.FreeSpinTime <= 0)
            {
                EnableButton(FreeTurnButton);
                _languageManager.SetText(35, FreeTurnButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>());
            }
            else
            {
                FreeTurnButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _uiManager.FormatTime(_gameManager.FreeSpinTime);
                
            }
               
            EnableButton(PaidTurnButton);
            EnableButton(btnClose);
            _startAngle = _finalAngle % 360;
            _finalSector.RewardCallback.Invoke();
        }
        else
        {
            float t = _currentLerpRotationTime / maxLerpRotationTime;
            t = t * t * t * (t * (6f * t - 15f) + 10f);

            float angle = Mathf.Lerp(_startAngle, _finalAngle, t);
            Circle.transform.eulerAngles = new Vector3(0, 0, angle);
        }
    }

    public void RewardCoins(float awardCoins)
    {
        Action action = spinReward.TakePrize(awardCoins);
        if (!spinReward.first)
        {
            action = spinReward.TakePrize(awardCoins);
        }
        else
        {
            action = spinReward.TakeFirstPrize(awardCoins);
        }

        action();
        if (spinReward.first)
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

[Serializable]
public class FortuneWheelSector : System.Object
{
    [Tooltip("Chance that this sector will be randomly selected")]
    [Range(0, 100)]
    public float Probability = 100;

    [Tooltip("Method that will be invoked if this sector will be randomly selected")]
    public UnityEvent RewardCallback;
}