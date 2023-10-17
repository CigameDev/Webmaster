using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommonPopup : BaseItemPopup, IGetIdItem, ITurnOnButton
{
    public int idClick;
    public static CommonPopup Instance { get; set; }

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
            SkinPref.SetRequireAdsCommonData(idClick, SkinPref.GetNumberAdsRequireInCommon(idClick) - 1);

            if(SkinPref.GetNumberAdsRequireInCommon(idClick) <= 0)
            SkinPref.SetTypeCommonItem(idClick, "Free");
        });
        unlockByMoney.onClick.AddListener(delegate {
            OnUnlockByMoney();
            SkinPref.SetTypeCommonItem(idClick, "Free");
        });
        StartCoroutine(SetCommonData());
    }

    // Update is called once per frame
    void Update()
    {
        if(!isSetData && itemsContainer.childCount == 10)
        {
            isSetData = true;
            StartCoroutine(SetCommonData());
        }
    }

    private IEnumerator SetCommonData()
    {
        yield return new WaitForSeconds(.3f);
        Debug.Log(itemList.Count);
        for(int i = 0; i < itemList.Count; i++)
        {
            Debug.Log(itemList[i].name);
            if (SkinPref.GetItemCommonData(i) == null) Debug.Log("null");
            SetDataItem(itemList[i], SkinPref.GetItemCommonData(i));
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
