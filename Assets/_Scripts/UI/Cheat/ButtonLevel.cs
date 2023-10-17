using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ButtonLevel : MonoBehaviour
{
    public int ID { get; set; }
    [SerializeField] private Button buttonLevel;
    [SerializeField] private TextMeshProUGUI levelText;
    
    private void Start()
    {
        buttonLevel.onClick.AddListener(OnClickButton);
    }

    private void OnClickButton()
    {
        DataPlayer.SetLevelValue(ID);
        SceneManager.LoadScene(GameConst.Game_Scene);
    }

    public void DisPlayTextLevel()
    {
        levelText.text = ID.ToString();
    }
}
