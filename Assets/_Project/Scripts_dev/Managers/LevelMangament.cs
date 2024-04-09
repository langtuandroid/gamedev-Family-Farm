using System.Collections;
using System.Collections.Generic;
using _Project.Scripts_dev.Classes;
using _Project.Scripts_dev.Configs;
using _Project.Scripts_dev.Managers;
using UnityEngine;
using Zenject;

public class LevelMangament : MonoBehaviour
{
    [Inject] private DataManager _dataManager;
    [Inject] private GameManager _gameManager;
    public int level = 0;

    public GameObject[] unlocks;
    public Goods goods;
    public float multiple;
    public List<float> prices;
    public int titleIndex;
    public int type;
    public Sprite sprite;

     bool load;
    private void Start()
    {
        prices = new List<float>();
        prices.Add(goods.Income * 100);
        prices.Add(prices[0]*6);
        foreach(GameObject go in unlocks)
        {
            go.SetActive(false);
        }
    }
    private void Update()
    {
        if(!load)
        {
            if (_dataManager.gameData != null && _gameManager.playTime<3)
            {
                GameData gameData = _dataManager.gameData;
                if (transform.GetChild(0).gameObject.activeInHierarchy)
                {
                    load = true; 
                    level = goods.Id >gameData.levels.Count?0: gameData.levels[goods.Id - 1];
                }
               
        
            }
            else
            {
                load = true;
            }
            return;

        }
        if (unlocks.Length > 0)
        {
            for (int i = 0; i < level; i++)
            {
                unlocks[i].SetActive(true);
            }
        }

    }
    public void Upgrade()
    {
        level++;
       
    }
}
