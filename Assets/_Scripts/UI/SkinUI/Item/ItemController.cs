using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemController : MonoBehaviour
{
    public string Free = "Free", Money = "Money", Level = "Level", WatchAds = "WatchAds";
    public int idItem;
    public string itemType;
    public int requireAdsNumber;
    public GameObject border;

    [SerializeField] private ItemWatchAds itemWatchAds;
    [SerializeField] private FreeItem freeItem;
    [SerializeField] private LockLevelItem lockLevelItem;
    [SerializeField] private MoneyItem moneyItem;
    public TextMeshProUGUI adsCount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        DeactiveAllItems();

        switch(itemType){
            case "Money":
                moneyItem.gameObject.SetActive(true);
                break;
            case "Level":
                lockLevelItem.gameObject.SetActive(true);
                break;
            case "Free":
                freeItem.gameObject.SetActive(true);
                break;
            case "WatchAds":
                itemWatchAds.gameObject.SetActive(true);
                adsCount.text = requireAdsNumber.ToString();
                break;
        }
    }

    public void SetTypeItem()
    {
        DeactiveAllItems();

        switch (itemType)
        {
            case "Money":
                moneyItem.gameObject.SetActive(true);
                break;
            case "Level":
                lockLevelItem.gameObject.SetActive(true);
                break;
            case "Free":
                freeItem.gameObject.SetActive(true);
                break;
            case "WatchAds":
                itemWatchAds.gameObject.SetActive(true);
                adsCount.text = requireAdsNumber.ToString();
                break;
        }
    }

    protected void DeactiveAllItems()
    {
        moneyItem.gameObject.SetActive(false);
        lockLevelItem.gameObject.SetActive(false);
        freeItem.gameObject.SetActive(false);
        itemWatchAds.gameObject.SetActive(false);
    }

    public void UpdateView()
    {
        DeactiveAllItems();

        switch (itemType)
        {
            case "Money":
                moneyItem.gameObject.SetActive(true);
                break;
            case "Level":
                lockLevelItem.gameObject.SetActive(true);
                break;
            case "Free":
                freeItem.gameObject.SetActive(true);
                break;
            case "WatchAds":
                itemWatchAds.gameObject.SetActive(true);
                adsCount.text = requireAdsNumber.ToString();
                break;
        }
    }

    public void OnClickWatchAdsItem()
    {
        itemWatchAds.gameObject.SetActive(false);
        freeItem.gameObject.SetActive(true);
    }

    public void TurnOnBorder()
    {
        border.SetActive(true);
    }

    public void UpdateRequireAds()
    {
        adsCount.text = requireAdsNumber.ToString();
    }
}
