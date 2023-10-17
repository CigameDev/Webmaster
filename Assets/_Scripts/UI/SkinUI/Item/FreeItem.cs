using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FreeItem : MonoBehaviour
{
    public Button button;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponentInChildren<Button>();
        button.onClick.AddListener(OnClickButton);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnClickButton()
    {
        ContentController contentController = GetComponentInParent<ContentController>();

        contentController.iGetItemId.GetIdItem(GetComponentInParent<ItemController>());
        //CommonPopup.Instance.OnClickItem(GetComponentInParent<ItemController>());
        contentController.iTurnOnButton.TurnOffAll();
    }
}
