using TMPro;
using UnityEngine;

namespace _Project.Scripts_dev.Language
{
    public class LaguageSetter : MonoBehaviour
    {
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
            LanguageManager.instance.SetText(index, text,outline);
        }
    }
}
