using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public class SpinRewardControler : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI prizeText1, prizeText2, prizeText3, prizeText4, prizeText5, prizeText6, prizeText7, prizeText8;
    TextMeshProUGUI moneyText, expText;
    [SerializeField] GameObject money, exp;
    [SerializeField] GameObject moneyPos, expPos;
    [SerializeField] Button freeButton, adsButton;
    private float price;
    private float maxExp;
    private GameManager gameManager;

    public bool first;
    private void Start()
    {
         gameManager = GameManager.instance;
        moneyText = UIManager.instance.moneyText;
        expText = UIManager.instance.expText;
        if (first) FirstPrize();
        
     
    }

    private void Update()
    {
       
       
    }
    private void OnDisable()
    {
        if (first)
        {
            GameManager.instance.UpdateUnlocked();
            UIManager.instance.BottomUI.SetActive(true);
          
        }
    }
    private void OnEnable()
    {
        UpdatePrize();
       
    }
    void UpdatePrize()
    {
        price = GameManager.instance.unlockOrder[GameManager.instance.currentUnlocked].transform.GetChild(1).GetComponent<Unlock>().price;
        maxExp = GameManager.instance.maxExp;
        prizeText1.text = "+" + UIManager.instance.FormatNumber(Mathf.Ceil(maxExp / 10));
        prizeText2.text = "+" +UIManager.instance.FormatNumber(Mathf.Ceil(price / 5));
        prizeText3.text = "+" + UIManager.instance.FormatNumber(Mathf.Ceil(maxExp *0.3f));
        prizeText4.text = "+" + UIManager.instance.FormatNumber(Mathf.Ceil(price / 2));
        prizeText5.text = "+" + UIManager.instance.FormatNumber(Mathf.Ceil(maxExp / 2));
        prizeText6.text = LanguageManager.instance.GetText(55)+" + 150s";
        prizeText7.text = LanguageManager.instance.GetText(56) + " +150s";
        prizeText8.text = LanguageManager.instance.GetText(34);

    }
    void FirstPrize()
    {
        prizeText1.text = "+" + UIManager.instance.FormatNumber(Mathf.Ceil(maxExp / 10));
        prizeText2.text = "+" + UIManager.instance.FormatNumber(Mathf.Ceil(price / 2)); ;
        prizeText3.text = "+" + UIManager.instance.FormatNumber(Mathf.Ceil(maxExp * 0.3f));
        prizeText4.text = "+150";
        prizeText5.text = "+" + UIManager.instance.FormatNumber(Mathf.Ceil(maxExp / 2));
        prizeText6.text = LanguageManager.instance.GetText(55) + " +150s";
        prizeText7.text = LanguageManager.instance.GetText(56) + " +150s";
        prizeText8.text = LanguageManager.instance.GetText(34);
    }
    
    public Action GetPrice(float prize)
    {
        Action prizeAction;
       

        switch (prize)
        {
            case 1: prizeAction = () => StartCoroutine(MakePrize(exp,expPos.GetComponent<RectTransform>(),5,gameManager.maxExp/10,1)); break;
            case 2: prizeAction = () => StartCoroutine(MakePrize(money,moneyPos.GetComponent<RectTransform>(),10,price/5,0)); break;
            case 3: prizeAction = () => StartCoroutine(MakePrize(exp, expPos.GetComponent<RectTransform>(),15,gameManager.maxExp*0.3f,1)); break;
            case 4: prizeAction = () => StartCoroutine(MakePrize(money, moneyPos.GetComponent<RectTransform>(), 20, price / 2, 0)); break;
            case 5: prizeAction = () => StartCoroutine(MakePrize(exp, expPos.GetComponent<RectTransform>(), 25, gameManager.maxExp / 2, 1)); break;
            case 6: prizeAction = () => gameManager.incomeBoostTime = 150; break;
            case 7: prizeAction = () => gameManager.speedBoostTime = 150; break;
            case 8: prizeAction = () => GameManager.instance.freeSpinTimer = 0; break;
            default: prizeAction = () => GameManager.instance.freeSpinTimer = 0; break;
        }
        return prizeAction;
    }public Action GetFirstPrice(float prize)
    {
        Action prizeAction;
       

        switch (prize)
        {
            case 1: prizeAction = () => StartCoroutine(MakePrize(exp,expPos.GetComponent<RectTransform>(),5,gameManager.maxExp/10,1)); break;
            case 2: prizeAction = () => StartCoroutine(MakePrize(money,moneyPos.GetComponent<RectTransform>(),10,120,0)); break;
            case 3: prizeAction = () => StartCoroutine(MakePrize(exp, expPos.GetComponent<RectTransform>(),15,gameManager.maxExp*0.3f,1)); break;
            case 4: prizeAction = () => StartCoroutine(MakePrize(money, moneyPos.GetComponent<RectTransform>(), 20, 150, 0)); break;
            case 5: prizeAction = () => StartCoroutine(MakePrize(exp, expPos.GetComponent<RectTransform>(), 25, gameManager.maxExp / 2, 1)); break;
            case 6: prizeAction = () => gameManager.incomeBoostTime = 150; break;
            case 7: prizeAction = () => gameManager.speedBoostTime = 150; break;
            case 8: prizeAction = () => GameManager.instance.freeSpinTimer = 0; break;
            default: prizeAction = () => GameManager.instance.freeSpinTimer = 0; break;
        }
        return prizeAction;
    }
    

    IEnumerator MakePrize(GameObject prefab,RectTransform destination, int times, float amount,int type)
    {
        float temp = amount;
        
        for (int i=0; i < times; i++)
        {
            yield return null;
            GameObject go = Instantiate(prefab, transform.parent);
           
            go.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().position+new Vector3(0,350,0);
            ParabolicMovement(go, go.transform.position, destination.position, UnityEngine.Random.Range(1, 1.5f), UnityEngine.Random.Range(-200f, 200f), () => {
                Destroy(go);
                if(type==1)
                DotweenCodes.instance.ScaleBig(expPos, 1.2f, 0.1f);
                if (type == 1)
                    gameManager.currentExp += amount / times;
                if (type == 0)
                    gameManager.money += amount / times;
                int r = UnityEngine.Random.Range(0, 2);
                if (r == 0 && type == 0)
                    SoundManager.instance.CreateSound(SoundManager.instance.sounds[5], transform.position, 0.5f);
                else if (r == 0)
                {
                    SoundManager.instance.CreateSound(SoundManager.instance.sounds[9], transform.position, 0.5f);
                }
            });
         /*   go.GetComponent<RectTransform>().DOMove(destination.position, 1f)
         .SetEase(Ease.InQuad).OnComplete(() => {
             Destroy(go);
             if(type==1)
             gameManager.currentExp += amount / times;
             if(type==0)
                 gameManager.money += amount / times;
         });*/
           
        }
    }
    void ParabolicMovement(GameObject go, Vector3 start, Vector3 targetPosition, float duration, float height, TweenCallback OnComplete)
    {
        Vector3[] path = new Vector3[3];
        path[0] = start;
        Vector3 tempPos = (Vector2) start + UnityEngine.Random.insideUnitCircle * 300;
        //path[1] = (targetPosition + start) / 2 + new Vector3(height, 0, 0);
        path[1] = tempPos;
        path[2] = targetPosition;

        go.GetComponent<RectTransform>().DOPath(path, duration, PathType.CatmullRom, PathMode.Sidescroller2D, 10, Color.red)
            .SetEase(Ease.InQuad)
            .OnComplete(OnComplete);

    }


}
