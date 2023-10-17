using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGamePref 
{
    public static MoneyData moneyData;

    public static void SetMoneyData(MoneyData money_data)
    {
        moneyData = money_data;
        SaveMoneyData();
    }

    public static int GetMoney()
    {
        return moneyData.money;
    }

    public static void AddMoney(int amount)
    {
        moneyData.money += amount;
        SaveMoneyData();
    }

    public static void SaveMoneyData()
    {
        string data = JsonUtility.ToJson(moneyData);
        PlayerPrefs.SetString("MONEYDATA", data);
    }
}
