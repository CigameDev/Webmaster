using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    private Animator animator;
    [SerializeField] SkinnedMeshRenderer meshBody;
    [SerializeField] SkinnedMeshRenderer meshItem;
    [SerializeField] AnimationClip[] danceClips;
    [SerializeField] SkinConfig skinConfig;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
   
    public void PlayRandomClip()
    {
        int r = Random.Range(0,danceClips.Length);
        if(animator ==null)
        {
            animator = GetComponent<Animator>();
        }    
        animator.Play(danceClips[r].name);
    }    
    public void ChangeSkin(int id)
    {
        meshBody.sharedMesh = skinConfig.GetMeshBody(id);
        meshBody.material = skinConfig.GetMaterialBody(id);
        meshItem.sharedMesh = skinConfig.GetMeshItem(id);
        meshItem.material = skinConfig.GetMaterialItem(id);
    }    
}
