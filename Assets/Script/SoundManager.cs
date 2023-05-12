using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoundManager : MonoBehaviour
{   
    public Sound[] bgmSounds, effectSounds;
    public AudioSource bgmSource, effectSource;

    public static SoundManager instance;

    private void Awake(){
        if (instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }
    
    void Start(){
        PlayBGM();
    }

    public void PlayBGM(float volume = 0.3f){
        Sound sound = bgmSounds[0];

        if(sound == null){
            Debug.Log("BGM Sound Error");
        }
        else{
            bgmSource.clip = sound.clip;
            bgmSource.volume = volume;
            bgmSource.Play();
        }
    }

    public void PlayEffect(string name){
        Sound sound = Array.Find(effectSounds, e => e.soundName == name);

        if(sound == null){
            Debug.Log($"Sound Error - name : {name}");
        }
        else{
            effectSource.volume = GameManager.instance.userData.effectVolume;
            effectSource.PlayOneShot(sound.clip);
        }
    }

    public void ChangeBGMVolume(float volume){
        bgmSource.volume = volume;
    }

}
