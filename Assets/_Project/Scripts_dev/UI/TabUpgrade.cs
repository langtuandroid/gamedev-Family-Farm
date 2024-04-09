using System.Collections.Generic;
using _Project.Scripts_dev.Items;
using _Project.Scripts_dev.Managers;
using UnityEngine;
using Zenject;

namespace _Project.Scripts_dev.UI
{
    public class TabUpgrade : MonoBehaviour
    {
        [Inject] private GameManager _gameManager;
        public List< LevelMangament> levelMangaments;
        public GameObject itemPrefab;
        public Transform contents;
        public int type;

        private void Start()
        {
            levelMangaments = new List<LevelMangament>();
            LoadList();

        }
        private void OnEnable()
        {
            LoadList();
        }

        private void LoadList()
        {
            levelMangaments.Clear();
            foreach(Transform t in contents)
            {
                Destroy(t.gameObject);
            }
            foreach (GameObject l in _gameManager._unlockOrder)
            {
                if (l.GetComponent<LevelMangament>()!=null)
                    if (l.GetComponent<LevelMangament>().Type == type)
                        levelMangaments.Add(l.GetComponent<LevelMangament>());
            }
            if (type != 3)
                levelMangaments.Sort((a, b) => a._goods.Id.CompareTo(b._goods.Id));
            
            for(int i=0; i < levelMangaments.Count; i++)
            {
                GameObject item = Instantiate(itemPrefab, contents);
                item.GetComponent<UpgradeItem>().LevelMangament = levelMangaments[i];
            }
        }
    }
}
