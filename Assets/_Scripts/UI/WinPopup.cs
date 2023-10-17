using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using DVAH;
using System;

public class WinPopup : MonoBehaviour
{
    [SerializeField] Button GetPassLevelMonneyButton;
    [SerializeField] Button rewardMonneyButton;
    [SerializeField] GameObject sliderValueReward;
    [SerializeField] TextMeshProUGUI rewardValue;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI monneyText;
    [SerializeField] Transform monneyIcon;
    [SerializeField] GameObject greenPointPrefab, yellowPointPrefab, whitePointPrefab;
    [SerializeField] Transform parentPoint;
    [SerializeField] Measure measure;
    [SerializeField] GameObject PileOfMoneyParent;
    [SerializeField] Transform moneyTarget;
    [SerializeField] SkinConfig skinConfig;
    [SerializeField] Image newSkinImage;
    private Vector2 moneyTargetPosition;
    private Vector3[] InitialPos;
    private Quaternion[] InitialRotation;
    private int totalMonney;
    private Vector3 targetPoint;
    private GameObject yellowPoint;
    private GameObject greenPoint;
    private int MoneyNo = 12;
    private bool doneReward;

   
    private void OnEnable()
    {
        levelText.text = "LEVEL " + DataPlayer.GetLevelValue();
        totalMonney = DataPlayer.GetMonneyValue();
        monneyText.text = totalMonney.ToString();
        SpawnPointProgress(DataPlayer.GetLevelValue() - 1);
        Invoke(nameof(HideYellowPoint), 0.5f);
        SoundFXManager.Instance.PlayWin();

        FireBaseManager.Instant.LogEventWithParameterAsync("win_start", new Hashtable()
        {
            {
                "id_screen","WIN"
            },
            {
                "id_level",DataPlayer.GetLevelValue() -1
            }
        });
    }
    private void Start()
    {
        targetPoint = GetTargetPoint();
        GetPassLevelMonneyButton.onClick.AddListener(() =>
        {
            SoundFXManager.Instance.PlayClickButton();
            GameManager.Instance.TapVibrate();
            measure.StopMoveMeasureForward();
            HideButton();
            //RewardPileOfMoney(50);
            if (DataFireBaseConfig.Instance.CanShowInterWin())
            {
                StartCoroutine(DataFireBaseConfig.Instance.IEShowInterAds(Callback_ShowInterWinLevel));
            }
            else
            {
                HideButton();
                RewardPileOfMoney(50);
            }

            FireBaseManager.Instant.LogEventWithParameterAsync("win_btn_collect", new Hashtable()
            {
                {
                     "id_screen","WIN"
                },
                {
                    "id_level",DataPlayer.GetLevelValue() -1
                }
            });
        });

        rewardMonneyButton.onClick.AddListener(() =>
        {
            SoundFXManager.Instance.PlayClickButton();
            GameManager.Instance.TapVibrate();
            measure.StopMoveMeasureForward();

            if (AdManager.Instant.VideoRewardIsLoaded())
            {
                StartCoroutine(ShowRewardAd(Callback_ShowReward));
            }

            //FireBase khi nhan nut xem ads
            FireBaseManager.Instant.LogEventWithParameterAsync("win_btn_watch_ad", new Hashtable()
            {
                {
                     "id_screen","WIN"
                },
                {
                    "id_level",DataPlayer.GetLevelValue() -1
                }
            });

        });

        moneyTargetPosition = moneyTarget.position;
        InitialPos = new Vector3[MoneyNo];
        InitialRotation = new Quaternion[MoneyNo];

        for (int i = 0; i < PileOfMoneyParent.transform.childCount; i++)
        {
            InitialPos[i] = PileOfMoneyParent.transform.GetChild(i).position;
            InitialRotation[i] = PileOfMoneyParent.transform.GetChild(i).rotation;
        }

        newSkinImage.sprite = skinConfig.GetSpriteIconSkinWinpopup(DataPlayer.alldata.lastValueSkinUnlocked + 1);

    }
    private void Callback_ShowInterWinLevel(InterVideoState state)
    {
        if (state == InterVideoState.Closed || state == InterVideoState.None)
        {
            RewardPileOfMoney(50);
        }
    }
    private void HideButton()
    {
        GetPassLevelMonneyButton.gameObject.SetActive(false);
        rewardMonneyButton.gameObject.SetActive(false);
    }
    private IEnumerator IECountMonney(int monneyReward, float time)
    {
        yield return new WaitForSeconds(1.5f);
        float timeCount = 0f;
        while (timeCount < time)
        {
            timeCount += Time.deltaTime;
            float perCent = timeCount / time;
            int tempMonney = totalMonney + Mathf.RoundToInt(perCent * monneyReward);
            yield return null;
            monneyText.text = tempMonney.ToString();
        }
        totalMonney += monneyReward;
        monneyText.text = totalMonney.ToString();
        DataPlayer.SetMonneyValue(totalMonney);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(GameConst.Game_Scene);
    }

