using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts_dev.Additional
{
    public class InactiveTimer : MonoBehaviour
    {
        [Inject] private GameManager _gameManager;
        public Image image,bg;
        public TextMeshProUGUI text;
        public Image adIcon;

        public float time;
        public bool play;

        private void Start()
        {
            _gameManager.playTimer = 0;
            _gameManager.inactiveTimer = 0;
            image.fillAmount = 1f;
            _gameManager.interTimer = 0;
            adIcon.gameObject.SetActive(false);
            bg.gameObject.SetActive(false);
            image.gameObject.SetActive(false);
            text.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!play)
            {
                if (_gameManager.inactiveTimer >= time - 5 && _gameManager.draggable)
                {
                    _gameManager.playTimer =time;
                    bg.gameObject.SetActive(true);
                    image.gameObject.SetActive(true);
                    text.gameObject.SetActive(true);
                    adIcon.gameObject.SetActive(true);
                    image.fillAmount = 1 - (_gameManager.inactiveTimer - (time - 5)) / 5;
                    text.text = ((int)(time - _gameManager.inactiveTimer) + 1).ToString();
                }
                else
                {
                    image.fillAmount = 1f;

                    adIcon.gameObject.SetActive(false);

                    bg.gameObject.SetActive(false);
                    image.gameObject.SetActive(false);
                    text.gameObject.SetActive(false);


                }
            }
        
            else
            {
                if (_gameManager.playTimer >= time - 5&& (_gameManager.draggable))
                {
                    bg.gameObject.SetActive(true);
                    image.gameObject.SetActive(true);
                    text.gameObject.SetActive(true);
                    adIcon.gameObject.SetActive(true);
                    image.fillAmount = 1 - (_gameManager.playTimer - (time - 5)) / 5;
                    text.text = ((int)(time - _gameManager.playTimer) + 1).ToString();
                }
                else
                {
                    image.fillAmount = 1f;

                    adIcon.gameObject.SetActive(false);

                    bg.gameObject.SetActive(false);
                    image.gameObject.SetActive(false);
                    text.gameObject.SetActive(false);


                }
            }
            if (image.fillAmount <= 0f)
            {
                _gameManager.playTimer = 0;
                _gameManager.inactiveTimer = 0;
                image.fillAmount = 1f;
                _gameManager.interTimer = 0;
                adIcon.gameObject.SetActive(false);
                bg.gameObject.SetActive(false);
                image.gameObject.SetActive(false);
                text.gameObject.SetActive(false);

            }
        }
    }
}
