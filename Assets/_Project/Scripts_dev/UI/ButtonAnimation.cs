using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts_dev.UI
{
    public class ButtonAnimation : MonoBehaviour
    {
        [Inject] private SoundManager _soundManager;
        public float scaleAmount = 0.8f;
        public float duration = 0.1f;

        public Vector3 originalScale=Vector3.one;
        public bool upgrade;
   
        public void OnButtonClick(Button button)
        {
            if(!upgrade)
                _soundManager.PlaySound(_soundManager.sounds[4]);
            else
                _soundManager.PlaySound(_soundManager.sounds[0]);

            button.transform.DOScale(originalScale * scaleAmount, duration).SetUpdate(true).OnComplete(()=> { ResetButtonScale(button); });
        }

        private void ResetButtonScale(Button btn)
        {
            btn.transform.DOScale(originalScale, duration).SetUpdate(true);
        }
    }
}
