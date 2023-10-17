using DG.Tweening;
using DVAH;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinShop : MonoBehaviour
{
    public PlayerUI playerUI;
    [HideInInspector] public int idSelected;
    [SerializeField] Button closeButton;
    [SerializeField] Button claimButton;
    [SerializeField] Button watchAdsButton;
    [SerializeField] Button unlockRandomButton;

    [SerializeField] Transform parent;
    [SerializeField] GameObject itemSkinButtonPrefab;
    [SerializeField] GameObject normalVfx;
    [SerializeField] GameObject rareVfx;
    [SerializeField] GameObject epicVfx;
    [SerializeField] TextMeshProUGUI textMonney;
    [SerializeField] SkinConfig skinConfig;
    [SerializeField] Transform _light;
    private Tweener tweener;
    private List<ItemSkinButton> skinButtons = new List<ItemSkinButton>();
    private int skinValueCurrent;
    private int monneyValueCurrent;

    private ItemSkinButton itemSkinButton;

    private void OnEnable()
    {
        skinValueCurrent = DataPlayer.GetSkinCurrentvalue();
        playerUI.ChangeSkin(skinValueCurrent);
        playerUI.PlayRandomClip();
        monneyValueCurrent = DataPlayer.GetMonneyValue();
        SetTextMonney(monneyValueCurrent);
        EnableClaimButton(false);
        ShowUnlockRandomButton();
        MusicController.instance.PlayShopMusic();
        RotateLight();


        //playerUI.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -300, -150);
        FireBaseManager.Instant.LogEventWithParameterAsync("collection_start", new Hashtable()
        {
            {
                "id_screen","COLLECTION"
            }
        });
    }
    private void Start()
    {
        PlayVfxById(skinValueCurrent);
        SpawnItemSkinButton();
        closeButton.onClick.AddListener(OnclickCloseButton);
        claimButton.onClick.AddListener(OnclickClaimButton);
        watchAdsButton.onClick.AddListener(OnclickWatchAdsButton);
        unlockRandomButton.onClick.AddListener(OnclickUnlockRandomButton);

    }

    private void OnDisable()
    {
        tweener.Kill();

    }
    private void RotateLight()
    {
        tweener = _light.DORotate(new Vector3(0, 0, -360f), 10f, RotateMode.FastBeyond360).SetEase(Ease.Linear).OnComplete(() => RotateLight());
    }

    private void OnclickCloseButton()
    {
        MusicController.instance.StopMusic();
        GameManager.Instance.TapVibrate();
        SoundFXManager.Instance.PlayClickButton();
        int skinvalueCurrent = DataPlayer.GetSkinCurrentvalue();
        GameEvent.instance.PostEvent_ChangeSkin(skinvalueCurrent);
        GameEvent.instance.PostEvent_GetMonney(monneyValueCurrent);
        EnableClaimButton(false);
        this.gameObject.SetActive(false);

        FireBaseManager.Instant.LogEventWithParameterAsync("btn_home", new Hashtable()
        {
            {
                "id_screen","COLLECTION"
            },
        });
    }    
    private void OnclickClaimButton()
    {
        SoundFXManager.Instance.PlayClickButton();
        GameManager.Instance.TapVibrate();

        StartCoroutine(ShowRewardAd(Callback_ClaimButton));

    }    
    private void OnclickWatchAdsButton()
    {
        //xem ads xong nhan thuong 200 tien
        SoundFXManager.Instance.PlayClickButton();
        GameManager.Instance.TapVibrate();

        StartCoroutine(ShowRewardAd(Callback_WatchAdsButton));

        FireBaseManager.Instant.LogEventWithParameterAsync("collection_money_ad", new Hashtable()
        {
            {
                "id_screen","COLLECTION"
            }
        });
    }    
    private void OnclickUnlockRandomButton()
    {
        //mo khoa 1 skin bat ky từ unlockedSkins
        SoundFXManager.Instance.PlayClickButton();
        GameManager.Instance.TapVibrate();
        if (monneyValueCurrent >=700)
        {
            monneyValueCurrent -= 700;
            DataPlayer.SetMonneyValue(monneyValueCurrent);
            SetTextMonney(monneyValueCurrent);
            int count = DataPlayer.alldata.unlockedSkins.Count;
            if(count > 0)
            {
                int r = UnityEngine.Random.Range(0, count);
                int value = DataPlayer.alldata.unlockedSkins[r];
                DataPlayer.GetSkin(value);

                skinButtons[value].ShowAdsBar(false);
                ShowUnlockRandomButton();
                EnableClaimButton(false);
            }
        }    
    }    
    private void SpawnItemSkinButton()
    {
        for(int i =0;i < 7;i++)
        {
            var item = Instantiate(itemSkinButtonPrefab, parent);
            ItemSkinButton itemSkinButton = item.GetComponent<ItemSkinButton>();
            skinButtons.Add(itemSkinButton);
            itemSkinButton.id = i;
            itemSkinButton.skinShop = this;
            itemSkinButton.SetSprite(skinConfig.GetSpriteIconSkin(i));
            if(i == skinValueCurrent)
            {
                itemSkinButton.ShowChoose(true);
            }    
            if (DataPlayer.alldata.ownedSkins.Contains(i))
            {
                itemSkinButton.ShowAdsBar(false);
                itemSkinButton.ShowItemLevel(false);
            }
            else if (DataPlayer.alldata.unlockedSkins.Contains(i))
            {
                itemSkinButton.ShowItemLevel(false);
            }
            else//lock thi 
            {
                itemSkinButton.ShowAdsBar(false);
                itemSkinButton.SetTextLevelUnlock(skinConfig.GetValueLevelUnlockSkin(i));
                itemSkinButton.EnableItemSkinButton(false);

            }
        }    
    }    

    public void EnableClaimButton(bool isShow)
    {
        claimButton.interactable = isShow;
    }    

    private void EnableUnlockRandomButton(bool isShow)
    {
        unlockRandomButton.interactable = isShow;
    }
    private void SetTextMonney(int  money)
    {
        textMonney.text = money.ToString();
    }    
    private void ShowUnlockRandomButton()
    {
        if(DataPlayer.alldata.unlockedSkins.Count > 0)
        {
            EnableUnlockRandomButton(true);
        }
        else
        {
            EnableUnlockRandomButton(false);
        }    
    }

    private void PlayNormalVfx()
    {
        normalVfx.SetActive(true);
        epicVfx.SetActive(false);
        rareVfx.SetActive(false);
    }    
    private void PlayEpicVfx()
    {
        normalVfx.SetActive(false);
        epicVfx.SetActive(true);
        rareVfx.SetActive(false);
    }    
    private void PlayRareVfx()
    {
        normalVfx.SetActive(false);
        epicVfx.SetActive(false);
        rareVfx.SetActive(true);
    }    
    public void PlayVfxById(int idSkin)
    {
        if (skinConfig.modeSkins[idSkin] == ModeSKin.Normal)
        {
            PlayNormalVfx();
        }
        else if (skinConfig.modeSkins[idSkin] == ModeSKin.Rare)
        {
            PlayRareVfx();
        }
        else
        {
            PlayEpicVfx();
        }
    }

    public void HideChoose()
    {
        for(int i =0;i < skinButtons.Count;i++)
        {
            skinButtons[i].ShowChoose(false);
        }    
    }

    private IEnumerator ShowRewardAd(Action<RewardVideoState> callback)
    {
        yield return null;
        AdManager.Instant.ShowRewardVideo(callback, true);
    }

    private void Callback_ClaimButton(RewardVideoState state)
    {
        if(state == RewardVideoState.Watched)
        {
            EnableClaimButton(false);
            skinButtons[idSelected].ShowAdsBar(false);

            DataPlayer.GetSkin(idSelected);
            DataPlayer.SetSkinCurrentValue(idSelected);

            FireBaseManager.Instant.LogEventWithParameterAsync("collection_select", new Hashtable()
            {
                 {
                     "id_screen","COLLECTION"
                 },
                 {
                     "id_character",idSelected
                 }
            });
        }    
    }   
    
    private void Callback_WatchAdsButton(RewardVideoState state)
    {
        if (state == RewardVideoState.Watched)
        {
            monneyValueCurrent += 200;
            DataPlayer.SetMonneyValue(monneyValueCurrent);
            SetTextMonney(monneyValueCurrent);
        }
    }    


}
