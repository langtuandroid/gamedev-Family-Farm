using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyPiile : MonoBehaviour
{
    public float currentQuantity;

    public GameObject productToShow;
    public Vector3 nextPos,prevPos;
    public int nextIndex;
    public bool load;
    public GameObject[] pos;

    private bool updating;

   

    private void Awake()
    {
        foreach (GameObject go in pos)
        {
            GameObject clone = Instantiate(productToShow, go.transform);
            clone.transform.position = go.transform.position;
            clone.SetActive(false);
        }
        nextPos = pos[0].transform.position;
        prevPos = pos[0].transform.position;
        
    }
    private void Update()
    {
       
        UpdateStockLooks();
        if (currentQuantity < 0) currentQuantity = 0;
       
    }
    void UpdateStockLooks()
    {
      // if(!updating)
           StartCoroutine (DelayActive());
        if (Mathf.Ceil(currentQuantity / GameManager.instance.moneyPerPack) < pos.Length)
            DelayDeative();
    }
    public void UpdateNextPos(bool add)
    {
      
        if(nextIndex<pos.Length)
            nextPos = pos[nextIndex].transform.position;
        if(nextIndex>0)
            prevPos = pos[nextIndex-1].transform.position;
        if (add)
            nextIndex++;
        else if (Mathf.Ceil(currentQuantity / GameManager.instance.moneyPerPack) <= pos.Length)
        {
            Debug.Log("Whu bug wtf man");
            nextIndex--;
        }
           
        if (nextIndex >= pos.Length)
        {
            nextIndex = pos.Length - 1;
        }
        if (nextIndex < 0)
        {
            nextIndex = 0;
        }
    }
    public void ReCalculatePos()
    {
        for(int i=0; i < pos.Length; i++)
        {
            if (!pos[i].transform.GetChild(0).gameObject.activeInHierarchy)
            {
                nextIndex = i ;
                break;
            }
        }
    }
    IEnumerator DelayActive()
    {
       
        for (int i = 0; i < (currentQuantity / GameManager.instance.moneyPerPack > pos.Length ? pos.Length :Mathf.Ceil(currentQuantity / GameManager.instance.moneyPerPack)); i++)
        {
            pos[i].transform.GetChild(0).gameObject.SetActive(true);
            yield return null;

        }
       // ReCalculatePos();
       
    }
    void DelayDeative()
    {
        for (int i =(int) (Mathf.Ceil(currentQuantity / GameManager.instance.moneyPerPack)); i < pos.Length; i++)
        {
           // Debug.Log(i);
           if (pos[i].transform.childCount>0)
            pos[i].transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
