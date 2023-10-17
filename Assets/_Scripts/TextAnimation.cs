using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextAnimation : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;
    private Spine.AnimationState spineAnimationState;

    [SpineAnimation] public string note;

    private void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        spineAnimationState = skeletonAnimation.AnimationState;
    }


    public void PlayAnimationNoteText()
    {
        spineAnimationState.SetAnimation(0, note, false);
    }    
    public void SetPositionParent(Vector3 pos)
    {
        Transform parent = GetComponentInParent<Transform>();
        parent.position = pos;
    }    
}
