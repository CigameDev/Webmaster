using DVAH;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataFireBaseConfig : MonoBehaviour
{
    public static DataFireBaseConfig Instance;
    public int interAdDistanceBetweenLevels { get; set; } = 1;
    public List<int> replayCountRequiredForInterAd { get; set; } = new List<int>();

    private List<int> ratingShowAfterCompleteLevel  = new List<int>();//sau khi complete level này thì sẽ hiện rating

    private const string DistanceLevel_Config = "DistanceLevelShowInterWin";
    private const string ReplayCount_Config = "ReplayCountShowLoseInter";
    private const string RatingShowAfterLevel_Config = "LevelShowRate";

    private string ReplayCount = "";//= "1,3,5,7";
    private string ShowRateAfter_CompleteLevel = "";//"3,5,10,15,18";

    public bool isLoaded = false;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(Instance);
        }
        Application.targetFrameRate = 120;
    }

    private void Start()
    {
        GetDataConfig();
    }

   
    private void GetDataConfig()
    {
        FireBaseManager.Instant.GetValueRemoteAsync(DistanceLevel_Config, (value) =>
        {
            this.interAdDistanceBetweenLevels = (int)value.DoubleValue;
        });

        FireBaseManager.Instant.GetValueRemoteAsync(ReplayCount_Config, (value) =>
        {
            this.ReplayCount = (string)value.StringValue;
            replayCountRequiredForInterAd = GetNumber(ReplayCount);
        });
        

        FireBaseManager.Instant.GetValueRemoteAsync(RatingShowAfterLevel_Config, (value) =>
        {
            this.ShowRateAfter_CompleteLevel = (string)value.StringValue;
            ratingShowAfterCompleteLevel = GetNumber(ShowRateAfter_CompleteLevel);
        });
      
    }    

    private List<int>GetNumber(string s)
    {
        List<int>listNumber = new List<int>();
        int length = s.Length;
        int n = 0;
        for(int i =0; i <length;i++)
        {
            if (s[i] >='0' && s[i] <='9')
            {
                n = n * 10 + (s[i] - '0');
            }  
            else
            {
                listNumber.Add(n);
                n = 0;
            }    
        }
        listNumber.Add(n);
        return listNumber;
    }

    public bool CanShowRate()
    {
        if (DataPlayer.alldata.showRated == true) return false;
        int levelCurrent = DataPlayer.GetLevelValue();
        if (ratingShowAfterCompleteLevel.Contains(levelCurrent -1)) return true;//show ở level 3 nhưng khi hết level 3 thì đã level =4 nên cần - 1;
        return false;
    }
    #region Show InterAd when win
    public bool CanShowInterWin()
    {
        if (DataPlayer.alldata.noAds == true) return false;
        if (AdManager.Instant.InterstitialIsLoaded() == false) return false;

        int level = DataPlayer.GetLevelValue();
        bool isNormal = DataPlayer.alldata.isNormalMap;
        if (level < 3) return false;
        else if ((level - 3) % interAdDistanceBetweenLevels != 0)
        {
            return false;
        }
        else if (level == 3 || level == 11 || level == 31)//chơi hết level 2 => level = 3 ,isNormal = false;
        {
            if (isNormal)
                return false;
        }
        return true;
    }
    public IEnumerator IEShowInterAds(Action<InterVideoState> callback)
    {
        yield return null;
        Debug.Log("ShowInterstitial - IEShowInterAds");
        AdManager.Instant.ShowInterstitial(callback, true);

    }
    private void Callback_ShowInterWinLevel(InterVideoState state)
    {
        if (state == InterVideoState.Closed || state == InterVideoState.None)
        {
            SceneManager.LoadScene(GameConst.Game_Scene);
        }
    }

    public void ShowInterWin()
    {
        StartCoroutine(IEShowInterAds(Callback_ShowInterWinLevel));
    }
    #endregion

    public bool CanShowInterLose()
    {
        if (DataPlayer.alldata.noAds == true)return false;
        if (AdManager.Instant.InterstitialIsLoaded() == false) return false;
        if (DataPlayer.GetLevelValue() < 2)return false;

        int countFail = DataPlayer.alldata.countFail;
        if (replayCountRequiredForInterAd.Contains(countFail) == false) return false;//countFail không có trong List 1 3 5 7 thi khong show

        return true;
    }
    private void Callback_ShowInterLose(InterVideoState state)
    {
        if (state == InterVideoState.Closed || state == InterVideoState.None)
        {
            SceneManager.LoadScene(GameConst.Game_Scene);
        }
    }    

    public void ShowInterLose()
    {
        StartCoroutine(IEShowInterAds(Callback_ShowInterLose));
    }    


}
