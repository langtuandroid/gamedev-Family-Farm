using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource music;
    public AudioSource sound;
    public AudioClip[] sounds;
    public static SoundManager instance;
    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
       
            music.volume = PlayerPrefs.GetFloat("music", 1)==1?0.5f:0;
            sound.volume = PlayerPrefs.GetFloat("sound", 1);
        
    }
    public void PlaySound(AudioClip clip)
    {
        sound.clip = clip;
        sound.Play();
    }
    public void CreateSound(AudioClip clip,Vector3 position,float volume=1)
    {
        GameObject go = new GameObject();
        AudioSource audio= go.AddComponent<AudioSource>();
        audio.clip = clip;
        audio.playOnAwake = true;
        audio.loop =false;
        audio.volume = sound.volume*volume;
        go.name = clip.name;

        Destroy (Instantiate(go,position,Quaternion.identity),2);
        
         Destroy(go, clip.length);
        
    }
}
