using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EpicPopup : BaseItemPopup, IGetIdItem, ITurnOnButton
{
    public int idClick;

    public static EpicPopup Instance { get; set; }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        UnlockAdsItemButton.GetComponent<Button>().onClick.AddListener(delegate {
            OnUnlockByAds(idClick);
            SkinPref.SetRequireAdsEpicData(idClick, SkinPref.GetNumberAdsRequireInEpic(idClick) - 1);

            if(SkinPref.GetNumberAdsRequireInEpic(idClick) <= 0)
            SkinPref.SetTypeEpicItem(idClick, "Free");
        });
        unlockByMoney.onClick.AddListener(delegate {
            OnUnlockByMoney();
            SkinPref.SetTypeEpicItem(idClick, "Free");
        });
        StartCoroutine(SetEpicData());
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSetData && itemsContainer.childCount == 10)
        {
            isSetData = true;
            StartCoroutine(SetEpicData());
        }
    }

    private IEnumerator SetEpicData()
    {
        yield return new WaitForSeconds(.3f);
        Debug.Log(itemList.Count);
        for (int i = 0; i < itemList.Count; i++)
        {
            Debug.Log(itemList[i].name);
            if (SkinPref.GetItemEpicData(i) == null) Debug.Log("null");
            SetDataItem(itemList[i], SkinPref.GetItemEpicData(i));
        }
    }

    public void OnClickItem(ItemController itemController)
    {
        idClick = itemController.idItem;
    }

    public void GetIdItem(ItemController itemController)
    {
        idClick = itemController.idItem;
    }
}
