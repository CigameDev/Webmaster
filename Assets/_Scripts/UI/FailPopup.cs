using DVAH;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FailPopup : MonoBehaviour
{
    [SerializeField] Button reviveButton;
    [SerializeField] Button retryButton;
    [SerializeField] TextMeshProUGUI countTime;


    private int intCountTime = 5;
    private bool playerDieFirstTime = true;

   
    private void OnEnable()
    {
        SoundFXManager.Instance.PlayFail();


        countTime.text = intCountTime.ToString();
        reviveButton.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(false);
        StartCoroutine(CountDownTime());

        if (playerDieFirstTime)
        {
            reviveButton.gameObject.SetActive(true);
            retryButton.gameObject.SetActive(false);
            playerDieFirstTime=false;
        }
        else
        {
            reviveButton.gameObject.SetActive(false);
            retryButton.gameObject.SetActive(true);
            countTime.gameObject.SetActive(false);
        }

        FireBaseManager.Instant.LogEventWithParameterAsync("fail_start", new Hashtable()
        {
            {
                "id_screen","FAIL"
            },
            {
                "id_level",DataPlayer.GetLevelValue()
            }
        });
    }
    private void Start()
    {
         
        reviveButton.onClick.AddListener(OnclickReviveButton);

        retryButton.onClick.AddListener(OnClickRetryButton);
    }
   
    private IEnumerator CountDownTime()
    {
        int time = intCountTime;
        while(time > 0)
        {
            //yield return new WaitForSeconds(1);
            yield return new WaitForSecondsRealtime(1);
            time--;
            countTime.text = time.ToString();
        }    
        reviveButton.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(true);
    }    
    private void OnclickReviveButton()
    {
        SoundFXManager.Instance.PlayClickButton();
        GameManager.Instance.TapVibrate();
        //xem quang cao sau do hoi sinh nhan vat
        StartCoroutine(ShowRewardAd(Callback_Revive));

        FireBaseManager.Instant.LogEventWithParameterAsync("fail_btn_revive", new Hashtable()
        {
            {
                "id_screen","FAIL"
            },
            {
                "id_level",DataPlayer.GetLevelValue()
            }
        });
    }

    private IEnumerator ShowRewardAd(Action<RewardVideoState> callback)
    {
        yield return null;
        AdManager.Instant.ShowRewardVideo(callback, true);
    }

    private void Callback_Revive(RewardVideoState state)
    {
        if (state == RewardVideoState.Watched)
        {
            GameManager.Instance.ChangeTimeScale(1f);//reset lai timeScalse
            GameEvent.instance.PostEvent_OnRevive();
            GameManager.Instance.IsCompleteLevel = false;
            playerDieFirstTime = false;
            this.gameObject.SetActive(false);
        }

       
    }
    private void OnClickRetryButton()
    {
        SoundFXManager.Instance.PlayClickButton();
        GameManager.Instance.TapVibrate();

        if (DataFireBaseConfig.Instance.CanShowInterLose())
        {
            DataFireBaseConfig.Instance.ShowInterLose();
        }
        else
        {
            SceneManager.LoadScene(GameConst.Game_Scene);
        }

        FireBaseManager.Instant.LogEventWithParameterAsync("fail_btn_retry", new Hashtable()
        {
            {
                "id_screen","FAIL"
            },
            {
                "id_level",DataPlayer.GetLevelValue()
            }
        });
    }
    
}
