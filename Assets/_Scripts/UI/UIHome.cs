using DVAH;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIHome : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Button settingButton;
    [SerializeField] Button skinButton;
    [SerializeField] Button noAdsButton;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI monneyText;
    [SerializeField] GameObject greenPointPrefab, yellowPointPrefab, whitePointPrefab;
    [SerializeField] Transform parentPoint;

    [SerializeField] Button selectButton;

    private void OnEnable()
    {
        if (DataPlayer.alldata.noAds)
        {
            noAdsButton.gameObject.SetActive(false);
        }
        levelText.text = "LEVEL " + DataPlayer.GetLevelValue();
        monneyText.text =  DataPlayer.GetMonneyValue().ToString();
        SpawnPointProgress(DataPlayer.GetLevelValue());

        FireBaseManager.Instant.LogEventWithParameterAsync("home_start", new Hashtable()
        {
            {
                "id_screen","HOME"
            }
        });
    }
    private void Start()
    {
        GameEvent.instance.OnGetMonney += RegisterEvent_OnChangeMonney;
        startButton.onClick.AddListener(() =>
        {
            GameManager.Instance.conditionDetectPlayer = true;
            GameManager.Instance.StartWaitReady();
            this.gameObject.SetActive(false);
            UIController.Instance.ShowIngamePopup();
            SoundFXManager.Instance.PlayClickButton();
            GameManager.Instance.TapVibrate();
            GameManager.Instance.startTimeLevel = Time.unscaledTime;
            FireBaseManager.Instant.LogEventWithParameterAsync("home_btn_start", new Hashtable()
            {
                {
                    "id_screen","HOME"
                }
            });
        });
        settingButton.onClick.AddListener(() =>
        {
            UIController.Instance.ShowSettingPopup();
            SoundFXManager.Instance.PlayClickButton();
            GameManager.Instance.TapVibrate();


            FireBaseManager.Instant.LogEventWithParameterAsync("home_btn_setting", new Hashtable()
            {
                {
                    "id_screen","HOME"
                }
            });
        });
        skinButton.onClick.AddListener(() =>
        {
            UIController.Instance.ShowSkinPopup();
            SoundFXManager.Instance.PlayClickButton();
            GameManager.Instance.TapVibrate();


            FireBaseManager.Instant.LogEventWithParameterAsync("home_btn_collection", new Hashtable()
            {
                {
                    "id_screen","HOME"
                }
            });
        });
        noAdsButton.onClick.AddListener(() =>
        {
            SoundFXManager.Instance.PlayClickButton();
            GameManager.Instance.TapVibrate();

            IAPManager.Instant.BuyProductID("hero.vip.noads", (isSuccess) =>
            {
                if (isSuccess)
                {
                    Debug.Log("Buy DOne!");
                    RemoveAds();
                }
                else
                {
                    Debug.Log("Buy Fail!");
                }
            });

            FireBaseManager.Instant.LogEventWithParameterAsync("in_app_purchase_click", new Hashtable()
            {
                {
                    "id_screen","HOME"
                },
                {
                    "package_name","hero.vip.noads"
                }
            });
        });

        selectButton.onClick.AddListener(() =>
        {
            UIController.Instance.DisplayLevelSelect();
        });

    }

    private void RegisterEvent_OnChangeMonney(int monney)
    {
        monneyText.text = monney.ToString();
    }

    private void OnDestroy()
    {
        GameEvent.instance.OnGetMonney -= RegisterEvent_OnChangeMonney;
    }

    private void SpawnPointProgress(int level)
    {
        int progress = level % 9;
        if (progress == 0) progress = 9;
        for(int i =0;i<9;i++)
        {
            if(i+1 <progress)
            {
                Instantiate(greenPointPrefab, parentPoint);
            }   
            else if(i +1 == progress)
            {
                Instantiate(yellowPointPrefab, parentPoint);
            }   
            else
            {
                Instantiate(whitePointPrefab, parentPoint);
            }    
        }
    }    

    private void RemoveAds()
    {
        DataPlayer.SetNoAds(true);
        AdManager.Instant.DestroyBanner();
        noAdsButton.gameObject.SetActive(false);
        AdManager.Instant.setOffAdPosition( true, AD_TYPE.open, AD_TYPE.resume, AD_TYPE.banner,AD_TYPE.inter);
    }    
}
