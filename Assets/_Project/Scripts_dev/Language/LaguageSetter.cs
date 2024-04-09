using TMPro;
using UnityEngine;
using Zenject;

namespace _Project.Scripts_dev.Language
{
    public class LaguageSetter : MonoBehaviour
    {
        [Inject] private LanguageManager _languageManager;
        public bool notUi;
        public bool outline;
        public int index;
        private TextMeshProUGUI text;
        private void Awake()
        {
            if (!notUi)
                text = GetComponent<TextMeshProUGUI>();
        }
        private void Start()
        {
            Set();
        }
        private void OnEnable()
        {
            Set();
        }
        public void Set()
        {
            _languageManager.SetText(index, text,outline);
        }
    }
}
