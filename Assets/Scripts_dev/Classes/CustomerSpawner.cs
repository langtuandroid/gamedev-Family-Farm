using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] int spawnNumber;
    [SerializeField] GameObject[] customerPrefab;
    [SerializeField] Transform[] spawnLocations;
    private void Start()
    {

        StartCoroutine(DelaySpaw());
    }
    void SpawnCustomer()
    {
        GameManager.instance.UpdateCurrentShops();
        if (GameManager.instance.shops.Count == 0) return;
        spawnNumber = GameManager.instance.shops.Count + 3;
        if (spawnNumber > 15) spawnNumber = 15;
        if (GameObject.FindGameObjectsWithTag("Customer").Length<spawnNumber)
        Instantiate(customerPrefab[Random.Range(0,2)],spawnLocations[Random.Range(0,spawnLocations.Length)].transform.position, Quaternion.identity);
    }
    
    IEnumerator DelaySpaw()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            yield return new WaitForSeconds(2.5f);
            SpawnCustomer();
        }
      
    }
}
