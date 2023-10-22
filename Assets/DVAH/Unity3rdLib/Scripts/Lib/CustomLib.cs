using DVAH;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomLib : MonoBehaviour
{
    AsyncOperation scene;

    private void Awake()
    {
        if(!DataPlayer.alldata.noAds)
        {
            RestorePurchased();//bất đồng bộ ,nếu user đã mua rồi nhưng xóa game và cài lại thì Noads = false ,thao tác này để trả NoAds về true và RemoveAds
        }    
    }

    void Start()
    {
        scene = SceneManager.LoadSceneAsync(GameConst.Game_Scene);
        //scene.allowSceneActivation = false;

        if (DataPlayer.alldata.noAds)
        {
            RemoveAds();
            LoadingManager.Instant.DoneConditionSelf(0, () => SceneManager.GetSceneByName(GameConst.Game_Scene).isLoaded);
            LoadingManager.Instant.SetMaxTimeLoading(30).Init(1,this.LoadingCompleteCallbackNoAds);
        }    
        else
        {
            LoadingManager.Instant.DoneConditionSelf(0, () => AdManager.Instant.AdsOpenIsLoaded());
            LoadingManager.Instant.DoneConditionSelf(1, () => AdManager.Instant.InterstitialIsLoaded());
            LoadingManager.Instant.SetMaxTimeLoading(10).Init(2, this.LoadingCompleteCallback);
        }

        FireBaseManager.Instant.LogEventWithParameterAsync("loading_start", new Hashtable()
        {
            {
                "id_screen","LOADING"
            }
        });
    }


    private void LoadingCompleteCallback(List<bool> list)
    {
        AdManager.Instant.InitializeBannerAdsAsync();
        AdManager.Instant.ShowAdOpen(0, true, (id, state) =>
        {
            if (state == OpenAdState.Closed)
            {
                Debug.Log("da tat ad");
            }
        });
        //scene.allowSceneActivation = true;
        DataFireBaseConfig.Instance.isLoaded = true;

        FireBaseManager.Instant.LogEventWithParameterAsync("loading_complete", new Hashtable()
        {
            {
                "id_screen","LOADING"
            }
        });

        FireBaseManager.Instant.LogEventWithParameterAsync("home_start", new Hashtable()
        {
            {
                 "id_screen","HOME"
            }
        });
    }

    private void LoadingCompleteCallbackNoAds(List<bool> list)
    {
        //scene.allowSceneActivation = true;
        DataFireBaseConfig.Instance.isLoaded = true;

        FireBaseManager.Instant.LogEventWithParameterAsync("loading_complete", new Hashtable()
        {
            {
                "id_screen","LOADING"
            }
        });

        FireBaseManager.Instant.LogEventWithParameterAsync("home_start", new Hashtable()
        {
            {
                 "id_screen","HOME"
            }
        });
    }
    private void RestorePurchased()
    {
         _ = IAPManager.Instant.TryAddRestoreEvent("hero.vip.noads", () =>
         {
             DataPlayer.SetNoAds(true);
             RemoveAds();
             Debug.Log("ProcessPurchase___Done");
         }, true);

    }
    private void RemoveAds()
    {
        AdManager.Instant.setOffAdPosition(true, AD_TYPE.open, AD_TYPE.resume, AD_TYPE.banner, AD_TYPE.inter);
        DataPlayer.SetNoAds(true);
        AdManager.Instant.DestroyBanner();
    }
}
