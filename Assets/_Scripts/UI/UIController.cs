using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header ("POPUPS:")]
    [SerializeField] private GameObject UIHome;
    [SerializeField] private GameObject Ingame;
    [SerializeField] private GameObject SettingPopup;
    [SerializeField] private GameObject TutorialPopup;
    [SerializeField] private GameObject EndTutorialPopup;
    [SerializeField] private GameObject SkinPopup;
    [SerializeField] private GameObject WinPopup;
    [SerializeField] private GameObject FailPopup;
    [SerializeField] private GameObject NewAvatar;
    [SerializeField] private Image backGround;
    [SerializeField] BackGroundSO backGroundSO;

    [SerializeField] private GameObject LevelSelect;

    public static UIController Instance { get;  set; }
    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        ChangeBG();
    }


    public void ShowUIHome()
    {
        UIHome.gameObject.SetActive(true);
    }
    public void HideUIHome()
    {
        UIHome.gameObject.SetActive(false);
    }    
    public void ShowIngamePopup()
    {
        Ingame.gameObject.SetActive(true);
    }    
    public void HideIngamePopup()
    {
        Ingame.gameObject.SetActive(false);
    }    
    public void ShowSettingPopup()
    {
        SettingPopup.gameObject.SetActive(true);
    }

    public void ShowTutorialPopup()
    {
        TutorialPopup.gameObject.SetActive(true);
    }

    public void ShowSkinPopup()
    {
        SkinPopup.gameObject.SetActive(true);
    }

    public void ShowWinPopup()
    {
        WinPopup.gameObject.SetActive(true);
    }

    private void ShowFailPopup()
    {
        FailPopup.gameObject.SetActive(true);
    }
    public void InvokeShowFailPopUp()
    {
        Invoke(nameof(ShowFailPopup), 1f);
    }    
    public void HideFailPopup()
    {
        FailPopup.gameObject.SetActive(false);
    }    
    public void ShowNewAvatarPopup()
    {
        NewAvatar.gameObject.SetActive(true);
    }

    public void LoadSceneAgain()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void ChangeBG()
    {
        int level = DataPlayer.GetLevelValue();
        backGround.sprite = backGroundSO.GetSpriteBGByLevel(level);
    }    

    private void ShowEndTutorial()
    {
        EndTutorialPopup.gameObject.SetActive(true);
    }    

    public void WaitShowEndTutorial()
    {
        Invoke(nameof(ShowEndTutorial), 1.5f);
    }    

    public void DisplayLevelSelect()
    {
        LevelSelect.SetActive(true);
    }    
}
