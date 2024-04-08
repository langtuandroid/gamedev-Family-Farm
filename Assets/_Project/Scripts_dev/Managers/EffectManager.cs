using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts_dev;
using _Project.Scripts_dev.Animation;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class EffectManager : MonoBehaviour
{
    [Inject] private GameManager _gameManager;
    public GameObject rainbowUnlock;
    public GameObject star;
    public GameObject starEXPBar; 
    public GameObject money;
    public GameObject MoneyBar;
    
    private void MoveFromWorkSpaceToUI(Transform startPos, RectTransform targetUI, GameObject obj, TweenCallback complete)
    {
        Vector3 startScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, startPos.position);
        float speed = 2000f;
        obj.transform.position = new Vector3(startScreenPos.x, startScreenPos.y,0);
        ParabolicMovement(obj, startScreenPos, targetUI.position, UnityEngine.Random.Range(1, 1.5f), UnityEngine.Random.Range(-200f, 200f), complete);
    }
    public void GetExpEffect(float exp, Transform startPos)
    {
        GameObject go= Instantiate(star,GameObject.FindGameObjectWithTag("MainUI").transform);
        go.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        MoveFromWorkSpaceToUI(startPos, starEXPBar.GetComponent<RectTransform>(), go,()=> {
            if (_gameManager.draggable)
                SoundManager.instance.CreateSound(SoundManager.instance.sounds[9], transform.position, 0.5f);
            StartCoroutine(EXPanim(exp));
            go.transform.GetChild(1).GetComponent<Image>().enabled = false;
            DotweenCodes.instance.ScaleBig(starEXPBar, 1.2f, 0.1f);
        });
    }
    public void GetMoneyEffect(float money, Transform startPos)
    {
        GameObject go= Instantiate(this.money,GameObject.FindGameObjectWithTag("MainUI2").transform);
        go.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        MoveFromWorkSpaceToUI(startPos, MoneyBar.GetComponent<RectTransform>(), go,()=> {
            if (_gameManager.draggable)
                SoundManager.instance.CreateSound(SoundManager.instance.sounds[5], transform.position, 0.5f);
           
            _gameManager.money += money;
            go.transform.GetChild(1).GetComponent<Image>().enabled = false;
           
        });
    }
    private IEnumerator EXPanim(float exp)
    {
        float part = exp / 20;
        while(exp>0)
        {
            yield return null;
            _gameManager.currentExp += part>exp?exp:part;
            exp -= part;
        }
    }
    private void ParabolicMovement(GameObject go,Vector3 start, Vector3 targetPosition, float duration, float height, TweenCallback OnComplete)
    {
        Vector3[] path = new Vector3[3];
        path[0] = start;
        Vector3 tempPos = (Vector2) start + UnityEngine.Random.insideUnitCircle * 250;
        path[1] = tempPos;
        path[2] = targetPosition;

        go.GetComponent<RectTransform>().DOPath(path, duration, PathType.CatmullRom, PathMode.Sidescroller2D, 10, Color.red)
            .SetEase(Ease.InQuad)
            .OnComplete(OnComplete);
    }
}
