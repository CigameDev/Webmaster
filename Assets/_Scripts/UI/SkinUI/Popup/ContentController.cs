using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentController : MonoBehaviour
{
    [SerializeField]
    public GameObject tabObject;

    public IGetIdItem iGetItemId;
    public ITurnOnButton iTurnOnButton;

    private void Awake()
    {
        iGetItemId = tabObject.GetComponent<IGetIdItem>();
        iTurnOnButton = tabObject.GetComponent<ITurnOnButton>();
    }
}
