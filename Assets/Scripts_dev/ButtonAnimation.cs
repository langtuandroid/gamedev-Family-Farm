using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAnimation : MonoBehaviour
{
    public float scaleAmount = 0.8f;
    public float duration = 0.1f;

    public Vector3 originalScale=Vector3.one;
    public bool upgrade;
   
    public void OnButtonClick(Button button)
    {
        if(!upgrade)
        SoundManager.instance.PlaySound(SoundManager.instance.sounds[4]);
        else
            SoundManager.instance.PlaySound(SoundManager.instance.sounds[0]);

        button.transform.DOScale(originalScale * scaleAmount, duration).SetUpdate(true).OnComplete(()=> { ResetButtonScale(button); });
    }

    private void ResetButtonScale(Button btn)
    {
        btn.transform.DOScale(originalScale, duration).SetUpdate(true);
    }
}
