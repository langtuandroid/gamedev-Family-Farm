using _Project.Scripts_dev.Language;
using _Project.Scripts_dev.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts_dev.Items
{
    public class UpgradeItem : MonoBehaviour
    {
        [Inject] private LanguageManager _languageManager;
        [Inject] private GameManager _gameManager;
        [Inject] private UIManager _uiManager;
        [FormerlySerializedAs("bg")] [SerializeField] private GameObject _backgroundObject;
        [FormerlySerializedAs("title")] [SerializeField] private TextMeshProUGUI _titleText;
        [FormerlySerializedAs("price")] [SerializeField] private TextMeshProUGUI _priceText;
        [FormerlySerializedAs("level")] [SerializeField] private TextMeshProUGUI _levelText;
        [FormerlySerializedAs("image")] [SerializeField] private Image _imageText;
        [FormerlySerializedAs("btnIcon")] [SerializeField] private Image _buttonIcon;
        [FormerlySerializedAs("upgradeBtn")] [SerializeField] private Button _upgradeButton;
        [FormerlySerializedAs("money")] [SerializeField] private Sprite _money;
        [FormerlySerializedAs("locked")] [SerializeField] private Sprite _lockedSprite;
        public LevelMangament LevelMangament { get; set; }
        private void Update()
        {
            _imageText.sprite = LevelMangament.sprite;
            if (LevelMangament == null) return;

            _languageManager.SetText(LevelMangament.titleIndex, _titleText);
            if (LevelMangament.transform.gameObject.activeInHierarchy)
            {
                _levelText.text = _languageManager.GetText(42)+": " + (LevelMangament.level + 1).ToString();
                _backgroundObject.SetActive(false);
                _buttonIcon.sprite = _money;
                if (LevelMangament.level < 2)
                {
                    _priceText.text = _uiManager.FormatNumber(LevelMangament.prices[LevelMangament.level]);
                }
                else
                {
                    _priceText.text = "Max";
                }
           
                if (LevelMangament.level >= 2 || LevelMangament.prices[LevelMangament.level] > _gameManager.money)
                {
                    _upgradeButton.interactable = false;
                }
                else
                {
                    _upgradeButton.interactable = true;
                }
            }
            else
            {
                _priceText.text = _languageManager.GetText(43);
                _upgradeButton.interactable = false;
                _levelText.text = _languageManager.GetText(42) + ": 0";
                _backgroundObject.SetActive(true);
                _buttonIcon.sprite = _lockedSprite;

            }
        }
        public void Upgrade()
        {
            _gameManager.money -= LevelMangament.prices[LevelMangament.level];
            LevelMangament.Upgrade();
        }
    }
}
