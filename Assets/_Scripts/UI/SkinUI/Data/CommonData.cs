using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonData
{
    public ItemElementData[] itemData;


    public CommonData()
    {
        itemData = new ItemElementData[]{
            new(),new(),new(),new(),new(),new(),new(),new(),new(),new()
        };
    }
}

[System.Serializable]
public class ItemElementData
{
    public int id;
    public string itemType;
    public int requireAdsNumber;
    public ItemElementData()
    {
        this.id = 1;
        this.itemType = "a";
        this.requireAdsNumber = 0;
    }
}