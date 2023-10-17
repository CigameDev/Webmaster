using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Measure : MonoBehaviour
{
    private RectTransform rectTransform;
    private int baseRewardAmount = 50;

    protected float timeToMove = 1.5f;
    [SerializeField] protected TextMeshProUGUI rewardText;
    private Vector2 previousAnchorPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        InvokeRepeating(nameof(MoveMeasureForward), 0, timeToMove * 2f);
        previousAnchorPosition = GetAnchorPosition(rectTransform);
    }
    private bool isScaled = false;
    private float prePosX;

    void Update()
    {
       
        Vector2 anchorPosition = GetAnchorPosition(rectTransform);

        if (anchorPosition.x > 50 && anchorPosition.x < 100)
        {
            if (rewardText.text != (baseRewardAmount * 2).ToString())
            {
                rewardText.text = (baseRewardAmount * 2).ToString();
                rewardText.rectTransform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), .3f)
                .OnComplete(() => rewardText.rectTransform.localScale = new Vector3(1, 1, 1));
            }
        }

        else if (anchorPosition.x > 100 && anchorPosition.x < 200)
        {
            if (rewardText.text != (baseRewardAmount * 3).ToString())
            {
                rewardText.text = (baseRewardAmount * 3).ToString();
                rewardText.rectTransform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), .3f)
                .OnComplete(() => rewardText.rectTransform.localScale = new Vector3(1, 1, 1));
            }
        }

        else if (anchorPosition.x > 200 && anchorPosition.x < 300)
        {
            if (rewardText.text != (baseRewardAmount * 4).ToString())
            {
                rewardText.text = (baseRewardAmount * 4).ToString();
                rewardText.rectTransform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), .3f)
                .OnComplete(() => rewardText.rectTransform.localScale = new Vector3(1, 1, 1));
            }
        }

        else if (anchorPosition.x > 300 && anchorPosition.x < 400)
        {
            if (rewardText.text != (baseRewardAmount * 5).ToString())
            {
                rewardText.text = (baseRewardAmount * 5).ToString();
                rewardText.rectTransform.DOScale(new Vector3(1.3f, 1.3f, 1.5f), .3f)
                .OnComplete(() => rewardText.rectTransform.localScale = new Vector3(1, 1, 1));
            }
        }

        else if (anchorPosition.x > 400 && anchorPosition.x < 500)
        {
            if (rewardText.text != (baseRewardAmount * 4).ToString())
            {
                rewardText.text = (baseRewardAmount * 4).ToString();
                rewardText.rectTransform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), .3f)
                .OnComplete(() => rewardText.rectTransform.localScale = new Vector3(1, 1, 1));
            }
        }

        else if (anchorPosition.x > 500 && anchorPosition.x < 600)
        {
            if (rewardText.text != (baseRewardAmount * 3).ToString())
            {
                rewardText.text = (baseRewardAmount * 3).ToString();
                rewardText.rectTransform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), .3f)
                .OnComplete(() => rewardText.rectTransform.localScale = new Vector3(1, 1, 1));
            }
        }

        else if (anchorPosition.x > 600)
        {
            if (rewardText.text != (baseRewardAmount * 2).ToString())
            {
                rewardText.text = (baseRewardAmount * 2).ToString();
                rewardText.rectTransform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), .3f)
                .OnComplete(() => rewardText.rectTransform.localScale = new Vector3(1, 1, 1));
            }
        }
    }

    void MoveMeasureForward()
    {
        rectTransform.DOAnchorPosX(650, timeToMove).OnComplete(MoveBack);
    }

    void MoveBack()
    {
        rectTransform.DOAnchorPosX(50, timeToMove);

    }
    public void StopMoveMeasureForward()
    {
        CancelInvoke(nameof(MoveMeasureForward));
        rectTransform.DOKill();
    }    
    public void ContinueMoveMeasureForward()
    {
        InvokeRepeating(nameof(MoveMeasureForward), 0, timeToMove * 2);
    }    
    private Vector2 GetAnchorPosition(RectTransform rectTransform)
    {
        Vector2 anchorPosition = rectTransform.anchoredPosition;
        return anchorPosition;
    }
}
