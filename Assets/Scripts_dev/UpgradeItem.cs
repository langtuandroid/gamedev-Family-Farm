using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeItem : MonoBehaviour
{
    public LevelMangament levelMangament;
    public TextMeshProUGUI title, price, level;
    public Image image;
    public Image btnIcon;
    public Button upgradeBtn;
    public Sprite money, locked;
    [SerializeField] GameObject bg;

    private void Update()
    {
        image.sprite = levelMangament.sprite;
        if (levelMangament == null) return;

        LanguageManager.instance.SetText(levelMangament.titleIndex,title);
        if (levelMangament.transform.gameObject.activeInHierarchy)
        {
            level.text = LanguageManager.instance.GetText(42)+": " + (levelMangament.level + 1).ToString();
            bg.SetActive(false);
            btnIcon.sprite = money;
            if (levelMangament.level < 2)
            {
                price.text = UIManager.instance.FormatNumber(levelMangament.prices[levelMangament.level]);
            }
            else
            {
                price.text = "Max";
            }
           
            if (levelMangament.level >= 2 || levelMangament.prices[levelMangament.level] > GameManager.instance.money)
            {
                upgradeBtn.interactable = false;
            }
            else
            {
                upgradeBtn.interactable = true;
            }
        }
     
       
      

       
        else
        {
            price.text = LanguageManager.instance.GetText(43);
            upgradeBtn.interactable = false;
            level.text = LanguageManager.instance.GetText(42) + ": 0";
            bg.SetActive(true);
            btnIcon.sprite = locked;

        }
    }
    public void Upgrade()
    {
        GameManager.instance.money -= levelMangament.prices[levelMangament.level];
        levelMangament.Upgrade();
    }
}
