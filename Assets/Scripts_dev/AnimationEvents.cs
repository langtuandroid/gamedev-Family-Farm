using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{

   public void Plant()
    {
        if (transform.name == "Player Variant") 
            SoundManager.instance.CreateSound(SoundManager.instance.sounds[6], transform.position);
        
    }
    public void RunTrail()
    {
        Instantiate(EffectManager.instance.dustTrail,transform).transform.localPosition =new Vector3(0,0,-0.04f);
    }
}
