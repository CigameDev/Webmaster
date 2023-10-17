using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinEntry : MonoBehaviour
{
    [SerializeField] Button closeButton;

    [SerializeField] protected ItemData itemCommonData;
    [SerializeField] protected ItemData itemRareData;
    [SerializeField] protected ItemData itemEpicData;

    private void OnEnable()
    {

    }

    void Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            this.gameObject.SetActive(false);
        });
        SetDefaultDataAppOpen();
    }

    void Update()
    {
        
    }

    protected void SetDefaultDataAppOpen()
    {
        SkinPref.SetCommonData(GetCommonData());
        SkinPref.SetRareData(GetRareData());
        SkinPref.SetEpicData(GetEpicData());
    }

    CommonData GetCommonData()
    {
        CommonData common_data;
        if (PlayerPrefs.HasKey("COMMON_DATA"))
        {
            string data = PlayerPrefs.GetString("COMMON_DATA");
            common_data = JsonUtility.FromJson<CommonData>(data);
        }

        else
        {
            common_data = new CommonData();
            
            Debug.Log(itemCommonData.items[3].id);
            for(int i = 0; i < itemCommonData.items.Length; i++)
            {
                common_data.itemData[i] = new ItemElementData();
                common_data.itemData[i].requireAdsNumber = itemCommonData.items[i].requireAdsNumber;
                common_data.itemData[i].id = itemCommonData.items[i].id;
                common_data.itemData[i].itemType = itemCommonData.items[i].itemType.ToString();
            }
        }

        return common_data;
    }

    RareData GetRareData()
    {
        RareData rare_data;
        if (PlayerPrefs.HasKey("RARE_DATA"))
        {
            string data = PlayerPrefs.GetString("RARE_DATA");
            rare_data = JsonUtility.FromJson<RareData>(data);
        }

        else
        {
            rare_data = new RareData();

            //Debug.Log(itemRareData.items[3].id);
            for (int i = 0; i < itemRareData.items.Length; i++)
            {
                rare_data.itemData[i] = new ItemElementData();
                rare_data.itemData[i].requireAdsNumber = itemRareData.items[i].requireAdsNumber;
                rare_data.itemData[i].id = itemRareData.items[i].id;
                rare_data.itemData[i].itemType = itemRareData.items[i].itemType.ToString();
            }
        }

        return rare_data;
    }

    EpicData GetEpicData()
    {
        EpicData epic_data;
        if (PlayerPrefs.HasKey("EPIC_DATA"))
        {
            string data = PlayerPrefs.GetString("EPIC_DATA");
            epic_data = JsonUtility.FromJson<EpicData>(data);
        }

        else
        {
            epic_data = new EpicData();

            Debug.Log(itemRareData.items[3].id);
            for (int i = 0; i < itemRareData.items.Length; i++)
            {
                epic_data.itemData[i] = new ItemElementData();
                epic_data.itemData[i].requireAdsNumber = itemEpicData.items[i].requireAdsNumber;
                epic_data.itemData[i].id = itemRareData.items[i].id;
                epic_data.itemData[i].itemType = itemRareData.items[i].itemType.ToString();
            }
        }

        return epic_data;
    }
}
