using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DVAH;

public class InGamePopup : MonoBehaviour
{
    [SerializeField] Button replayButton;
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] TextMeshProUGUI levelText;

    private void OnEnable()
    {
        moneyText.text = DataPlayer.GetMonneyValue().ToString();
        levelText.text = "LEVEL "+DataPlayer.GetLevelValue().ToString();

        FireBaseManager.Instant.LogEventWithParameterAsync("gameplay_start", new Hashtable()
        {
            {
                "id_screen","GAMEPLAY"
            }
        });
    }
    private void Start()
    {
        replayButton.onClick.AddListener(() =>
        {
            SoundFXManager.Instance.PlayClickButton();
            GameManager.Instance.TapVibrate();
            SceneManager.LoadScene(GameConst.Game_Scene);
            UIController.Instance.HideUIHome();

        });
    }
}
