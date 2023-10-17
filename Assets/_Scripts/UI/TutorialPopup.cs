using DG.Tweening;
using DVAH;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialPopup : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] VideoPlayer video;
    [SerializeField] RectTransform videoRectTransform;
    [SerializeField] VideoConfig videoConfig;
    private void OnEnable()
    {
        video.clip = videoConfig.GetVideoClip(DataPlayer.alldata.indexVideoCurrent);
        video.Play();

        FireBaseManager.Instant.LogEventWithParameterAsync("tutorial_start", new Hashtable()
        {
            {
                "id_screen","TUTORIAL"
            },
            {
                "id_level",DataPlayer.GetLevelValue()
            }
        });
    }
    private void Start()
    {
        startButton.onClick.AddListener(OnclickStartButton);
    }

    private void OnclickStartButton()
    {
        GameManager.Instance.StartWaitReady();
        GameManager.Instance.SetPositionHand();
        SoundFXManager.Instance.PlayClickButton();
        videoRectTransform.DOAnchorPosY(750f, 0.3f);
        startButton.gameObject.SetActive(false);
        GameManager.Instance.TapVibrate();
    }
}
