using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BackGroundSO", menuName = "ScriptableObjects/BackGroundSO")]
public class BackGroundSO : ScriptableObject
{
    public List<Sprite> listSpriteBG;

    public Sprite GetSpriteBGByLevel(int levelValue)
    {
        
        int value = levelValue / 9;
        if (levelValue % 9 == 0) value--;
        int id = value % 2;
        return listSpriteBG[id];
    }    
}
