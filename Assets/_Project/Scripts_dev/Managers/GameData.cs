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
            this.money = _gameManager.Money;
            this.currentUnlocked = _gameManager.CurrentUnlocked;
            this.level = _gameManager.Level;
            this.exp = _gameManager.Exp;
            freeSpinTime = _gameManager.FreeSpinTime;
            speedTime = _gameManager.SpeedBoostTime;
            incomeTime = _gameManager.IncomeBoostTime;
            firstRolled = _gameManager.FirstRolled;
            timeQuit = System.DateTime.Now.ToString();
            totalPlayTime = _gameManager.TotalPlayTime;
            unlockMoney = !(currentUnlocked > _gameManager._unlockOrder.Length - 1) ? _gameManager._unlockOrder[currentUnlocked].transform.GetChild(1).GetComponent<Unlock>().remain:0;
            StackData();
            ShelvesData();
            LevelData();
            FarmData();
        }

        private void StackData()
        {
            stackNumbers = new List<int>();
            List<Stack> stacks = new List<Stack>(Object.FindObjectsOfType<Stack>());
            stacks.Sort((a, b) => a.productToShow.Id.CompareTo(b.productToShow.Id));
            foreach (var stack in stacks)
            {
                stackNumbers.Add(stack.currentQuantity);
            }
        }

        private void ShelvesData()
        {
            productNumbers = new List<int>();
            List<Shelf>shelves = new List<Shelf>(Object.FindObjectsOfType<Shelf>());
            shelves.Sort((a, b) => a._productsToDisplay.Id.CompareTo(b._productsToDisplay.Id));
            for (int i = 0; i < shelves.Count; i++)
            {
                productNumbers.Add(shelves[i].Quantity);
            }
        }

        private void LevelData()
        {
            levels = new List<int>();
            List<LevelMangament> levelMangaments =new List<LevelMangament>(Object.FindObjectsOfType<LevelMangament>());
            levelMangaments.Sort((a, b) => a._goods.Id.CompareTo(b._goods.Id));
            for (int i = 0; i < levelMangaments.Count; i++)
            {
                if(levelMangaments[i].transform.GetChild(0).gameObject.activeInHierarchy)
                    levels.Add(levelMangaments[i].Level);
            }
        }

        private void FarmData()
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