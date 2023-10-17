using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;

    [SerializeField] private AudioSource sfxAus;
    [SerializeField] AudioClip shoot;//ban sung
    [SerializeField] AudioClip shield;
    [SerializeField] AudioClip woodenBox;
    [SerializeField] AudioClip gate;
    [SerializeField] AudioClip enemyAttack;
    [SerializeField] AudioClip fail;
    [SerializeField] AudioClip playerDie;
    [SerializeField] AudioClip doubleKill;
    [SerializeField] AudioClip tripleKill;
    [SerializeField] AudioClip killStreak;
    [SerializeField] AudioClip woodenBoxBroken;
    [SerializeField] AudioClip spiderweb;
    [SerializeField] AudioClip boom;
    [SerializeField] AudioClip ball;
    [SerializeField] AudioClip win;
    [SerializeField] AudioClip clickButton;

    [Range(0f, 1f)]
    [SerializeField] private float volumeDefault = 1f;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void PlayShoot()
    {
        if (!DataPlayer.GetHasSound()) return;
        sfxAus.PlayOneShot(shoot, volumeDefault);
    }

    public void PlayShield()
    {
        if (!DataPlayer.GetHasSound()) return;
        sfxAus.PlayOneShot(shield, volumeDefault);
    }

    public void PlayWoodenBox()
    {
        if (!DataPlayer.GetHasSound()) return;
        sfxAus.PlayOneShot(woodenBox, volumeDefault);
    }

    public void PlayGate()
    {
        if (!DataPlayer.GetHasSound()) return;
        sfxAus.PlayOneShot(gate, volumeDefault);
    }

    public void PlayEnemyAttack()
    {
        if (!DataPlayer.GetHasSound()) return;
        sfxAus.PlayOneShot(enemyAttack, volumeDefault);
    }

    public void PlayFail()
    {
        if (!DataPlayer.GetHasSound()) return;
        sfxAus.PlayOneShot(fail, volumeDefault);
    }

    public void PlayPlayerDie()
    {
        if (!DataPlayer.GetHasSound()) return;
        sfxAus.PlayOneShot(playerDie, volumeDefault);
    }

    public void PlayWoodenBoxBroken()
    {
        if (!DataPlayer.GetHasSound()) return;
        sfxAus.PlayOneShot(woodenBoxBroken, volumeDefault/2f);
    }

    public void PlaySpiderWeb()
    {
        if (!DataPlayer.GetHasSound()) return;
        sfxAus.PlayOneShot(spiderweb, volumeDefault);
    }

    public void PlayBoom()
    {
        if (!DataPlayer.GetHasSound()) return;
        sfxAus.PlayOneShot(boom, volumeDefault);
    }

    public void PlayBall()
    {
        if (!DataPlayer.GetHasSound()) return;
        sfxAus.PlayOneShot(ball, volumeDefault);
    }

    public void PlayWin()
    {
        if (!DataPlayer.GetHasSound()) return;
        sfxAus.PlayOneShot(win, volumeDefault);
    }

    public void PlayClickButton()
    {
        if (!DataPlayer.GetHasSound()) return;
        sfxAus.PlayOneShot(clickButton, volumeDefault);
    }

    public void DoubleKill()
    {
        if (!DataPlayer.GetHasSound()) return;
        sfxAus.PlayOneShot(doubleKill, volumeDefault);
    }
    public void TripleKill()
    {
        if (!DataPlayer.GetHasSound()) return;
        sfxAus.PlayOneShot(tripleKill, volumeDefault);
    }

    public void KillStreak()
    {
        if (!DataPlayer.GetHasSound()) return;
        sfxAus.PlayOneShot(killStreak, volumeDefault);
    }

    public void PlayKillEnemy(int quantity)
    {
        if(quantity ==1)
        {
            PlayEnemyAttack();
        }   
        else if(quantity ==2) 
        { 
            DoubleKill();
        }
        else if(quantity ==3)
        {
            TripleKill();
        }
        else if(quantity >=4)
        {
            KillStreak();
        }    

    }    
}
