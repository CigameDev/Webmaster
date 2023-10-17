using DG.Tweening;
using DVAH;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NewAvataPopup : MonoBehaviour
{
    [SerializeField] Button claimButton;
    [SerializeField] Button loseitButton;
    [SerializeField] PlayerUI playerUI;
    [SerializeField] GameObject normalVfx;
    [SerializeField] GameObject rareVfx;
    [SerializeField] GameObject epicVfx;
    [SerializeField] SkinConfig skinConfig;
    [SerializeField] Transform _light;

    [SerializeField] CanvasGroup canvasGroupLoseit;

    int levelValueCurrent;
    int idUnlock;
    Tweener rotationTween;
    private void OnEnable()
    {
        levelValueCurrent = DataPlayer.GetLevelValue();//level 6 unlock skins[1]
        idUnlock = GetNextSkinUnlock(levelValueCurrent);
        DataPlayer.UnlockSkin(idUnlock);//unlock nhung chua so huu
        playerUI.PlayRandomClip();
        playerUI.ChangeSkin(idUnlock);
        PlayVfxById(idUnlock);
    }
    private void Start()
    {
        claimButton.onClick.AddListener(OnclickClaimButton);
        loseitButton.onClick.AddListener(OnclickLoseitButton);

        StartCoroutine(IEShowLoseitButton());
        Rotate();
    }

    private void Rotate()
    {
        rotationTween =_light.DORotate(new Vector3(0, 0, -360f), 10f, RotateMode.FastBeyond360).SetEase(Ease.Linear).OnComplete(() => Rotate());
    }

    private void OnDestroy()
    {
        rotationTween.Kill();
    }
    private void OnclickClaimButton()
    {
        SoundFXManager.Instance.PlayClickButton();
        GameManager.Instance.TapVibrate();
        //xem quang cao nhan reward skin 
        StartCoroutine(ShowRewardAd(Callback_ClaimButton));

        //DataPlayer.GetSkin(idUnlock);
        //DataPlayer.SetSkinCurrentValue(idUnlock);//nhan skin moi va su dung luon
        //GameEvent.instance.PostEvent_ChangeSkin(idUnlock);//thay doi luon trong gameplay
        //this.gameObject.SetActive(false);
        

    }
    private void OnclickLoseitButton()
    {
        SoundFXManager.Instance.PlayClickButton();
        GameManager.Instance.TapVibrate();
        this.gameObject.SetActive(false);
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
    private int GetNextSkinUnlock(int level)
    {
        for(int i =0;i <7;i++)
        {
            if (skinConfig.GetValueLevelUnlockSkin(i) == level - 1) return i;
        }
        return 0;
    }    

    private IEnumerator IEShowLoseitButton()
    {
        loseitButton.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        loseitButton.gameObject.SetActive(true);
        float fadeTime = 0f;
        canvasGroupLoseit.alpha = fadeTime;

        while(fadeTime < 1f)
        {
            yield return new WaitForSeconds(0.05f);
            fadeTime += 0.05f;
            canvasGroupLoseit.alpha = fadeTime;
        }    

    }

    private IEnumerator ShowRewardAd(Action<RewardVideoState> callback)
    {
        yield return null;
        AdManager.Instant.ShowRewardVideo(callback, true);
    }

    private void Callback_ClaimButton(RewardVideoState state)
    {
        if (state == RewardVideoState.Watched)
        {
            DataPlayer.GetSkin(idUnlock);
            DataPlayer.SetSkinCurrentValue(idUnlock);//nhan skin moi va su dung luon
            GameEvent.instance.PostEvent_ChangeSkin(idUnlock);//thay doi luon trong gameplay
            this.gameObject.SetActive(false);
        }
    }
}
