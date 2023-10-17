using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemWatchAds : MonoBehaviour
{
    public Button button;
    public CommonPopup commonPopup;

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
        contentController.iTurnOnButton.TurnOnUnlockAdsItemButton();
        /*CommonPopup.Instance.OnClickItem(GetComponentInParent<ItemController>());
        CommonPopup.Instance.TurnOnUnlockAdsItemButton();*/
    }
}
