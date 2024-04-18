using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts_dev.UI
{
    public class Loading : MonoBehaviour
    {
        [SerializeField] private GameObject _startMenu;
        [SerializeField] private GameObject _optionsMenu;

        private void Start()
        {
            AudioListener.volume = PlayerPrefs.GetInt("Audio", 1);
        }

        public void Play()
        {
            SceneManager.LoadSceneAsync(1);
        }

        public void OpenSettings(bool isOpen)
        {
            _startMenu.SetActive(!isOpen);
            _optionsMenu.SetActive(isOpen);
        }
        
        private IEnumerator WaitRoutine()
        {
            yield return new WaitForSeconds(2);
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
        }
    }
}
