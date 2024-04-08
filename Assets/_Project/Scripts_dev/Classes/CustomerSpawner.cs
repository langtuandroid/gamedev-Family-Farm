using System.Collections;
using UnityEngine;
using Zenject;

namespace _Project.Scripts_dev.Classes
{
    public class CustomerSpawner : MonoBehaviour
    {
        [Inject] private GameManager _gameManager;
        [SerializeField] private int spawnNumber;
        [SerializeField] private GameObject[] customerPrefab;
        [SerializeField] private Transform[] spawnLocations;
        private void Start()
        {

            StartCoroutine(DelaySpaw());
        }
        private void SpawnCustomer()
        {
            _gameManager.UpdateCurrentShops();
            if (_gameManager.shops.Count == 0) return;
            spawnNumber = _gameManager.shops.Count + 3;
            if (spawnNumber > 15) spawnNumber = 15;
            if (GameObject.FindGameObjectsWithTag("Customer").Length<spawnNumber)
                Instantiate(customerPrefab[Random.Range(0,2)],spawnLocations[Random.Range(0,spawnLocations.Length)].transform.position, Quaternion.identity);
        }
    
        private IEnumerator DelaySpaw()
        {
            yield return new WaitForSeconds(1f);
            while (true)
            {
                yield return new WaitForSeconds(2.5f);
                SpawnCustomer();
            }
      
        }
    }
}
