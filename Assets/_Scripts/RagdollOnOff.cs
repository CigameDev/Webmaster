using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollOnOff : MonoBehaviour
{
    public CapsuleCollider mainCollider;
    public GameObject thisGuyRig;//lay cai skeletonOutlaw-Rig
    public Animator thisGuyAnimator;
    Collider[] ragDollColliders;
    Rigidbody[] limbsRigidbodies;
    private void Start()
    {
        GetRagdollBits();
        RagdollModeOff();
        //RagdollModeOn();
    }
    private void Update()
    {
        
    }
   
    private void GetRagdollBits()
    {
        ragDollColliders = thisGuyRig.GetComponentsInChildren<Collider>();
        limbsRigidbodies = thisGuyRig.GetComponentsInChildren<Rigidbody>();
    }    
    public void RagdollModeOn()
    {
        thisGuyAnimator.enabled = false;

        foreach (Collider col in ragDollColliders)
        {
            col.enabled = true;
        }
        foreach (Rigidbody rigid in limbsRigidbodies)
        {
            rigid.isKinematic = false;
        }
        mainCollider.enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }    

    public void RagdollModeOff()
    {
        foreach(Collider col in ragDollColliders)
        {
            col.enabled = false;
        }    
        foreach(Rigidbody rigid in limbsRigidbodies)
        {
            rigid.isKinematic = true;
        }    
        //thisGuyAnimator.enabled = true;
        //mainCollider.enabled = true;
        GetComponent<Collider>().enabled = true;
        GetComponent<Animator>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
    }    
    public void RagdollNoGravity()
    {
        thisGuyAnimator.enabled = false;

        foreach (Collider col in ragDollColliders)
        {
            col.enabled = true;
        }

        foreach (Rigidbody rigid in limbsRigidbodies)
        {
            rigid.isKinematic = false;
            rigid.useGravity = false;
        }
        mainCollider.enabled = false;
        GetComponent<Rigidbody>().isKinematic = false;
    }    
}
