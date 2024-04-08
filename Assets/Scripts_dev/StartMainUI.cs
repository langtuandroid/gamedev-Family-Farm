using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StartMainUI : MonoBehaviour
{
   

    List<Vector2> oldPos;
    public RectTransform[] rects;
    [SerializeField] float speed;
  

    // Start is called before the first frame update
    void Start()
    {
        oldPos = new List<Vector2>() ;
      
        SetUp();
        Step1(0.2f);
        Step2(0.4f);
        Step3(0.6f);
        Step4(0.8f);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SetUp()
    {
        for(int i = 0; i < rects.Length; i++)
        {
            oldPos.Add(rects[i].position);
        }
        rects[0].anchoredPosition = new Vector2(rects[0].anchoredPosition.x - 350, rects[0].anchoredPosition.y);
         rects[1].anchoredPosition = new Vector2(rects[1].anchoredPosition.x + 350, rects[1].anchoredPosition.y);
         rects[2].anchoredPosition = new Vector2(rects[2].anchoredPosition.x + 350, rects[2].anchoredPosition.y);
         rects[3].anchoredPosition = new Vector2(rects[3].anchoredPosition.x + 350, rects[3].anchoredPosition.y);
        rects[4].anchoredPosition = new Vector2(rects[4].anchoredPosition.x - 350, rects[4].anchoredPosition.y);
        rects[5].anchoredPosition = new Vector2(rects[5].anchoredPosition.x + 350, rects[5].anchoredPosition.y);
        rects[6].anchoredPosition = new Vector2(rects[6].anchoredPosition.x + 350, rects[6].anchoredPosition.y);
        rects[7].anchoredPosition = new Vector2(rects[7].anchoredPosition.x, rects[7].anchoredPosition.y+350);
        rects[8].anchoredPosition = new Vector2(rects[8].anchoredPosition.x - 350, rects[8].anchoredPosition.y);
    }
    void Step1(float delay)
    {

        rects[0].DOMove(oldPos[0]+new Vector2(20,0), speed).SetEase(Ease.OutQuad).SetDelay(delay).OnComplete(()=> {
            rects[0].DOMove(oldPos[0], speed/2).SetEase(Ease.InQuad);
        }); 
        rects[1].DOMove(oldPos[1] - new Vector2(20, 0), speed).SetEase(Ease.OutQuad).SetDelay(delay).OnComplete(() => {
            rects[1].DOMove(oldPos[1], speed/2).SetEase(Ease.InQuad);
        }); ;
    }
    void Step2(float delay)
    {
        rects[2].DOMove(oldPos[2] - new Vector2(20, 0), speed).SetEase(Ease.OutQuad).SetDelay(delay).OnComplete(() => {
            rects[2].DOMove(oldPos[2], speed/2).SetEase(Ease.InQuad);
        }); 
    }
    void Step3(float delay)
    {
        rects[3].DOMove(oldPos[3] - new Vector2(20, 0), speed).SetEase(Ease.OutQuad).SetDelay(delay).OnComplete(() => {
            rects[3].DOMove(oldPos[3], speed/2).SetEase(Ease.InQuad);
        });
    }
    void Step4(float delay)
    {
        rects[4].DOMove(oldPos[4] + new Vector2(20, 0), speed).SetEase(Ease.OutFlash).SetDelay(delay).OnComplete(() => {
            rects[4].DOMove(oldPos[4], speed/2).SetEase(Ease.InQuad);
        });
        rects[5].DOMove(oldPos[5] - new Vector2(20, 0), speed).SetEase(Ease.OutQuad).SetDelay(delay).OnComplete(() => {
            rects[5].DOMove(oldPos[5], speed/2).SetEase(Ease.InQuad);
        });
        rects[6].DOMove(oldPos[6] - new Vector2(20, 0), speed).SetEase(Ease.OutQuad).SetDelay(delay).OnComplete(() => {
            rects[6].DOMove(oldPos[6], speed/2).SetEase(Ease.InQuad);
        });
        rects[7].DOMove(oldPos[7] - new Vector2(0, 20), speed).SetEase(Ease.OutQuad).SetDelay(delay).OnComplete(() => {
            rects[7].DOMove(oldPos[7], speed/2).SetEase(Ease.InQuad);

        });
        rects[8].DOMove(oldPos[8] + new Vector2(20, 0), speed).SetEase(Ease.OutQuad).SetDelay(delay).OnComplete(() => {
            rects[8].DOMove(oldPos[8], speed / 2).SetEase(Ease.InQuad);
        });
    }

}
