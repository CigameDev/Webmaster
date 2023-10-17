using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public static MusicController instance;

    [SerializeField] private AudioSource musicAus;
    [SerializeField] private AudioClip shopMusic;
    [SerializeField] private AudioClip moneyFly;
    [SerializeField] const  float volumeMusicDefault = 0.1f;
    private void Awake()
    {
        instance = this; 
    }

    private void PlayMusic(AudioClip clip,bool loop,float volume = volumeMusicDefault)
    {
        if (!DataPlayer.GetHasSound()) return;
        if(musicAus && clip)
        {
            musicAus.clip = clip;
            musicAus.loop = loop;
            musicAus.volume = volume;
            musicAus.Play();
        }    
    }    
    public void PlayShopMusic()
    {
        StopMusic();
        PlayMusic(shopMusic, true);
    }    
    public void PlayMoneyFly()
    {
        PlayMusic(moneyFly, false,1f);
    }    
    public void WaitPlayMoneyFly()
    {
        Invoke(nameof(PlayMoneyFly), 1.5f);
    }    
    public void StopMusic()
    {
        if (musicAus)
        {
            musicAus.Stop();
        }

    }
}
