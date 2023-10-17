using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColliderCenterPlayerSO", menuName = "ScriptableObjects/ColliderCenterPlayerSO")]
public class ColliderCenterPlayerSO : ScriptableObject
{
    public Vector3 idle_L;
    public Vector3 idle_R;
    public Vector3 ngoi_L;
    public Vector3 ngoi_R;
    public Vector3 posCol_Idle;
    public Vector3 kick_L;
    public Vector3 kick_R;
    public Vector3 kick_L2;
    public Vector3 kick_R2;
    public Vector3 shoot;
}
