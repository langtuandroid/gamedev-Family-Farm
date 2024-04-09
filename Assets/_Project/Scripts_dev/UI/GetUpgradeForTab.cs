using System.Collections.Generic;
using _Project.Scripts_dev.Items;
using UnityEngine;
using Zenject;

namespace _Project.Scripts_dev.UI
{
    public class GetUpgradeForTab : MonoBehaviour
    {
        [Inject] private GameManager _gameManager;
        public List< LevelMangament> levelMangaments;
        public GameObject itemPrefab;
        public Transform contents;
        public int type;

        private void Start()
        {
            levelMangaments = new List<LevelMangament>();
            UpdateList();

        }
        private void OnEnable()
        {
            UpdateList();
        }

        private void UpdateList()
        {
            levelMangaments.Clear();
            foreach(Transform t in contents)
            {
                Destroy(t.gameObject);
            }
            foreach (GameObject l in _gameManager.unlockOrder)
            {
                if (l.GetComponent<LevelMangament>()!=null)
                    if (l.GetComponent<LevelMangament>().type == type)
                        levelMangaments.Add(l.GetComponent<LevelMangament>());
            }
            if (type != 3)
                levelMangaments.Sort((a, b) => a.goods.Id.CompareTo(b.goods.Id));
            
            for(int i=0; i < levelMangaments.Count; i++)
            {
                GameObject item = Instantiate(itemPrefab, contents);
                item.GetComponent<UpgradeItem>().LevelMangament = levelMangaments[i];
            }
        }
    }
}
