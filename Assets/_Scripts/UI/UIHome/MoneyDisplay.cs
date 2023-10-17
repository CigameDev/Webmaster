using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyDisplay : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI money_text;

    private void Start()
    {
        //money_text.text = InGamePref.GetMoney().ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
