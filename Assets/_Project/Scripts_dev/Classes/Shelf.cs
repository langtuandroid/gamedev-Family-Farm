using System.Collections;
using System.Collections.Generic;
using _Project.Scripts_dev.AI;
using _Project.Scripts_dev.Configs;
using _Project.Scripts_dev.Control;
using _Project.Scripts_dev.Managers;
using UnityEngine;
using Zenject;

namespace _Project.Scripts_dev.Classes
{
    public class Shelf : MonoBehaviour
    {
        [Inject] private DataManager _dataManager;
        [Inject] private GameManager _gameManager;
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
            if (_dataManager.gameData != null && !load)
            {
                GameData gameData = _dataManager.gameData;
                int[] productNumbers = gameData.productNumbers.ToArray();
                load = true;
                if (_gameManager.playTime < 3) 
                    currentQuantity = productToShow.id > productNumbers.Length? 0 : productNumbers[productToShow.id - 1];
            }
            if (_dataManager.gameData == null)
            {
                load = true;
            }
            if (!load) return;
            UpdateStockLooks();
        }
        private void UpdateStockLooks()
        {

            StartCoroutine(DelayActive());
            if (currentQuantity < pos.Length)
                StartCoroutine(DelayDeative());

        }
        private IEnumerator DelayActive()
        {
            for (int i = 0; i < (currentQuantity > pos.Length ? pos.Length : currentQuantity); i++)
            {
                pos[i].transform.GetChild(0).gameObject.SetActive(true);
                yield return new WaitForSeconds(0.05f);

            }
        }
        private IEnumerator DelayDeative()
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
                Cart cart = tag == "Shipper" ? other.GetComponent<CharactersAI>().Cart : other.GetComponent<PlayerControl>().cart;
           
                if (CheckCart(cart))
                {
                    cart.Remove(transform.name,productNeeded, matPlace, 10000, true, this);
                }
            }
        }

        private bool CheckCart(Cart cart)
        {
            List<GameObject> list = cart.cart; 
            foreach (GameObject go in list)
            {
                if (go.GetComponent<Product>().goods.id == productNeeded)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
