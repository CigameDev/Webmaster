using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefKnifeController : EnemyBase
{
    [SerializeField] KnifeController knifeController;

    protected override void Start()
    {
        base.Start();

        OnStopMove += () =>
        {
            animator.SetBool("walk", false);
        };
    }

    void ActiveKnife()
    {
        knifeController.ActiveAttack();
    }

    void DisableKnife()
    {
        knifeController.DisableAttack();
    }

    protected override void Attack()
    {
        animator.SetTrigger("attack");
    }
}
