using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stack : MonoBehaviour
{
    public int currentQuantity;
    public Shelf shelf;
    public bool destroy;

   public Goods productToShow;
    [SerializeField] GameObject[] pos;
    private bool load;
    Shelf[] shelves;
    bool shelfFound;
    private void Start()
    {
        foreach(GameObject go in pos)
        {
            GameObject clone= Instantiate(productToShow.prefab, go.transform);
            clone.transform.position = go.transform.position;
            clone.transform.rotation = Quaternion.identity;
            clone.SetActive(false);
        }
      
    }
    private void Update()
    {
        if (!shelfFound)
        {
            shelves = FindObjectsOfType<Shelf>();
            foreach (Shelf s in shelves)
            {
                if (s.productNeeded == productToShow.id)
                {
                    shelf = s;
                    shelfFound = true;
                }
            }
        }
        if (DataManager.instance.gameData != null&&!load)
        {
            GameData gameData = DataManager.instance.gameData;
            int[] stackNumbers = gameData.stackNumbers.ToArray();
            load = true;
            if(GameManager.instance.playTime<3)
            currentQuantity = productToShow.id > stackNumbers.Length?0: stackNumbers[productToShow.id - 1];
           
        }
        if (DataManager.instance.gameData == null)
        {
            load = true;
        }
        UpdateStockLooks();
        if(destroy)
        {
            if (currentQuantity <= 0) Destroy(gameObject);
        }
    }
    void UpdateStockLooks()
    {

        DelayActive();
        if(currentQuantity<pos.Length)
            DelayDeative();

    }
    void DelayActive()
    {
        for (int i = 0; i < (currentQuantity > pos.Length ? pos.Length : currentQuantity); i++)
        {
            pos[i].transform.GetChild(0).gameObject.SetActive(true);
            //yield return new WaitForSeconds(0.05f);

        }
    } void DelayDeative()
    {
        for (int i = currentQuantity; i < pos.Length; i++)
        {
            pos[i].transform.GetChild(0).gameObject.SetActive(false);
           // yield return new WaitForSeconds(0.05f);
        }
    }
}
