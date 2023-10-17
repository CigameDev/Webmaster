using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button Common;
    [SerializeField] private Button Rare;
    [SerializeField] private Button Epic;

    [Header("Popup")]
    [SerializeField] private GameObject commonPopup;
    [SerializeField] private GameObject rarePopup;
    [SerializeField] private GameObject epicPopup;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 120;
        InitButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitButtons()
    {
        Common.onClick.AddListener(TurnOnCommonPopup);
        Rare.onClick.AddListener(TurnOnRarePopup);
        Epic.onClick.AddListener(TurnOnEpicPopup);
    }

    private void TurnOnCommonPopup()
    {
        commonPopup.SetActive(true);
        rarePopup.SetActive(false);
        epicPopup.SetActive(false);
    }

    private void TurnOnRarePopup()
    {
        commonPopup.SetActive(false);
        rarePopup.SetActive(true);
        epicPopup.SetActive(false);
    }

    private void TurnOnEpicPopup()
    {
        commonPopup.SetActive(false);
        rarePopup.SetActive(false);
        epicPopup.SetActive(true);
    }
}
