using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public static class SoundManager
{
    public enum Sound {
        SnakeMove,
        SnakeDie,
        SnakeEat,
        ButtonClick,
        ButtonOver
    }


    public static void PlaySound(Sound sound){
            GameObject soundGameObject = new GameObject("Sound");
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.PlayOneShot(GetAudioClip(sound));

    }
    
    private static AudioClip GetAudioClip(Sound sound){
        foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets.i.soundAudioClipArray) {
            if (soundAudioClip.sound == sound){
                return soundAudioClip.audioClip;
            }
        }    
        Debug.LogError("Sound " + sound + " not found!");
        return null;
    }

    //extending a previously written class without changing the code in the class
    public static void AddButtonSounds(this Button_UI buttonUI){
        buttonUI.MouseOverOnceFunc += () => SoundManager.PlaySound(Sound.ButtonOver);
        buttonUI.ClickFunc += () => SoundManager.PlaySound(Sound.ButtonClick);
    }
}
