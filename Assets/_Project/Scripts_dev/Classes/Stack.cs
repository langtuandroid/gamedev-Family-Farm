using _Project.Scripts_dev.Configs;
using _Project.Scripts_dev.Managers;
using UnityEngine;
using Zenject;

namespace _Project.Scripts_dev.Classes
{
    public class Stack : MonoBehaviour
    {
        [Inject] private DataManager _dataManager;
        [Inject] private GameManager _gameManager;
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
            if (_dataManager.gameData != null&&!load)
            {
                GameData gameData = _dataManager.gameData;
                int[] stackNumbers = gameData.stackNumbers.ToArray();
                load = true;
                if(_gameManager.playTime<3)
                    currentQuantity = productToShow.id > stackNumbers.Length?0: stackNumbers[productToShow.id - 1];
           
            }
            if (_dataManager.gameData == null)
            {
                load = true;
            }
            UpdateStockLooks();
            if(destroy)
            {
                if (currentQuantity <= 0) Destroy(gameObject);
            }
        }
        private void UpdateStockLooks()
        {

            DelayActive();
            if(currentQuantity<pos.Length)
                DelayDeative();

        }
        private void DelayActive()
        {
            for (int i = 0; i < (currentQuantity > pos.Length ? pos.Length : currentQuantity); i++)
            {
                pos[i].transform.GetChild(0).gameObject.SetActive(true);
            }
        } 
        private void DelayDeative()
        {
            for (int i = currentQuantity; i < pos.Length; i++)
            {
                pos[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
}
