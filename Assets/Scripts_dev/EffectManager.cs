using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;

    public GameObject dustTrail;
    public GameObject rainbowUnlock;
    public GameObject star;
    public GameObject starEXPBar; 
    public GameObject money;
    public GameObject MoneyBar;
    public GameObject noInternetText;
    public Transform effectUI;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void MoveFromWorkSpaceToUI(Transform startPos, RectTransform targetUI, GameObject obj, TweenCallback complete)
    {

      
        Vector3 startScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, startPos.position);

        Vector3 targetScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, targetUI.position);
        float travelDistance = Vector3.Distance(startScreenPos, targetScreenPos);
        float speed = 2000f;
        obj.transform.position = new Vector3(startScreenPos.x, startScreenPos.y,0);
        ParabolicMovement(obj, startScreenPos, targetUI.position, UnityEngine.Random.Range(1, 1.5f), UnityEngine.Random.Range(-200f, 200f), complete);
       // obj.GetComponent<RectTransform>().DOMove(targetUI.position, travelDistance/speed).SetEase(Ease.InQuad).OnComplete(()=>complete());
    }
    public void GetExpEffect(float exp, Transform startPos)
    {
       
        GameObject go= Instantiate(star,GameObject.FindGameObjectWithTag("MainUI").transform);
       
        // go.transform.localScale = Vector3.one * 0.5f;
        go.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        MoveFromWorkSpaceToUI(startPos, starEXPBar.GetComponent<RectTransform>(), go,()=> {
            if (GameManager.instance.draggable)
                SoundManager.instance.CreateSound(SoundManager.instance.sounds[9], transform.position, 0.5f);
            StartCoroutine(EXPanim(exp));
           // GameManager.instance.currentExp += exp;
            go.transform.GetChild(1).GetComponent<Image>().enabled = false;
            DotweenCodes.instance.ScaleBig(starEXPBar, 1.2f, 0.1f);
        });
    }public void GetMoneyEffect(float money, Transform startPos)
    {
       
        GameObject go= Instantiate(this.money,GameObject.FindGameObjectWithTag("MainUI2").transform);
       
        // go.transform.localScale = Vector3.one * 0.5f;
        go.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        MoveFromWorkSpaceToUI(startPos, MoneyBar.GetComponent<RectTransform>(), go,()=> {
            if (GameManager.instance.draggable)
                SoundManager.instance.CreateSound(SoundManager.instance.sounds[5], transform.position, 0.5f);
           
           GameManager.instance.money += money;
            go.transform.GetChild(1).GetComponent<Image>().enabled = false;
           
        });
    }
    IEnumerator EXPanim(float exp)
    {
        float part = exp / 20;
        while(exp>0)
        {
            yield return null;
            GameManager.instance.currentExp += part>exp?exp:part;
            exp -= part;
        }
    }
    void ParabolicMovement(GameObject go,Vector3 start, Vector3 targetPosition, float duration, float height, TweenCallback OnComplete)
    {
        Vector3[] path = new Vector3[3];
        path[0] = start;
        Vector3 tempPos = (Vector2) start + UnityEngine.Random.insideUnitCircle * 250;
        //path[1] = (targetPosition + start) / 2 + new Vector3(height, 0, 0);
        path[1] = tempPos;
        path[2] = targetPosition;

        go.GetComponent<RectTransform>().DOPath(path, duration, PathType.CatmullRom, PathMode.Sidescroller2D, 10, Color.red)
            .SetEase(Ease.InQuad)
            .OnComplete(OnComplete);
       
    }
    public void NoInternet()
    {
        Instantiate(noInternetText, effectUI).GetComponent<RectTransform>().anchoredPosition =Vector2.zero;
    }
}
