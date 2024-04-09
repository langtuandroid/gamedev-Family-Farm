using _Project.Scripts_dev.Managers;
using UnityEngine;
using Zenject;

namespace _Project.Scripts_dev.Control
{
    public class AnimationEvents : MonoBehaviour
    {
        [Inject] private SoundManager _soundManager;
        [Inject] private EffectManager _effectManager;
        
        public void Plant()
        {
            if (transform.name == "Player Variant") 
                _soundManager.CreateSound(_soundManager.Clips[6], transform.position);
        }
        public void RunTrail()
        {
            Instantiate(_effectManager._dustTrailPrefab, transform).transform.localPosition =new Vector3(0,0,-0.04f);
        }
    }
}
