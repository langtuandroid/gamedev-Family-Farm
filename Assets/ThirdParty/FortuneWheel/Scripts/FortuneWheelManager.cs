using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using _Project.Scripts_dev.Additional;
using _Project.Scripts_dev.Managers;
using _Project.Scripts_dev.UI;
using Integration;
using UnityEngine.Events;
using TMPro;
using Zenject;
#if UNITY_EDITOR

#endif
public class FortuneWheelManager : MonoBehaviour
{
    [Inject] private SoundManager _soundManager;
    [Inject] private UIManager _uiManager;
    [Inject] private GameManager _gameManager;
    [Inject] private RewardedAdController _rewardedAdController;
    [Header("Game Objects for some elements")]
    public Button FreeTurnButton;              
    public Button btnClose;
    [SerializeField] private Button _videoTurnButton;
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
        if (spinReward.first)
            btnClose.interactable = false;
    }

  
    private void TurnWheelForFree()
    {
        TurnWheel();
       
    }
    public void TurnWheelForAds()
    {
        _rewardedAdController.ShowAd();
        _rewardedAdController.GetRewarded += TurnWheel;
        _rewardedAdController.OnVideoClosed += OnAddClosed;
        _gameManager.InterTimer = 0;
    }

    private void OnAddClosed()
    {
        _rewardedAdController.GetRewarded -= TurnWheel;
        _rewardedAdController.OnVideoClosed -= OnAddClosed;
    }

    private void TurnWheel()
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
                break;
            }
        }

        int fullTurnovers = 5;
        _finalAngle = fullTurnovers * 360 + randomFinalAngle - 20f;
        _isStarted = true;

        _videoTurnButton.interactable = false;
        FreeTurnButton.interactable = false;
        btnClose.interactable = false;
    }

    public void TurnWheelButtonClick()
    {
        if (_gameManager.FreeSpinTime<=0)
        {
            _gameManager.FreeSpinTime = 600;
            TurnWheelForFree();
        }
    }
    

    private void Update()
    {
        if (_gameManager.FreeSpinTime > 0)
        {
            FreeTurnButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _uiManager.FormatTime(_gameManager.FreeSpinTime);
            FreeTurnButton.interactable = false;
        }
        else if (!_isStarted)
        {
            FreeTurnButton.interactable = true;
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
                FreeTurnButton.interactable = true;
            }
            else
            {
                FreeTurnButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _uiManager.FormatTime(_gameManager.FreeSpinTime);
            }
            
            btnClose.interactable = true;
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