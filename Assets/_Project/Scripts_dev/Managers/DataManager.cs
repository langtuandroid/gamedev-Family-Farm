using UnityEngine;
using Zenject;

namespace _Project.Scripts_dev.Managers
{
    public class DataManager : MonoBehaviour
    {
        [Inject] private GameManager _gameManager;
        public GameData gameData;
    
        private void Start()
        {
            gameData = LoadGameData();
        }

        public void SaveGameData()
        {
            GameData gameData = new GameData(_gameManager);
            string gameDataJson = JsonUtility.ToJson(gameData);
            PlayerPrefs.SetString("GameData", gameDataJson);
            PlayerPrefs.Save();
        
        }
        public GameData LoadGameData()
        {
            if (PlayerPrefs.HasKey("GameData"))
            {
                string savedGameDataJson = PlayerPrefs.GetString("GameData");
                GameData savedGameData = JsonUtility.FromJson<GameData>(savedGameDataJson);
                return savedGameData;
            }
            else
            {
                return null;
            }
        }
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SaveGameData();
            }
        }
        private void OnApplicationQuit()
        {
            SaveGameData();
        }
    }
}