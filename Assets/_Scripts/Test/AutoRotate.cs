using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AutoRotate : MonoBehaviour
{
    [SerializeField] AnimationClip spineL, spineR;
    [SerializeField] Transform target;
    [SerializeField] TwoBoneIKConstraint left, leftAim, right, rightAim;

    Animator animator;
    bool isRight;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float xTarget = target.position.x;
        if(xTarget < transform.position.x && isRight)
        {
            isRight = false;
            animator.SetTrigger("left");
        }else if (xTarget > transform.position.x && !isRight)
        {
            left.weight = 0f;
            right.weight = 1f;

            isRight = true;
            animator.SetTrigger("right");
        }

        if (!isRight)
        {
            left.weight += Time.deltaTime;
            leftAim.weight += Time.deltaTime;

            right.weight -= Time.deltaTime * 4f;
            rightAim.weight -= Time.deltaTime * 4f;
        }
        else
        {
            left.weight -= Time.deltaTime * 4f;
            leftAim.weight -= Time.deltaTime * 4f;

            right.weight += Time.deltaTime;
            rightAim.weight += Time.deltaTime;
        }
    }
}
