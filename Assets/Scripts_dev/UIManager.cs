using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    
    public static UIManager instance;
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
        
        instance = this;
       // shouldShowUnlockReward = true;
        music =PlayerPrefs.GetFloat("music", 1)==1?true:false;
        sound=  PlayerPrefs.GetFloat("sound", 1)==1?true :false;
        vnmeseImage.SetActive(PlayerPrefs.GetFloat("language", 1) == 0 ? true : false);
        
    }

    void Update()
    {

        if (!GameManager.instance.firstRolled&&DataManager.instance.gameData==null)
        {
            StartCoroutine(Delay(() => FirstSpinWheel.SetActive(true),1.5f));
            BottomUI.SetActive(false);
            GameManager.instance.firstRolled = true;
        }
       
        if (FindObjectOfType<FarmSlot>() != null)
        {
            UpgradeIcon.SetActive(true);
        }
        moneyText.text =FormatNumber(Mathf.Ceil(GameManager.instance.money));
        EXPBarImage.fillAmount = GameManager.instance.currentExp / GameManager.instance.maxExp;
        levelText.text = GameManager.instance.level.ToString();
        expText.text = Mathf.Ceil(GameManager.instance.currentExp) + "/" + Mathf.Ceil(GameManager.instance.maxExp);
        if(GameManager.instance.truckTime<=0)
        cartText.text = playerCart.inCart + "/" + GameManager.instance.maxCart;
        else
        {
            cartText.text = truckCart.inCart + "/" + GameManager.instance.maxCart;
        }

        UpdateSoundIcons();
        if (GameManager.instance.freeSpinTimer <= 0)
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
        if (shouldShowUnlockReward&&GameManager.instance.money==0&&FindObjectOfType<Unlock>().remain>=1&&GameManager.instance.currentUnlocked!=0)
        {
            StartCoroutine(DelayUnlock());
            shouldShowUnlockReward = false;
        }
    }
    IEnumerator DelayUnlock()
    {
        yield return null;
        if (GameManager.instance.money == 0 &&(FindObjectOfType<Unlock>().unlockTime > 1 || FindObjectOfType<Unlock>().remain != FindObjectOfType<Unlock>().price))
        {
            unlockRewardUI.SetActive(true);
           
        }
      
    }
    void UpdateBoostButton()
    {
        Button incomeButton = incomeRemain.transform.parent.GetComponent<Button>();
        Button speedButton = speedRemain.transform.parent.GetComponent<Button>();

        float incomeTime = GameManager.instance.incomeBoostTime;
        float speedTime = GameManager.instance.speedBoostTime;
        incomeRemain.text =incomeTime>0? FormatTime(incomeTime):"3m";
        speedRemain.text = speedTime>0?FormatTime(speedTime):"3m";
        if (GameManager.instance.incomeBoostTime>0)
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
        if (GameManager.instance.speedBoostTime>0)
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
                GameManager.instance.speedBoostTime = 150;
                GameManager.instance.interTimer = 0;
                GameManager.instance.playTimer = 0;
            }, 0.2f));
        }
        if (type == 0)
        {
            StartCoroutine(Delay(() =>
            {
                GameManager.instance.incomeBoostTime = 150;
                GameManager.instance.interTimer = 0;
                GameManager.instance.playTimer = 0;
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
            StartCoroutine(GameManager.instance.GetLevelUpRewardAdsCoroutine(oldReward, 1.3f * GameManager.instance.maxExp / 10, true));
            GameManager.instance.interTimer = 0;
            GameManager.instance.playTimer = 0;

            TurnOffPopUp(canvas);
        }, 0.2f));
    } 
    public void GetLevelUpRewardFree(RectTransform canvas)
    {
            StartCoroutine(GameManager.instance.GetLevelUpRewardAdsCoroutine(oldReward, 1.3f * GameManager.instance.maxExp / 10, false));
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
                GameManager.instance.gainExp = true;

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
        if (GameManager.instance.interTimer >= 45&&GameManager.instance.totalPlayTime>60)
        {
            
            GameManager.instance.playTimer = 45 - 5;
        }
    }
    public void RewardUnlock()
    {
        StartCoroutine(Delay(() =>
        {
            FindObjectOfType<Unlock>().remain=0;
            unlockRewardUI.SetActive(false);
            GameManager.instance.interTimer = 0;
            GameManager.instance.playTimer = 0;
        }, 0.2f));
    } public void RewardCar()
    {
        StartCoroutine(Delay(() =>
        {
            GameObject.Find("Player").GetComponent<PlayerControl>().GetCar(GameObject.Find("PlayerWithCar").GetComponent<Collider>());
            GameManager.instance.interTimer = 0; GameManager.instance.playTimer = 0;
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
            LanguageManager.instance.language = 0;
        }
        else { PlayerPrefs.SetFloat("language", 1);
            LanguageManager.instance.language = 1;
        }
        LanguageManager.instance.SetTextAll();
    }
}
