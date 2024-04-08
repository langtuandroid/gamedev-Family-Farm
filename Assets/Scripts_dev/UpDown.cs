using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpDown : MonoBehaviour
{
     RectTransform uiObject;
    public float movementDistance = 100f;
    public float movementDuration = 1f;

    private void Start()
    {
        uiObject = GetComponent<RectTransform>();
        // Tạo hiệu ứng loop di chuyển lên xuống
        Sequence sequence = DOTween.Sequence();
        sequence.Append(uiObject.DOAnchorPosY(uiObject.anchoredPosition.y + movementDistance, movementDuration).SetEase(Ease.OutQuad));
        sequence.Append(uiObject.DOAnchorPosY(uiObject.anchoredPosition.y, movementDuration).SetEase(Ease.OutQuad));
        sequence.SetLoops(-1); // Loop vô hạn
    }
    private void Update()
    {
        
    }
}
