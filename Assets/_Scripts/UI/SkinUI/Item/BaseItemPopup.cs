using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class BaseItemPopup : MonoBehaviour
{
    [Header("Item Data")]
    [SerializeField] private ItemData itemsData;
    [SerializeField] protected Transform itemsContainer;
    [SerializeField] private GameObject item;

    [Header("Button")]
    [SerializeField] private GameObject DefaultBottomButton;
    [SerializeField] protected Button unlockByMoney;

    public GameObject UnlockAdsItemButton;

    protected List<GameObject> itemList = new List<GameObject>();
    protected bool isSetData;

    // Start is called before the first frame update
    private void OnEnable()
    {
        if(itemsContainer.childCount <= 0)
        StartCoroutine(SpawnItems());
    }

    private void OnDisable()
    {
        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
        }
        isSetData = false;
    }

    private IEnumerator SpawnItems()
    {
        itemList.Clear();
        yield return new WaitForSeconds(.1f);
        for (int i = 0; i < itemsData.items.Length; i++)
        {
            GameObject obj = Instantiate(item, itemsContainer);
            //SetDataItem(obj, itemsData.items[i]);
            obj.transform.localScale = Vector3.zero;
            itemList.Add(obj);
        }

        foreach (GameObject obj in itemList)
        {
            obj.transform.DOScale(1f, .5f).From(0f);
            yield return new WaitForSeconds(.1f);
        }
    }
 
    // Update is called once per frame
    void Update()
    {
        
    }

    protected void SetDataItem(GameObject obj, ItemElementData itemData)
    {
        ItemController itemController = obj.GetComponent<ItemController>();

        itemController.itemType = itemData.itemType.ToString();
        itemController.idItem = itemData.id;
        itemController.requireAdsNumber = itemData.requireAdsNumber;
        itemController.adsCount.text = " aaa " + itemData.requireAdsNumber.ToString();
        itemController.UpdateView();
    }

    
    public void TurnOnTheDefaultBottomButton()
    {
        DefaultBottomButton.gameObject.SetActive(true);
        UnlockAdsItemButton.gameObject.SetActive(false);
    }

    public void TurnOnUnlockAdsItemButton()
    {
        UnlockAdsItemButton.gameObject.SetActive(true);
        DefaultBottomButton.gameObject.SetActive(false);
    }

    public void TurnOffAll()
    {
        UnlockAdsItemButton.gameObject.SetActive(false);
        DefaultBottomButton.gameObject.SetActive(false);
    }

    protected void OnUnlockByAds(int id)
    {
        for(int i = 0; i < itemsData.items.Length; i++)
        {
            if(i == id)
            {
                ItemController itemController = itemList[i].gameObject.GetComponent<ItemController>();
                if(itemController.requireAdsNumber > 0)
                itemController.requireAdsNumber--;


                if(itemController.requireAdsNumber == 0)
                itemController.itemType = itemController.Free;

                itemController.SetTypeItem();
                itemController.UpdateRequireAds();
            }
        }
    }

    protected void OnUnlockByMoney()
    {
        Debug.Log("vao day");

        //lay ra nhung item là money item
        List<int> item_money_index = new List<int>();
        for (int i = 0; i < itemsData.items.Length; i++)
        {
            ItemController itemController = itemList[i].gameObject.GetComponent<ItemController>();
            if (itemController.itemType == itemController.Money)
            {
                item_money_index.Add(i);
            }
        }

        System.Random rnd = new System.Random();
        int random_index = item_money_index[rnd.Next(item_money_index.Count)];

        Debug.Log("random index " + random_index);
        //hieu ứng random với những money item
        StartCoroutine(SelectRandomEffect(random_index));

    }

    protected IEnumerator SelectRandomEffect(int random_index)
    {
        int time = 3, count = 0;

        while(count <= time)
        {
            for (int i = 0; i < itemsData.items.Length; i++)
            {
                ItemController itemControllertemp = itemList[i].gameObject.GetComponent<ItemController>();
                if (itemControllertemp.itemType == itemControllertemp.Money)
                {
                    itemControllertemp.border.SetActive(true);
                    Debug.Log("turn on border");
                    yield return new WaitForSeconds(.3f);
                    itemControllertemp.border.SetActive(false);
                }
            }
            count++;
        }

        ItemController itemController = itemList[random_index].gameObject.GetComponent<ItemController>();
        itemController.border.SetActive(true);

        yield return new WaitForSeconds(.2f);

        itemController.itemType = itemController.Free;
        itemController.SetTypeItem();
    }
}
