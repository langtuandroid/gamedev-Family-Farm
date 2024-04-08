using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    [SerializeField] RectTransform border;
    public bool noMoney;
    private bool imPoor;
    private void Start()
    {
        
    }
    private void Update()
    {
        if (GameManager.instance.money <= 0&&!noMoney&&!imPoor)
        {
            DOTween.Kill(border, true);
            border.DOScale(Vector2.one, 0.1f);
            imPoor = true;
        }
        if (GameManager.instance.money > 0) imPoor = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!noMoney) {
            if (other.CompareTag("Player") && !DOTween.IsTweening(border))
            {
                if (GameManager.instance.money > 0)
                {
                
                    border.DOScale(1.2f * Vector2.one, 0.15f).OnComplete(() =>
                    {
                        border.DOScale(Vector2.one *1f, 0.15f);
                    }).SetLoops(-1, LoopType.Restart);
                }
                else
                {
                    border.DOScale(1.2f * Vector2.one, 0.15f);
                }
            }

           
        }
        
       

    }
    private void OnTriggerStay(Collider other)
    {
        if(noMoney)
        {
            if ((other.CompareTag("Player")/* || other.CompareTag("Customer") || other.CompareTag("Farmer") || other.CompareTag("Shipper"))*/) && !DOTween.IsTweening(border))
            {
                border.DOScale(1.2f * Vector2.one, 0.15f);

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.CompareTag("Player")/*|| other.CompareTag("Customer") || other.CompareTag("Farmer") || other.CompareTag("Shipper")*/))
        {
            DOTween.Kill(border, true);
            border.DOScale(Vector2.one, 0.15f);
        }
    }
}
