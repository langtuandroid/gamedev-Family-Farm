using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LaguageSetter : MonoBehaviour
{
    public bool notUi,outline;
    public int index;
    TextMeshProUGUI text;
    TextMeshPro worldText;
    private void Awake()
    {
        if (!notUi)
            text = GetComponent<TextMeshProUGUI>();
        else worldText = GetComponent<TextMeshPro>();
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
