using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundTextSO", menuName = "ScriptableObjects/SoundTextSO")]
public class SoundTextSO : ScriptableObject
{
    public List<Sprite> attackEnemy;
    public List<Sprite> brokenWoodenBox;
    public List<Sprite> fallingBall;
    public List<Sprite> rateText;
    public List<Sprite> detectPlayer;
    public Sprite GetSpriteAttackEnemy()
    {
        int rand = Random.Range(0,attackEnemy.Count);
        return attackEnemy[rand];
    }   
    public Sprite GetSpriteBrokenWoodenbox()
    {
        int rand = Random.Range(0,brokenWoodenBox.Count);
        return brokenWoodenBox[rand];
    }    

    public Sprite GetSpriteFallingBall()
    {
        int rand = Random.Range(0, fallingBall.Count);
        return fallingBall[rand];
    }
    public Sprite GetSpriteRateText()
    {
        int rand = Random.Range (0, rateText.Count);
        return rateText[rand];  
    }    
    public Sprite GetDetectPlayer()
    {
        int rand = Random.Range(0, detectPlayer.Count);
        return detectPlayer[rand];
    }    
}
