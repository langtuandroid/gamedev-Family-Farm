using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace _Project.Scripts_dev.Language
{
    public class LanguageSetter : MonoBehaviour
    {
        [Inject] private LanguageManager _languageManager;
        [FormerlySerializedAs("outline")] [SerializeField] private bool _isOutline;
        [FormerlySerializedAs("index")] [SerializeField] private int _index;
        private TextMeshProUGUI _text;
        private bool _isNotUI;
        private void Awake()
        {
            if (!_isNotUI)
                _text = GetComponent<TextMeshProUGUI>();
        }
        private void Start()
        {
            SetText();
        }
        private void OnEnable()
        {
            SetText();
        }
        public void SetText()
        {
            _languageManager.SetText(_index, _text,_isOutline);
        }
    }
}
