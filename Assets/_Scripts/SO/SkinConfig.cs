using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinConfig", menuName = "ScriptableObjects/SkinConfig")]
public class SkinConfig : ScriptableObject
{
    public List<Skin> skins;
    public List<int> valueLevelUnlockSkin;
    public List<ModeSKin> modeSkins;
    public List<Sprite> spriteSkin;
    public List<Sprite> spriteIconSkinWinPopup;

    public Material GetMaterialBody(int id)
    {
        return skins[id].materialBody;
    }    

    public Mesh GetMeshBody(int id)
    {
        return skins[id].meshBody;
    }
    public Material GetMaterialItem(int id)
    {
        return skins[id].materialItem; 
    }    
    public Mesh GetMeshItem(int id)
    {
        return skins[id].meshItem;
    }    

    public int GetValueLevelUnlockSkin(int id)
    {
        if (id < valueLevelUnlockSkin.Count)
        {
            return valueLevelUnlockSkin[id];
        }
        return valueLevelUnlockSkin[valueLevelUnlockSkin.Count - 1];
    }    
    public Sprite GetNextSpriteSkin(int level)
    {
        for(int i =1; i<= valueLevelUnlockSkin.Count;i++ )
        {
            if(level -1 <= valueLevelUnlockSkin[i])
                return spriteSkin[i];
        }
        return spriteSkin[0];
    }    
    public Sprite GetSpriteIconSkin(int id)
    {
        return spriteSkin[id];
    }    
    public Sprite GetSpriteIconSkinWinpopup(int id)
    {
        int length = spriteIconSkinWinPopup.Count;
        if (id < length)
        {
            return spriteIconSkinWinPopup[id];
        }
        return spriteIconSkinWinPopup[length - 1];
    }    
}

[System.Serializable]
public class Skin
{
    public Material materialBody;
    public Material materialItem;
    public Mesh meshBody;
    public Mesh meshItem;
}
public enum ModeSKin
{
    Normal,
    Epic,
    Rare
}
