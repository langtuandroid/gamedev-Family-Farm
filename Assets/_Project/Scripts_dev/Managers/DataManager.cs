using UnityEngine;
using Zenject;

namespace _Project.Scripts_dev.Managers
{
    public class DataManager : MonoBehaviour
    {
        [Inject] private GameManager _gameManager;
        public GameData GameData { get; private set; }
    
        private void Awake()
        {
            GameData = LoadData();
        }

        private void SaveData()
        {
            GameData gameData = new GameData(_gameManager);
            string gameDataJson = JsonUtility.ToJson(gameData);
            PlayerPrefs.SetString("GameData", gameDataJson);
            PlayerPrefs.Save();
        }

        private GameData LoadData()
        {
            if (!PlayerPrefs.HasKey("GameData"))
            {
                return null;
            }
            string savedGameDataJson = PlayerPrefs.GetString("GameData");
            GameData savedGameData = JsonUtility.FromJson<GameData>(savedGameDataJson);
            return savedGameData;
        }
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SaveData();
            }
        }
        private void OnApplicationQuit()
        {
            SaveData();
        }
    }
}