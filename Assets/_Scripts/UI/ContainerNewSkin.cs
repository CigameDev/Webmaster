using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContainerNewSkin : MonoBehaviour
{
    [SerializeField] private Image newSkinImage;
    [SerializeField] private Image processNewSkinImage;
    [SerializeField] private TextMeshProUGUI processNewSkinText;
    [SerializeField] SkinConfig skinConfig;
    [SerializeField] WinPopup winPopup;

    float preProcessValue;
    float targetProcessValue;
    int levelValue;
    int levelUnlockPre;
    int levelUnlockNext;
    int unlockProgress;
    int lastLevelUnlock = 30;
    private void Awake()
    {
        levelValue = DataPlayer.GetLevelValue();

        if (levelValue > lastLevelUnlock +1)
        {
            winPopup.ShowButtonReward();
            return;
        }

        preProcessValue = DataPlayer.alldata.valueProcessNewSkin;
        processNewSkinImage.fillAmount = preProcessValue;
        unlockProgress = DataPlayer.alldata.lastValueSkinUnlocked;
        levelUnlockPre = skinConfig.GetValueLevelUnlockSkin(unlockProgress);
        levelUnlockNext = skinConfig.GetValueLevelUnlockSkin(unlockProgress + 1);
        targetProcessValue = (levelValue - levelUnlockPre - 1) * 1.0f / (levelUnlockNext - levelUnlockPre);
    }

    void Start()
    {
        if (levelValue > lastLevelUnlock +1) return;
        StartCoroutine(LoadProcess(targetProcessValue));
    }

    private IEnumerator LoadProcess(float target)
    {
        while (preProcessValue < target)
        {
            processNewSkinImage.fillAmount += Time.deltaTime/3f;
            preProcessValue += Time.deltaTime/3f;
            processNewSkinText.text = ((int)(preProcessValue * 100)).ToString() +"%";
            yield return null;
        }
        if(target ==1f)target = 0f;
        DataPlayer.SetValueProcessNewSkin(target);
        if (targetProcessValue >= 1f)
        {
            unlockProgress++;
            DataPlayer.SetLastValueSkinUnlocked(unlockProgress);
            yield return new WaitForSeconds(0.5f);
            UIController.Instance.ShowNewAvatarPopup();
            winPopup.ShowButtonReward();
        }
        else
        {
            winPopup.ShowButtonReward();
        }
    }    
}
