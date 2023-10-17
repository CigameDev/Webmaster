using Spine.Unity;
using UnityEngine;

public class SpiderWeb : MonoBehaviour
{
    private SkeletonAnimation ske;
    private  Spine.AnimationState spineAnimationState;
    [SpineAnimation]
    public string webInto;
    [SpineAnimation]
    public string webEnd;
    private void OnEnable()
    {
        ske = GetComponent<SkeletonAnimation>();
        spineAnimationState = ske.AnimationState;
    }

    public void PlayAnimationInto()
    {
        spineAnimationState.SetAnimation(0, webInto, false);
    }
    public void PlayAnimationEnd()
    {
        if (spineAnimationState != null)
        {
            spineAnimationState.SetAnimation(0, webEnd, false);
        }
    }
}
