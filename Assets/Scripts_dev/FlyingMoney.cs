/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FlyingMoney : MonoBehaviour
{
     [SerializeField] Transform target;
     public float moneyPercoin;
    public Transform ic_moneyTemp;
     private void OnEnable()
     {*/
        /*  target =ic_moneyTemp;
          int count = transform.childCount;
          for (int i = 0; i < transform.childCount; i++)
          {

               Vector3 tempPos = (Vector2)transform.GetChild(i).position + Random.insideUnitCircle * 10;

               //Vector3 tempPos = transform.GetChild(i).position - Random.insideUnitCircle * 10;

               Vector3[] path = new Vector3[2];
               path[0] = tempPos;
               path[1] = target.position;
               transform.GetChild(i).DOPath(path, Random.Range(0.5f, 1f), PathType.CatmullRom, PathMode.Sidescroller2D).SetEase(Ease.Linear).OnComplete(() =>
               {
                    SoundManager.instance.PlaySound(8);
                   ic_money.GetChild(0).DOScale(1f, 0f);
                    ic_money.GetChild(0).DOScale(1.25f, 0.5f).OnComplete(() =>
                    {

                         UIManager.instance.ic_money.GetChild(0).DOScale(1f, 1f);
                    });
                    GameManager.instance.UpdateMoney(moneyPercoin);
                    count--;
                    if (count == 0) Destroy(gameObject);
               });*/
         // }


          // for (int i = 0; i < transform.childCount; i++)
          // {
          //      transform.GetChild(i).DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetDelay(delay);

          //      transform.GetChild(i).DOMove(target.position, 0.3f).SetDelay(delay + 0.5f);
          //      transform.GetChild(i).DOScale(0f, 0.3f).SetDelay(delay + 1f).SetEase(Ease.OutBack).OnComplete(() =>
          //      {
          //           count--;
          //           if (count == 0) Destroy(gameObject);
          //      });

          //      delay += 0.1f;
          // }

