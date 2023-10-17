using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    button =0,
    match =1,
}
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    private  AudioSource audioFx;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }    
        else
        {
            Destroy(this);
        }    
    }
    private void OnValidate()
    {
        if(audioFx == null)
        {
            audioFx = gameObject.AddComponent<AudioSource>();
            audioFx.playOnAwake = false;
        }    
    }

    public void OnPlaySound(SoundType soundType)
    {
        if (!DataPlayer.GetHasSound()) return;
        var audio = Resources.Load<AudioClip>($"Sounds/{soundType.ToString()}");
        audioFx.PlayOneShot(audio);
    }    
}
