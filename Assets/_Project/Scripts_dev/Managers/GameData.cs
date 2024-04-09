using System.Collections.Generic;
using _Project.Scripts_dev.Classes;
using _Project.Scripts_dev.Farm;
using _Project.Scripts_dev.Items;
using UnityEngine;

namespace _Project.Scripts_dev.Managers
{
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
        private GameManager _gameManager;

        public GameData(GameManager gameManager)
        {
            _gameManager = gameManager;
            this.money = _gameManager.money;
            this.currentUnlocked = _gameManager.currentUnlocked;
            this.level = _gameManager.level;
            this.exp = _gameManager.currentExp;
            freeSpinTime = _gameManager.freeSpinTimer;
            speedTime = _gameManager.speedBoostTime;
            incomeTime = _gameManager.incomeBoostTime;
            firstRolled = _gameManager.firstRolled;
            timeQuit = System.DateTime.Now.ToString();
            totalPlayTime = _gameManager.totalPlayTime;
            unlockMoney = !(currentUnlocked > _gameManager.unlockOrder.Length - 1) ? _gameManager.unlockOrder[currentUnlocked].transform.GetChild(1).GetComponent<Unlock>().remain:0;
            GetAllStackInfo();
            GetAllShelvesInfo();
            GetAllLevelInfo();
            GetAllFarmsInfo();
        }
        void GetAllStackInfo()
        {
            stackNumbers = new List<int>();
            List<Stack> stacks = new List<Stack>(Object.FindObjectsOfType<Stack>());
            stacks.Sort((a, b) => a.productToShow.Id.CompareTo(b.productToShow.Id));
            for (int i =0; i< stacks.Count;i++)
            {
                stackNumbers.Add(stacks[i].currentQuantity);
            }
        } void GetAllShelvesInfo()
        {
            productNumbers = new List<int>();
            List<Shelf>shelves = new List<Shelf>(Object.FindObjectsOfType<Shelf>());
            shelves.Sort((a, b) => a._productsToDisplay.Id.CompareTo(b._productsToDisplay.Id));
            for (int i = 0; i < shelves.Count; i++)
            {
                productNumbers.Add(shelves[i].Quantity);
            }
        }
        void GetAllLevelInfo()
        {
            levels = new List<int>();
            List<LevelMangament> levelMangaments =new List<LevelMangament>(Object.FindObjectsOfType<LevelMangament>());
            levelMangaments.Sort((a, b) => a.goods.Id.CompareTo(b.goods.Id));
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

    }
}