    private Vector3 GetTargetPoint()
    {
        Vector3 screenPoint = monneyIcon.position;
        screenPoint.z = 10f;
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPoint);
        worldPoint.z = 0f;
        return worldPoint;
    }
    private void MoveMonneyToTarget(Transform monney)
    {
        monney.DOMove(targetPoint, 1.5f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            Destroy(monney.gameObject);
        });
    }
    public void ShowButtonReward()
    {

        rewardMonneyButton.gameObject.SetActive(true);
        GetPassLevelMonneyButton.gameObject.SetActive(true);
        sliderValueReward.SetActive(true);
    }

    private void SpawnPointProgress(int level)
    {
        int progress = level % 9;
        if (progress == 0) progress = 9;
        for (int i = 0; i < 10; i++)
        {
            if (i + 1 < progress)
            {
                Instantiate(greenPointPrefab, parentPoint);
            }
            else if (i + 1 == progress)
            {
                greenPoint = Instantiate(greenPointPrefab, parentPoint);
                greenPoint.SetActive(false);
            }
            else if (i + 1 == progress + 1)
            {
                yellowPoint = Instantiate(yellowPointPrefab, parentPoint);
            }
            else
            {
                Instantiate(whitePointPrefab, parentPoint);
            }
        }
    }
    public void HideYellowPoint()
    {
        greenPoint?.SetActive(true);
        yellowPoint?.SetActive(false);
    }

    private void Reset()
    {
        for (int i = 0; i < PileOfMoneyParent.transform.childCount; i++)
        {
            PileOfMoneyParent.transform.GetChild(i).position = InitialPos[i];
            PileOfMoneyParent.transform.GetChild(i).rotation = InitialRotation[i];
        }
    }

    private void RewardPileOfMoney(int monney)
    {
        Reset();
        var delay = 0f;

        PileOfMoneyParent.SetActive(true);
        MusicController.instance.WaitPlayMoneyFly();
        for (int i = 0; i < PileOfMoneyParent.transform.childCount; i++)
        {
            PileOfMoneyParent.transform.GetChild(i).DOScale(1f, 0.3f).SetDelay(delay).SetEase(Ease.OutBack);

            PileOfMoneyParent.transform.GetChild(i).GetComponent<Transform>().DOMove(moneyTargetPosition, 1f).SetDelay(delay + 0.5f).SetEase(Ease.InBack);
            PileOfMoneyParent.transform.GetChild(i).DORotate(Vector3.zero, 0.5f).SetDelay(delay + 0.5f).SetEase(Ease.Flash);
            PileOfMoneyParent.transform.GetChild(i).DOScale(0f, 0.3f).SetDelay(delay + 1.8f).SetEase(Ease.OutBack);
            delay += 0.1f;
        }

        StartCoroutine(IECountMonney(monney, 1f));
    }


    private IEnumerator ShowRewardAd(Action<RewardVideoState> callback)
    {
        yield return null;
        AdManager.Instant.ShowRewardVideo(callback, true);
    }

    private void Callback_ShowReward(RewardVideoState state)
    {
        if (state == RewardVideoState.Watched)
        {
            doneReward = true;
            int rewardMonney = int.Parse(rewardValue.text);
            int monney = DataPlayer.GetMonneyValue() + rewardMonney;
            HideButton();
            RewardPileOfMoney(rewardMonney);
        }
        else if (state == RewardVideoState.Closed)
        {
            if (doneReward == false)
            {
                measure.ContinueMoveMeasureForward();
            }

        }
    }
}
