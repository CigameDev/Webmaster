using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DVAH;

public class EndTutorialPopup : MonoBehaviour
{
    [SerializeField] Button replayButton;
    [SerializeField] Button continueButton;

    private void OnEnable()
    {
        FireBaseManager.Instant.LogEventWithParameterAsync("tutorial_finish", new Hashtable()
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
        replayButton.onClick.AddListener(OnclickReplayButton);
        continueButton.onClick.AddListener(OnclickContinueButton);
    }

    private void OnclickContinueButton()
    {
        FireBaseManager.Instant.LogEventWithParameterAsync("tutorial_continue", new Hashtable()
        {
            {
                "id_screen","TUTORIAL"
            },
            {
                "id_level",DataPlayer.GetLevelValue()
            }
        });


        // chuyen sang man choi that
        int indexVideoCurrent = DataPlayer.alldata.indexVideoCurrent;
        indexVideoCurrent++;
        DataPlayer.SetIndexVideoCurrent(indexVideoCurrent);
        SoundFXManager.Instance.PlayClickButton();
        GameManager.Instance.TapVibrate();
        DataPlayer.SetIsNormalMap(true);
        SceneManager.LoadScene(GameConst.Game_Scene);
    }

    private void OnclickReplayButton()
    {
        // choi lai man tutorial nay
        SoundFXManager.Instance.PlayClickButton();
        GameManager.Instance.TapVibrate();
        SceneManager.LoadScene(GameConst.Game_Scene);

        FireBaseManager.Instant.LogEventWithParameterAsync("tutorial_replay", new Hashtable()
        {
            {
                "id_screen","TUTORIAL"
            },
            {
                "id_level",DataPlayer.GetLevelValue()
            }
        });
    }
}
