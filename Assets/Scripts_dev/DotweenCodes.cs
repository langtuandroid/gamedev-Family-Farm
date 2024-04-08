using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DotweenCodes : MonoBehaviour
{
    public static DotweenCodes instance;
    private void Awake()
    {
        instance = this;
    }
    public void PopOut(Transform targetTransform,Vector3 maxSize,float duration)
    {
        targetTransform.localScale =  Vector3.one;
        // Mở rộng scale của đối tượng
        targetTransform.DOScale(maxSize, duration*0.7f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // Thu nhỏ lại đối tượng
                targetTransform.DOScale(Vector3.one, duration * 0.7f)
                    ;
            });
        Vector3 originalPosition = targetTransform.position;

        // Nảy lên
        targetTransform.DOJump(originalPosition + Vector3.up * 0.5f, 1, 1, duration*1.5f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // Rơi lại chỗ cũ
                
            });
        targetTransform.DOMove(originalPosition, duration).SetDelay(duration*1.2f).OnComplete(()=> {
           
        });
                    ;
    }
    public void ScaleBig(GameObject go,float amount, float duration)
    {
        Vector3 originalScale = go.transform.localScale;
        go.transform.DOScale(Vector3.one * amount, duration).SetEase(Ease.OutQuad).OnComplete(()=>{ go.transform.DOScale(Vector3.one , duration).SetEase(Ease.OutQuad); });
    }
}
