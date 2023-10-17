using DVAH;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemSkinButton : MonoBehaviour
{
    public int id { private get; set; }
    public SkinShop skinShop { private get; set; }


    [SerializeField] GameObject adsBar;
    [SerializeField] GameObject itemLevel;
    [SerializeField] GameObject choose;
    [SerializeField] Image main;


    private Button itemSkinButton;
    private void Awake()
    {
        itemSkinButton = GetComponent<Button>();
    }
    private void Start()
    {

        itemSkinButton.onClick.AddListener(() =>
        {
            SoundFXManager.Instance.PlayClickButton();
            GameManager.Instance.TapVibrate();
            if (DataPlayer.alldata.ownedSkins.Contains(id))//neu co trong list da so huu thi len ScreenShow va luu lai trang thai
            {

                DataPlayer.SetSkinCurrentValue(id);
                skinShop.playerUI.ChangeSkin(id);
                skinShop.EnableClaimButton(false);
                skinShop.PlayVfxById(id);
                skinShop?.HideChoose();
                ShowChoose(true);

                //cbon 1 nhan vat co ID la id de choi

                FireBaseManager.Instant.LogEventWithParameterAsync("collection_select", new Hashtable()
                {
                    {
                        "id_screen","COLLECTION"
                    },
                    {
                        "id_character",id
                    }
                });
            }    
            else if(DataPlayer.alldata.unlockedSkins.Contains(id))// neu co trong list da unlock thi cho hien thi len ScreenShow nhung khong luu lai trang thai
            {
                skinShop.playerUI.ChangeSkin(id);
                skinShop.EnableClaimButton(true);
                skinShop.idSelected = id;
                skinShop.PlayVfxById(id);
                skinShop?.HideChoose();
                ShowChoose(true);
            }    
        });
    }

    public  void ShowAdsBar(bool isShow)
    {
        adsBar.SetActive(isShow);
    }    
    public void ShowItemLevel(bool isShow)
    {
        itemLevel.SetActive(isShow);
    }
    public void ShowChoose(bool isShow)
    {
        choose.SetActive(isShow);
    }
    public void SetSprite(Sprite sprite)
    {
        main.sprite = sprite;
    }    
    public void SetTextLevelUnlock(int value)
    {
        if(itemLevel ==null) return;
        TextMeshProUGUI text = itemLevel.GetComponentInChildren<TextMeshProUGUI>();
        text.text = "LEVEL " + value;
    }
    public void EnableItemSkinButton(bool isShow)
    {
        itemSkinButton.interactable = isShow;
    }

  
}
