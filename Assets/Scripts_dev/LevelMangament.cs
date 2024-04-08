using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMangament : MonoBehaviour
{
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
        prices.Add(goods.income * 100);
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
            if (DataManager.instance.gameData != null&&GameManager.instance.playTime<3)
            {
                GameData gameData = DataManager.instance.gameData;
                if (transform.GetChild(0).gameObject.activeInHierarchy)
                {
                    load = true; 
                    level = goods.id >gameData.levels.Count?0: gameData.levels[goods.id - 1];
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
