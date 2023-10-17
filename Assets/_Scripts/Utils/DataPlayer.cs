using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataPlayer 
{
    public const string ALL_DATA = "all_data";
    public static AllData alldata;
    static DataPlayer()
    {
        alldata = JsonUtility.FromJson<AllData>(PlayerPrefs.GetString(ALL_DATA));
        if(alldata == null)
        {
            alldata = new AllData
            {
                level = 1,
                monney = 0,
                hasSound = true,
                hasVibration = true,
                skinCurrent = 0,
                ownedSkins = new List<int> { 0 },
                lockSkins = new List<int> { 1, 2, 3, 4, 5, 6 },
                unlockedSkins = new List<int> { },

                lastValueSkinUnlocked = 0,
                isNormalMap = false,
                valueProcessNewSkin = 0f,
                indexVideoCurrent = 0,
                countFail = 0,
                noAds = false,
                showRated = false,
            }; 
            SaveData();
        }    
    }

    public static void SaveData()
    {
        var json = JsonUtility.ToJson(alldata);
        PlayerPrefs.SetString(ALL_DATA, json);
    }
    public static int GetLevelValue()
    {
        return alldata.level;
    }
    public static void SetLevelValue(int level)
    {
        alldata.level = level;
        SaveData();
    }

    public static int GetMonneyValue()
    {
        return alldata.monney;
    }
    public static void SetMonneyValue(int monney)
    {
        alldata.monney = monney;
        SaveData();
    }

    public static bool GetHasSound()
    {
        return alldata.hasSound;
    }
    public static void SetHasSound(bool hasSound)
    {
        alldata.hasSound = hasSound;
        SaveData();
    }
    public static bool GetHasVibration()
    {
        return alldata.hasVibration;
    }
    public static void SetHasVibration(bool hasVibration)
    {
        alldata.hasVibration = hasVibration;
        SaveData();
    }
    public static int GetSkinCurrentvalue()
    {
        return alldata.skinCurrent;
    }
    public static void SetSkinCurrentValue(int value)
    {
        alldata.skinCurrent = value;
        SaveData();
    }
    public static void GetSkin(int skinValue)
    {
        alldata.GetSkin(skinValue);
        SaveData();
    }
    public static void UnlockSkin(int skinValue)
    {
        alldata.UnlockSkin(skinValue);
        SaveData();
    }
    public static void SetLastValueSkinUnlocked(int value)
    {
        alldata.lastValueSkinUnlocked = value;
        SaveData();
    }
    public static void SetIsNormalMap(bool isNormalMap)
    {
        alldata.isNormalMap = isNormalMap;
        SaveData();
    }

    public static void SetValueProcessNewSkin(float value)
    {
        alldata.valueProcessNewSkin = value;
        SaveData();
    }

    public static void SetIndexVideoCurrent(int value)
    {
        alldata.indexVideoCurrent = value;
        SaveData();
    }

    public static void SetCountFail(int value)
    {
        alldata.countFail = value;
        SaveData();
    }
    public static void SetNoAds(bool value)
    {
        alldata.noAds = value;
        SaveData();
    }
    public static void SetShowRated(bool value)
    {
        alldata.showRated = value;
        SaveData();
    }

}

public class AllData
{
    public int level;
    public int monney;
    public int skinCurrent;
    public bool hasSound;
    public bool hasVibration;
    public List<int> ownedSkins;//da so huu
    public List<int> unlockedSkins;//da mo khoa nhung chua so huu
    public List<int> lockSkins;//chua mo khoa 

    public int lastValueSkinUnlocked;//0 1 2 3 4 5 6
    public bool isNormalMap;// co map tutorial va map thuong
    public float valueProcessNewSkin;
    public int indexVideoCurrent;
    public int countFail;
    public bool noAds;
    public bool showRated;
    
    public void GetSkin(int skinValue)//nhan SKin moi
    {
        if(!ownedSkins.Contains(skinValue))
        {
            ownedSkins.Add(skinValue);
            unlockedSkins.Remove(skinValue);
        }    
    }

    public void UnlockSkin(int skinValue)//mo khoa skin nhung chua so huu,phai xem ads
    {
        if(!unlockedSkins.Contains(skinValue))
        {
            unlockedSkins.Add(skinValue);
            lockSkins.Remove(skinValue);
        }    
    }
}
