using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LanguageManager : MonoBehaviour
{
    [SerializeField] TMP_FontAsset EN, ENOutline, VNI, VNIOutline;
    public static LanguageManager instance;

    public float language;
    public ExcelLanguage file;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        language = PlayerPrefs.GetFloat("language", 1);
    }
   

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetText(int index, TextMeshProUGUI text,bool outline=false)
    {
        Debug.Log(text.text);
        text.text = language==0? file.textLanguages[index].vni: file.textLanguages[index].eng;
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
        LaguageSetter[] setters = FindObjectsOfType<LaguageSetter>();

        foreach(LaguageSetter setter in setters)
        {
            setter.Set();
        }
    }
    public string GetText(int index)
    {
        return language == 0 ? file.textLanguages[index].vni : file.textLanguages[index].eng;
    }
}
