using System.Collections;
using System.Collections.Generic;
using _Project.Scripts_dev.Classes;
using _Project.Scripts_dev.Configs;
using _Project.Scripts_dev.Managers;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class LevelMangament : MonoBehaviour
{
    [Inject] private DataManager _dataManager;
    [Inject] private GameManager _gameManager;
    
    [SerializeField] private GameObject[] unlocks;
    public int Level { get; private set; }
    [FormerlySerializedAs("goods")] public Goods _goods;
    [FormerlySerializedAs("multiple")] public float _multiply;
    [FormerlySerializedAs("prices")] public List<float> _prices;
    [FormerlySerializedAs("titleIndex")] public int _titleId;
    [FormerlySerializedAs("type")] public int Type;
    [FormerlySerializedAs("sprite")] public Sprite _sprite;
    
    private bool _load;
    private void Start()
    {
        _prices = new List<float>();
        _prices.Add(_goods.Income * 100);
        _prices.Add(_prices[0]*6);
        foreach(GameObject go in unlocks)
        {
            go.SetActive(false);
        }
    }
    private void Update()
    {
        if(!_load)
        {
            if (_dataManager.GameData != null && _gameManager.PlayTime<3)
            {
                GameData gameData = _dataManager.GameData;
                if (transform.GetChild(0).gameObject.activeInHierarchy)
                {
                    _load = true; 
                    Level = _goods.Id >gameData.levels.Count?0: gameData.levels[_goods.Id - 1];
                }
               
        
            }
            else
            {
                _load = true;
            }
            return;

        }
        if (unlocks.Length > 0)
        {
            for (int i = 0; i < Level; i++)
            {
                unlocks[i].SetActive(true);
            }
        }

    }
    public void Upgrade()
    {
        Level++;
    }
}
