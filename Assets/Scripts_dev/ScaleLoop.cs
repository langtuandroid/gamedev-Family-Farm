using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleLoop : MonoBehaviour
{
    public Transform target;
    public float scaleDuration = 1f;
    public float loopDelay = 0.5f;
    public float scaleFactor = 2f;
    Sequence sequence;
    Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        // Phóng to và thu nhỏ đối tượng target với DOTween
         sequence = DOTween.Sequence();
        sequence.Append(target.DOScale(scaleFactor, scaleDuration).SetEase(Ease.InOutQuad)); // Phóng to đối tượng
    
        sequence.Append(target.DOScale(1f, scaleDuration).SetEase(Ease.InOutQuad)); // Thu nhỏ đối tượng
        sequence.SetLoops(-1); // Lặp vô hạn
    }
    public void OnClick()
    {
       
            DOTween.KillAll();
        
    }
}
