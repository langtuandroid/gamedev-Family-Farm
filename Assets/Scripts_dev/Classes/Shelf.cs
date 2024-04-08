using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    public int currentQuantity;
    public int productNeeded;

    public Goods productToShow;
    [SerializeField] GameObject[] pos;
    [SerializeField] MatPlace matPlace;
    private bool load;

    private void Start()
    {
        foreach (GameObject go in pos)
        {
            GameObject clone = Instantiate(productToShow.prefab, go.transform);
            clone.transform.position = go.transform.position;
            clone.SetActive(false);
        }
    }
    private void Update()
    {
        if (DataManager.instance.gameData != null && !load)
        {
            GameData gameData = DataManager.instance.gameData;
            int[] productNumbers = gameData.productNumbers.ToArray();
            load = true;
            if (GameManager.instance.playTime < 3) 
            currentQuantity = productToShow.id > productNumbers.Length? 0 : productNumbers[productToShow.id - 1];
           

        }
        if (DataManager.instance.gameData == null)
        {
            load = true;
        }
        if (!load) return;
        UpdateStockLooks();
    }
    void UpdateStockLooks()
    {

        StartCoroutine(DelayActive());
        if (currentQuantity < pos.Length)
            StartCoroutine(DelayDeative());

    }
    IEnumerator DelayActive()
    {
        for (int i = 0; i < (currentQuantity > pos.Length ? pos.Length : currentQuantity); i++)
        {
            pos[i].transform.GetChild(0).gameObject.SetActive(true);
            yield return new WaitForSeconds(0.05f);

        }
    }
    IEnumerator DelayDeative()
    {
        for (int i = currentQuantity; i < pos.Length; i++)
        {
            pos[i].transform.GetChild(0).gameObject.SetActive(false);
            yield return new WaitForSeconds(0.05f);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Shipper") || other.CompareTag("Player"))
        {
           
            string tag = other.gameObject.tag;
            Cart cart = tag == "Shipper" ? other.GetComponent<AIController>().cart : other.GetComponent<PlayerControl>().cart;
           
            if (CheckCart(cart))// xem worker co mat nha may can khong
            {
                
                cart.Remove(transform.name,productNeeded, matPlace, 10000, true, this);
            }
        }
    }

    bool CheckCart(Cart cart)
    {
        
        List<GameObject> list = cart.cart; 
        foreach (GameObject go in list)
        {
           
                if (go.GetComponent<Farm.Product>().goods.id == productNeeded)
                {
                    return true;
                }

           
        }
        return false;
    }
}
