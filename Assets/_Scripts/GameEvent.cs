using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent : MonoBehaviour
{
    public static GameEvent instance;

    private void Awake()
    {
        instance = this;
    }


    public event Action OnWinGame;
    public void PostEvent_WinGame()
    {
        OnWinGame?.Invoke();
    }


    public event Action<int> OnChangeSkin;
    public void PostEvent_ChangeSkin(int idSkin)
    {
        OnChangeSkin?.Invoke(idSkin);
    }


    public event Action<int> OnGetMonney;
    public void PostEvent_GetMonney(int valueMoney)
    {
        OnGetMonney?.Invoke(valueMoney);
    }


    public event Action<bool> OnTurnOnLaser;
    public void PostEvent_TurnLaser(bool turnLaser)
    {
         OnTurnOnLaser?.Invoke(turnLaser);
    }

    /// <summary>
    /// hook va cham voi Switch thi phat ra su kien 
    /// </summary>
    public event Action<int> OnOpenGate;
    public void PostEvent_Opengate(int idGate)
    {
        OnOpenGate?.Invoke(idGate);
    }

    public event Action OnRevive;
    public void PostEvent_OnRevive()
    {
        OnRevive?.Invoke();
    }

}
