using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MusicType
{
    main_0 =0,
    main_1 =1,
    main_2 =2,
}
public class MusicManager : MonoBehaviour
{

    public static MusicManager Instance;
    private AudioSource musicFx;
    private void Awake()
    {
        if (Instance == null)
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
        if (musicFx == null)
        {
            musicFx = gameObject.AddComponent<AudioSource>();
            musicFx.playOnAwake = false;
        }
    }

    public void OnPlayMusic(MusicType musicType)
    {
        if (!DataPlayer.GetHasSound()) return;
        var audio = Resources.Load<AudioClip>($"Musics/{musicType.ToString()}");
        musicFx.clip = audio;
        musicFx.loop = true;
        musicFx.Play();
    }    
}
