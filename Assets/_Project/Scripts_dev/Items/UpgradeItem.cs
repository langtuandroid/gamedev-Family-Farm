
using _Project.Scripts_dev.Managers;
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
            _imageText.sprite = LevelMangament._sprite;
            if (LevelMangament == null) return;

            _titleText.text = LevelMangament._goods.Name; 
            if (LevelMangament.transform.gameObject.activeInHierarchy)
            {
                _levelText.text = "Level: " + (LevelMangament.Level + 1).ToString();
                _backgroundObject.SetActive(false);
                _buttonIcon.sprite = _money;
                if (LevelMangament.Level < 2)
                {
                    _priceText.text = _uiManager.FormatNumber(LevelMangament._prices[LevelMangament.Level]);
                }
                else
                {
                    _priceText.text = "Max";
                }
           
                if (LevelMangament.Level >= 2 || LevelMangament._prices[LevelMangament.Level] > _gameManager.Money)
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
                _priceText.text = "Locked";
                _upgradeButton.interactable = false;
                _levelText.text = "Level: 0";
                _backgroundObject.SetActive(true);
                _buttonIcon.sprite = _lockedSprite;

            }
        }
        public void Upgrade()
        {
            _gameManager.Money -= LevelMangament._prices[LevelMangament.Level];
            LevelMangament.Upgrade();
        }
    }
}
