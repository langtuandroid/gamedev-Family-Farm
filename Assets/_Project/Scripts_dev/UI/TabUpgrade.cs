using System.Collections.Generic;
using _Project.Scripts_dev.Items;
using _Project.Scripts_dev.Managers;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace _Project.Scripts_dev.UI
{
    public class TabUpgrade : MonoBehaviour
    {
        [Inject] private DiContainer _diContainer;
        [Inject] private GameManager _gameManager;
        [FormerlySerializedAs("levelMangaments")] [SerializeField] private List< LevelMangament> _levelManagers;
        [FormerlySerializedAs("itemPrefab")] [SerializeField] private GameObject _itemPrefab;
        [FormerlySerializedAs("contents")] [SerializeField] private Transform _content;
        public int type;

        private void Start()
        {
            _levelManagers = new List<LevelMangament>();
            LoadList();

        }
        private void OnEnable()
        {
            LoadList();
        }

        private void LoadList()
        {
            _levelManagers.Clear();
            foreach(Transform t in _content)
            {
                Destroy(t.gameObject);
            }
            foreach (GameObject l in _gameManager._unlockOrder)
            {
                if (l.GetComponent<LevelMangament>()!=null)
                    if (l.GetComponent<LevelMangament>().Type == type)
                        _levelManagers.Add(l.GetComponent<LevelMangament>());
            }
            if (type != 3)
                _levelManagers.Sort((a, b) => a._goods.Id.CompareTo(b._goods.Id));
            
            for(int i=0; i < _levelManagers.Count; i++)
            {
                GameObject item = _diContainer.InstantiatePrefab(_itemPrefab, _content);
                item.GetComponent<UpgradeItem>().LevelMangament = _levelManagers[i];
            }
        }
    }
}
