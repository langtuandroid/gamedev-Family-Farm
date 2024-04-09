using TMPro;
using UnityEngine;

namespace _Project.Scripts_dev.Language
{
    public class LanguageManager : MonoBehaviour
    {
        [SerializeField] TMP_FontAsset EN, ENOutline, VNI, VNIOutline;

        public float language;
        public ExcelLanguage file;
        private void Awake()
        {
            language = PlayerPrefs.GetFloat("language", 1);
        }
    
        public void SetText(int index, TextMeshProUGUI text,bool outline=false)
        {
            text.text = language==0? file.textLanguages[index].VniVariant: file.textLanguages[index].EnglishVariant;
            if (outline)
            {
                text.font = language == 0 ? VNIOutline :ENOutline;
            }
            else
            {
                text.font = language == 0 ? VNI : EN;
            }
       
        }
        public void SetTextAll()
        {
            LanguageSetter[] setters = FindObjectsOfType<LanguageSetter>();

            foreach(LanguageSetter setter in setters)
            {
                setter.SetText();
            }
        }
        public string GetText(int index)
        {
            return language == 0 ? file.textLanguages[index].VniVariant : file.textLanguages[index].EnglishVariant;
        }
    }
}
