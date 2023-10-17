using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITextVfx : MonoBehaviour
{
    private SkeletonGraphic skeleton;
    private Spine.AnimationState spineAnimationState;
    [SpineAnimation] public string[] attackText;
    [SpineAnimation] public string[] brokenWoodenBox;
    [SpineAnimation] public string[] rateText;
    [SpineAnimation] public string fallStellBall;
    [SpineAnimation] public string note;
    private void OnEnable()
    {
        skeleton = GetComponent<SkeletonGraphic>();
        spineAnimationState = skeleton.AnimationState;
    }

    public void PlayAnimationAttackText()
    {
        SetScale(0.4f);
        int r = Random.Range(0, attackText.Length);
        spineAnimationState.SetAnimation(0, attackText[r],false);
    }    

    public void PlayAnimationbrokenWoodenBox()
    {
        SetScale(0.4f);
        int r = Random.Range(0,brokenWoodenBox.Length);
        spineAnimationState.SetAnimation(0, brokenWoodenBox[r],false);  
    }  
    
    public void PlayAnimationRateText()
    {
        SetScale(0.5f);
        int r = Random.Range(0, rateText.Length);
        spineAnimationState.SetAnimation(0, rateText[r],false);
        Invoke(nameof(HideText), 1.5f);
    }
    public void PlayAnimationFallStellBall()
    {
        SetScale(0.4f);
        spineAnimationState.SetAnimation(0, fallStellBall, false);
    }
    public void PlayAnimationNoteText()
    {
        SetScale(0.5f);
        spineAnimationState.SetAnimation(0, note, false);
    }
    private void SetScale(float scale)
    {
        this.gameObject.SetActive(true);
        this.gameObject.GetComponent<RectTransform>().localScale = new Vector3 (scale, scale, scale);
    }     

    public void SetPositionUITextVfx(Vector3 position ,bool direct)
    {
        if(direct)
        {
            this.transform.position = position;
        }   
        else
        {
            Vector3 screenPoint = GameManager.Instance.GetScreenPoint(position);
            if (screenPoint.y < 100f)
            {
                screenPoint.y = 100f;
                screenPoint.x = 540f;
            }
            this.transform.position = screenPoint;
        }    
    }
    private void HideText()
    {
        this.gameObject.SetActive (false);
    }
}
