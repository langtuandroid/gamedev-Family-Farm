using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts_dev.Managers
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _musicAudioSource;
        [SerializeField] private AudioSource _soundAudioSource;
        [FormerlySerializedAs("sounds")] public AudioClip[] Clips;
        public AudioSource Sound => _soundAudioSource;

        private void Start()
        {
            AudioListener.volume = PlayerPrefs.GetInt("Audio", 1);
        }

        private void Update()
        {
            _musicAudioSource.volume = PlayerPrefs.GetFloat("music", 1)==1?0.5f:0;
            _soundAudioSource.volume = PlayerPrefs.GetFloat("sound", 1);
        }
        public void PlaySound(AudioClip clip)
        {
            _soundAudioSource.clip = clip;
            _soundAudioSource.Play();
        }
        public void CreateSound(AudioClip clip,Vector3 position,float volume=1)
        {
            GameObject go = new GameObject();
            AudioSource audio= go.AddComponent<AudioSource>();
            audio.clip = clip;
            audio.playOnAwake = true;
            audio.loop =false;
            audio.volume = _soundAudioSource.volume*volume;
            go.name = clip.name;
            Destroy (Instantiate(go,position,Quaternion.identity),2);
            Destroy(go, clip.length);
        }
    }
}
