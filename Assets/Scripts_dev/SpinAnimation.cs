using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpinAnimation : MonoBehaviour
{
    public float spinDuration = 1f;
    public Vector3 spinAxis = Vector3.up;
    public Ease spinEase = Ease.Linear;
    public bool isLooping = false;

    private void Start()
    {
        StartSpinAnimation();
    }
    private void OnEnable()
    {
        StartSpinAnimation();
    }

    private void StartSpinAnimation()
    {
        Tweener tweener = transform.DORotate(transform.rotation.eulerAngles + spinAxis * 360f, spinDuration, RotateMode.FastBeyond360)
            .SetEase(spinEase).SetUpdate(true);

        if (isLooping)
        {
            tweener.SetLoops(-1, LoopType.Restart);
        }
    }
}
