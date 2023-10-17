using DVAH;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : MonoBehaviour
{
    [SerializeField] Button backButton;
    [SerializeField] Button privacyButton;
    [SerializeField] Button sfxButton;
    [SerializeField] Button vibrationButton;
    [SerializeField] Sprite toggleOff;
    [SerializeField] Sprite toggleOn;

    private bool isSound;
    private bool isVibration;
    private void OnEnable()
    {

        isSound = DataPlayer.GetHasSound();
        isVibration = DataPlayer.GetHasVibration();
        if(isSound )
        {
            sfxButton.GetComponent<Image>().sprite = toggleOn;
        }
        else
        {
            vibrationButton.GetComponent <Image>().sprite = toggleOff;
        }   
        
        if( isVibration )
        {
            vibrationButton.GetComponent<Image>().sprite = toggleOn;
        }    
        else
        {
            vibrationButton.GetComponent <Image>().sprite = toggleOff;
        }


        FireBaseManager.Instant.LogEventWithParameterAsync("setting_start", new Hashtable()
        {
            {
                "id_screen","SETTING"
            }
        });
    }
    private void Start()
    {
        backButton.onClick.AddListener(() =>
        {
            this.gameObject.SetActive(false);
            SoundFXManager.Instance.PlayClickButton();
            GameManager.Instance.TapVibrate();


            FireBaseManager.Instant.LogEventWithParameterAsync("btn_home", new Hashtable()
            {
                {
                    "id_screen","SETTING"
                }
            });
        });

        privacyButton.onClick.AddListener(() =>
        {
            SoundFXManager.Instance.PlayClickButton();
            GameManager.Instance.TapVibrate();
        });
        sfxButton.onClick.AddListener(() =>
        {
            SoundFXManager.Instance.PlayClickButton();
            GameManager.Instance.TapVibrate();
            if ( isSound )
            {
                sfxButton.GetComponent<Image>().sprite = toggleOff;
                isSound = false;
                DataPlayer.SetHasSound(isSound);
                MusicController.instance.StopMusic();
            }else
            {
                sfxButton.GetComponent<Image>().sprite= toggleOn;
                isSound = true;
                DataPlayer.SetHasSound(isSound);
            }

            FireBaseManager.Instant.LogEventWithParameterAsync("setting_btn_sfx", new Hashtable()
            {
                {
                   "id_screen","SETTING"
                }
            });
        });
        vibrationButton.onClick.AddListener(() =>
        {
            SoundFXManager.Instance.PlayClickButton();
            GameManager.Instance.TapVibrate();
            if (isVibration)
            {
                vibrationButton.GetComponent<Image>().sprite = toggleOff;
                isVibration = false;
            }    
            else
            {
                vibrationButton.GetComponent<Image>().sprite = toggleOn;
                isVibration = true;
            }    
            DataPlayer.SetHasVibration(isVibration);

            FireBaseManager.Instant.LogEventWithParameterAsync("setting_btn_vibration", new Hashtable()
            {
                {
                    "id_screen","SETTING"
                }
            });
        });
    }
}
