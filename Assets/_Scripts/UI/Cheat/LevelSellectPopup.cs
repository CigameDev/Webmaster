using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSellectPopup : MonoBehaviour
{
    [SerializeField] private  GameObject buttonPrefab;
    [SerializeField] private Transform content;
    private void OnEnable()
    {
        
    }

    private void Start()
    {
        for(int i = 0;i < 50;i++)
        {
            var button = Instantiate(buttonPrefab, content);
            ButtonLevel buttonLevel = button.GetComponent<ButtonLevel>();
            buttonLevel.ID = i + 1;
            buttonLevel.DisPlayTextLevel();
        }
    }
}
