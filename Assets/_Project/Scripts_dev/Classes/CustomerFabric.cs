using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace _Project.Scripts_dev.Classes
{
    public class CustomerFabric : MonoBehaviour
    {
        [Inject] private DiContainer _diContainer;
        [Inject] private GameManager _gameManager;
        private int _spawnNumber;
        [FormerlySerializedAs("customerPrefab")] [SerializeField] private GameObject[] _customerPrefabs;
        [FormerlySerializedAs("spawnLocations")] [SerializeField] private Transform[] _spawnPositions;
        private void Start()
        {
            StartCoroutine(DelaySpawnRoutine());
        }
        private void SpawnCustomer()
        {
            _gameManager.UpdateCurrentShops();
            if (_gameManager.shops.Count == 0) return;
            _spawnNumber = _gameManager.shops.Count + 3;
            if (_spawnNumber > 15) _spawnNumber = 15;
            if (GameObject.FindGameObjectsWithTag("Customer").Length<_spawnNumber)
                _diContainer.InstantiatePrefab(_customerPrefabs[Random.Range(0,2)],_spawnPositions[Random.Range(0,_spawnPositions.Length)].transform.position, Quaternion.identity, null);
        }
    
        private IEnumerator DelaySpawnRoutine()
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
