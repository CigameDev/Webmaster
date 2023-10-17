using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinPref 
{
    static CommonData commonData;
    static RareData rareData;
    static EpicData epicData;

    #region RareData
    public static void SetRareData(RareData rare_data)
    {
        rareData = rare_data;
        SaveRareData();
    }

    public static void SetRequireAdsRareData(int item_id, int numberAds)
    {
        rareData.itemData[item_id].requireAdsNumber = numberAds;
        SaveRareData();
    }
    public static int GetNumberAdsRequireInRare(int item_id)
    {
        return rareData.itemData[item_id].requireAdsNumber;
    }

    public static void SetTypeRareItem(int item_id, string type)
    {
        rareData.itemData[item_id].itemType = type;
        SaveRareData();
    }

    public static ItemElementData GetItemRareData(int idx)
    {
        return rareData.itemData[idx];
    }

    public static string GetTypeRareItem(int item_id)
    {
        return rareData.itemData[item_id].itemType;
    }
    #endregion

    #region EpicData
    public static void SetEpicData(EpicData epic_data)
    {
        epicData = epic_data;
        SaveEpicData();
    }

    public static void SetRequireAdsEpicData(int item_id, int numberAds)
    {
        epicData.itemData[item_id].requireAdsNumber = numberAds;
        SaveEpicData();
    }

    public static void SetTypeEpicItem(int item_id, string type)
    {
        epicData.itemData[item_id].itemType = type;
        SaveEpicData();
    }

    public static ItemElementData GetItemEpicData(int idx)
    {
        return epicData.itemData[idx];
    }

    public static int GetNumberAdsRequireInEpic(int item_id)
    {
        return epicData.itemData[item_id].requireAdsNumber;
    }

    public static string GetTypeEpicItem(int item_id)
    {
        return epicData.itemData[item_id].itemType;
    }
    #endregion

    #region CommonData
    public static void SetCommonData(CommonData common_data)
    {
        commonData = common_data;
        SaveCommonData();
    }
    public static void SetRequireAdsCommonData(int item_id, int numberAds)
    {
        commonData.itemData[item_id].requireAdsNumber = numberAds;
        SaveCommonData();
    }

    public static int GetNumberAdsRequireInCommon(int item_id)
    {
        return commonData.itemData[item_id].requireAdsNumber;
    }

    public static void SetTypeCommonItem(int item_id, string type)
    {
        commonData.itemData[item_id].itemType = type;
        SaveCommonData();
    }

    public static ItemElementData GetItemCommonData(int idx)
    {
        return commonData.itemData[idx];
    }

    public static string GetTypeCommonItem(int item_id)
    {
        return commonData.itemData[item_id].itemType;
    }
    #endregion

    public static void SaveCommonData()
    {
        string common_data = JsonUtility.ToJson(commonData);
        PlayerPrefs.SetString("COMMON_DATA", common_data);
    }

    public static void SaveRareData()
    {
        string rare_data = JsonUtility.ToJson(rareData);
        PlayerPrefs.SetString("RARE_DATA", rare_data);
    }

    public static void SaveEpicData()
    {
        string rare_data = JsonUtility.ToJson(rareData);
        PlayerPrefs.SetString("EPIC_DATA", rare_data);
    }
}
