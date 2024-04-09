using _Project.Scripts_dev.Managers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts_dev.UI
{
    public class ButtonAnim : MonoBehaviour
    {
        [Inject] private SoundManager _soundManager;
        private readonly float _scale = 0.8f;
        private readonly float _duration = 0.1f;
        [FormerlySerializedAs("originalScale")] [SerializeField] private Vector3 _startScale = Vector3.one;
        [FormerlySerializedAs("upgrade")] [SerializeField] private bool IsUpgrade;
   
        public void OnButtonClick(Button button)
        {
            if(!IsUpgrade)
                _soundManager.PlaySound(_soundManager.Clips[4]);
            else
                _soundManager.PlaySound(_soundManager.Clips[0]);

            button.transform.DOScale(_startScale * _scale, _duration).SetUpdate(true).OnComplete(()=> { ResetButtonScale(button); });
        }

        private void ResetButtonScale(Button btn)
        {
            btn.transform.DOScale(_startScale, _duration).SetUpdate(true);
        }
    }
}
