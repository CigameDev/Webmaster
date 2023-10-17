using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    [SerializeField] private int idGate;
    [SerializeField] private Animator animator;
    private bool isOpen;


    private void Start()
    {
        GameEvent.instance.OnOpenGate += RegisterEvent_OnOpenGate;
    }

    private void RegisterEvent_OnOpenGate(int obj)
    {
        if(obj == this.idGate)
        {
            isOpen = !isOpen;
            animator.SetBool("Open", isOpen);
            SoundFXManager.Instance.PlayGate();
        }
    }

    private void OnDestroy()
    {
        GameEvent.instance.OnOpenGate -= RegisterEvent_OnOpenGate;
    }
}
