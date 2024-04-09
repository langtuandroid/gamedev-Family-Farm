using System.Collections;
using System.Collections.Generic;
using _Project.Scripts_dev.AI;
using _Project.Scripts_dev.Configs;
using _Project.Scripts_dev.Control;
using _Project.Scripts_dev.Managers;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace _Project.Scripts_dev.Classes
{
    public class Shelf : MonoBehaviour
    {
        [Inject] private DataManager _dataManager;
        [Inject] private GameManager _gameManager;
        public int Quantity { get; set; }
        [FormerlySerializedAs("productNeeded")] public int _productsRequierment;
        [FormerlySerializedAs("productToShow")] public Goods _productsToDisplay;
        [FormerlySerializedAs("pos")] [SerializeField] private GameObject[] _positions;
        [FormerlySerializedAs("matPlace")] [SerializeField] private MaterialPlace _materialsPlace;
        private bool _isLoad;

        private void Start()
        {
            foreach (GameObject go in _positions)
            {
                GameObject clone = Instantiate(_productsToDisplay.ItemPrefab, go.transform);
                clone.transform.position = go.transform.position;
                clone.SetActive(false);
            }
        }
        private void Update()
        {
            if (_dataManager.GameData != null && !_isLoad)
            {
                GameData gameData = _dataManager.GameData;
                int[] productNumbers = gameData.productNumbers.ToArray();
                _isLoad = true;
                if (_gameManager.PlayTime < 3) 
                    Quantity = _productsToDisplay.Id > productNumbers.Length? 0 : productNumbers[_productsToDisplay.Id - 1];
            }
            if (_dataManager.GameData == null)
            {
                _isLoad = true;
            }
            if (!_isLoad) return;
            UpdateStockView();
        }
        private void UpdateStockView()
        {
            StartCoroutine(ActiveRoutine());
            if (Quantity < _positions.Length)
                StartCoroutine(DeactivateRoutine());

        }
        private IEnumerator ActiveRoutine()
        {
            for (int i = 0; i < (Quantity > _positions.Length ? _positions.Length : Quantity); i++)
            {
                _positions[i].transform.GetChild(0).gameObject.SetActive(true);
                yield return new WaitForSeconds(0.05f);
            }
        }
        private IEnumerator DeactivateRoutine()
        {
            for (int i = Quantity; i < _positions.Length; i++)
            {
                _positions[i].transform.GetChild(0).gameObject.SetActive(false);
                yield return new WaitForSeconds(0.05f);
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Shipper") || other.CompareTag("Player"))
            {
                string tag = other.gameObject.tag;
                Cart cart = tag == "Shipper" ? other.GetComponent<CharactersAI>().Cart : other.GetComponent<PlayerControl>().Cart;
           
                if (CheckCartRequierment(cart))
                {
                    cart.Remove(transform.name,_productsRequierment, _materialsPlace, 10000, true, this);
                }
            }
        }

        private bool CheckCartRequierment(Cart cart)
        {
            List<GameObject> list = cart.cart; 
            foreach (GameObject go in list)
            {
                if (go.GetComponent<Product>().Goods.Id == _productsRequierment)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
