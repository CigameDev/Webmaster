using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// cong tac dien
/// </summary>
public class Switch : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private int idSwitch;
    public int IDSwitch => idSwitch;
    
    private bool isOpen;
    public bool IsOpen=>isOpen;

    private void Start()
    {
        GameEvent.instance.OnOpenGate += RegisterEvent_OnOpenGate;
    }

    private void RegisterEvent_OnOpenGate(int idGate)
    {
        if(idGate == this.idSwitch)
        {
            if(this.isOpen ==false)//lan dau dang dong,neu cham se mo ra
            {
                this.isOpen = true;
            }
            else
            {
                this.isOpen=false;
            }
            animator.SetBool("Open", isOpen);
        }
    }

    private void OnDestroy()
    {
        GameEvent.instance.OnOpenGate -= RegisterEvent_OnOpenGate;
    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!isOpen)
    //    {
    //        isOpen = true;
    //        animator.SetBool("Open", true);
    //    }
    //    else
    //    {
    //        isOpen = false;
    //        animator.SetBool("Open", false);
    //    }
    //}
}
