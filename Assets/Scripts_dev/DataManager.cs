using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameData
{
    public float money;
    public float unlockMoney;
    public string timeQuit;
    public int currentUnlocked;
    public int level;
    public float exp;
    public float freeSpinTime,speedTime, incomeTime;
    public bool firstRolled;
    public float totalPlayTime;
    public List<int> productNumbers;
    public List<int> stackNumbers;
    public List<int> levels;
    public List<int> status;
  //  public List<float> prices;

    public GameData()
    {
        this.money = GameManager.instance.money;
        this.currentUnlocked = GameManager.instance.currentUnlocked;
        this.level = GameManager.instance.level;
        this.exp = GameManager.instance.currentExp;
        freeSpinTime = GameManager.instance.freeSpinTimer;
        speedTime = GameManager.instance.speedBoostTime;
        incomeTime = GameManager.instance.incomeBoostTime;
        firstRolled = GameManager.instance.firstRolled;
        timeQuit = System.DateTime.Now.ToString();
        totalPlayTime = GameManager.instance.totalPlayTime;
        unlockMoney = !(currentUnlocked > GameManager.instance.unlockOrder.Length - 1) ? GameManager.instance.unlockOrder[currentUnlocked].transform.GetChild(1).GetComponent<Unlock>().remain:0;
        GetAllStackInfo();
        GetAllShelvesInfo();
        GetAllLevelInfo();
        GetAllFarmsInfo();
       // GetAllPricesInfo();
    }
    void GetAllStackInfo()
    {
        stackNumbers = new List<int>();
        List<Stack> stacks = new List<Stack>(Object.FindObjectsOfType<Stack>());
        stacks.Sort((a, b) => a.productToShow.id.CompareTo(b.productToShow.id));
        for (int i =0; i< stacks.Count;i++)
        {
            stackNumbers.Add(stacks[i].currentQuantity);
        }
    } void GetAllShelvesInfo()
    {
        productNumbers = new List<int>();
        List<Shelf>shelves = new List<Shelf>(Object.FindObjectsOfType<Shelf>());
        shelves.Sort((a, b) => a.productToShow.id.CompareTo(b.productToShow.id));
        for (int i = 0; i < shelves.Count; i++)
        {
            productNumbers.Add(shelves[i].currentQuantity);
        }
    }
    void GetAllLevelInfo()
    {
        levels = new List<int>();
        List<LevelMangament> levelMangaments =new List<LevelMangament>(Object.FindObjectsOfType<LevelMangament>());
        levelMangaments.Sort((a, b) => a.goods.id.CompareTo(b.goods.id));
        for (int i = 0; i < levelMangaments.Count; i++)
        {
            if(levelMangaments[i].transform.GetChild(0).gameObject.activeInHierarchy)
            levels.Add(levelMangaments[i].level);
        }
    }
    void GetAllFarmsInfo()
    {
        status = new List<int>();
        FarmSlot[] farmSlots = Object.FindObjectsOfType<FarmSlot>();
        status.Add(-1);
        foreach(FarmSlot farm in farmSlots)
        {
            if(farm.farmStatus==2)
            status.Add(farm.productID);
          
        }

    }
   /* void GetAllPricesInfo()
    {
        prices = new List<float>();
        for (int i = 0; i < GameManager.instance.unlockOrder.Length; i++)
        {
                prices.Add(GameManager.instance.unlockOrder[i].transform.GetChild(1).GetComponent<Unlock>().remain);
        }
    }
    */
}
public class DataManager : MonoBehaviour
{
    
    public static DataManager instance;
    public GameData gameData;
    private void Awake()
    {
        
        instance = this;
    }
    private void Start()
    {
        gameData = LoadGameData();
    }

    public void SaveGameData()
    {
        GameData gameData = new GameData();
        string gameDataJson = JsonUtility.ToJson(gameData);
        PlayerPrefs.SetString("GameData", gameDataJson);
        PlayerPrefs.Save();
        
    }
    public GameData LoadGameData()
    {
        if (PlayerPrefs.HasKey("GameData"))
        {
            string savedGameDataJson = PlayerPrefs.GetString("GameData");
            GameData savedGameData = JsonUtility.FromJson<GameData>(savedGameDataJson);
            return savedGameData;
        }
        else
        {
            return null;
        }
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveGameData();
        }
    }
    private void OnApplicationQuit()
    {
        SaveGameData();
    }
}
