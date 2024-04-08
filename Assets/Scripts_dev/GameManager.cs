using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float money;
    public List<GameObject> shops;
    public int moneyPerPack;
    public GameObject[] unlockOrder;
    public int currentUnlocked;
    public int maxCart, level;
    public float currentExp;
    public float maxExp;
    public float reward;
    public bool gainExp;
    public float interTimer;
    public float inactiveTimer;
    public float playTimer;
    public bool draggable;
    public float incomeBoostTime, speedBoostTime,truckTime;
    public float incomeBoost, speedBoost;
    public float freeSpinTimer;
    public bool firstRolled;
    public float totalPlayTime;

 
    public bool load;
    public int some1Taking;
    public float playTime;

    public bool dragon;

    
    // Start is called before the first frame update
    void Awake()
    {
        freeSpinTimer = 0;
        instance = this;
        shops = new List<GameObject>();
    }
    private void Start()
    {
        level = 1;
        gainExp = true;
        interTimer = 50;
        UpdateStats();
        draggable = true;
        UpdateCurrentShops();
        for (int i = 0; i <unlockOrder.Length; i++)
        {
          
            unlockOrder[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        StartCoroutine(Delay());
    }
    

    private void Update()
    {
       
        //Time.timeScale = 4;
        TimerManage();
        if (!load)
        {
            LoadGame();
        }
       // if (level == 10) return;
        if (currentExp >= maxExp&&draggable)
        {
            LevelUp();
        }
       
        if(!gainExp) currentExp =maxExp;
        if (truckTime <= 0)
        {
            maxCart = (level - 1) + 10;

        }
        else
        {
            maxCart = ((level - 1) + 10) >= 30 ? 80 : 40;
        }

    }
   
    void TimerManage()
    {
        
        playTime += Time.deltaTime;
        totalPlayTime += Time.deltaTime;

        interTimer += Time.deltaTime;
        if (draggable&& totalPlayTime>60 &&  (interTimer > 15))
        {
           
                inactiveTimer += Time.deltaTime;
            playTimer += Time.deltaTime;
        }
           
        if (inactiveTimer > 30) inactiveTimer = 30;
        if (Input.GetMouseButton(0))
        {
            inactiveTimer = 0;
        }
        if (incomeBoostTime > 0)
        {
            incomeBoostTime -= Time.deltaTime;
            incomeBoost = 2;
        } if (truckTime > 0)
        {
            truckTime -= Time.deltaTime;
          
        }
        else { incomeBoost = 1; }
        if (speedBoostTime > 0)
        {
            speedBoostTime -= Time.deltaTime;
            speedBoost = 2;
        }
        else
        {
            speedBoost = 1;
        }
        if (freeSpinTimer > 0)
        {
            freeSpinTimer -= Time.deltaTime;
        }
       // maxCart = 80;
        //maxCart = (level - 1) + 10;
    }
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(2);
        
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
        Debug.Log("Current active : " + SceneManager.GetActiveScene().name);
      
    }
    void LoadGame()
    {
        
       
        if (DataManager.instance.gameData != null && !load)
        {
            

            GameData gameData = DataManager.instance.gameData;

            currentUnlocked = gameData.currentUnlocked;
            
           
            money = gameData.money;

            level = gameData.level;
            currentExp = gameData.exp;
            System.DateTime datequit=System.DateTime.Now;
            System.DateTime.TryParse(gameData.timeQuit, out datequit);
            freeSpinTimer = gameData.freeSpinTime-(float)(System.DateTime.Now.Subtract(datequit)).TotalSeconds;
            speedBoostTime = gameData.speedTime;
            incomeBoostTime = gameData.incomeTime;
            firstRolled = gameData.firstRolled;
            totalPlayTime = gameData.totalPlayTime;
            UpdateStats();

            for (int i = unlockOrder.Length - 1; i > currentUnlocked; i--)
            {
                unlockOrder[i].SetActive(false);
                unlockOrder[i].transform.GetChild(0).gameObject.SetActive(false);
            }
            for (int i = 0; i < currentUnlocked; i++)
            {
                unlockOrder[i].transform.GetChild(0).gameObject.SetActive(true);
                unlockOrder[i].transform.GetChild(1).gameObject.SetActive(false);
            }
            if (!(currentUnlocked > unlockOrder.Length - 1))
            {
                unlockOrder[currentUnlocked].transform.GetChild(0).gameObject.SetActive(false);
                unlockOrder[currentUnlocked].transform.GetChild(1).gameObject.SetActive(true);
            }

            UpdateUnlocked();
        }
        if (DataManager.instance.gameData == null)
        {
            for (int i = unlockOrder.Length - 1; i > currentUnlocked; i--)
            {
                unlockOrder[i].SetActive(false);
                firstRolled = false;
                

            }
        }
        load = true;
        if (currentUnlocked == 0&& firstRolled)
        {
            if (money < 150) money = 150;
        }
    }
    void LevelUp()
    {
        
        SoundManager.instance.PlaySound(SoundManager.instance.sounds[2]);
        gainExp = false;
        float bonusExp = (maxExp*1.3f) / 10;
        UIManager.instance.UILevelUp.SetActive(true);
        UIManager.instance.UpdateLevelUpUI((level - 1) + 10, (level - 1) + 10  + 1, reward,bonusExp);
    }
    public IEnumerator GetLevelUpRewardAdsCoroutine(float money, float exp, bool reward)
    {
        gainExp = true;
        currentExp = 0;
        level++;
        if (level == 3)
        {
            StartCoroutine(DelayFocus(GameObject.Find("PlayerWithCar")));
        }
        UpdateStats();
        for (int i = 0; i < 5; i++)
        {
            EffectManager.instance.GetMoneyEffect(money / 5, FindObjectOfType<PlayerControl>().transform);
            EffectManager.instance.GetExpEffect(exp / 5, FindObjectOfType<PlayerControl>().transform);
            
            yield return null;
        }
        
        if (reward)
            for (int i = 0; i < 10; i++)
            {
                EffectManager.instance.GetMoneyEffect(money / 5, FindObjectOfType<PlayerControl>().transform);

                yield return null;
            }

    }


    void UpdateStats()
    {
       
        maxCart = (level - 1) +10;
        if (maxCart > 40) maxCart = 40;
        maxExp =  level<5? 100 * Mathf.Pow(1.3f, level - 1): 100 * Mathf.Pow(1.3f, 4 - 1) * Mathf.Pow(1.3f, level - 4);
        reward=  0.3f*maxExp;

    }

    public void UpdateCurrentShops()
    {
        shops.Clear();
        GameObject[] temp;
        temp = GameObject.FindGameObjectsWithTag("Shelf");
        foreach (GameObject go in temp)
        {
            shops.Add(go);
        }
    }
    public void UpdateUnlocked()
    {
        GameObject go = currentUnlocked >unlockOrder.Length-1? null : unlockOrder[currentUnlocked];
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
        if (!unlockOrder[currentUnlocked].transform.GetChild(0).CompareTag("Area")) 
        StartCoroutine(DelayFocus(focus));
    }
    public IEnumerator DelayFocus(GameObject focus)
    {
        yield return new WaitForSeconds(1f);
        FocusCamera.instance.SetCameraFocus(focus);
    }
}
