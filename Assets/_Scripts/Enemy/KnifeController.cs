using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class KnifeController : EnemyWeapon
{
    BoxCollider boxCollider;
    bool isSafe;

    protected override void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        isSafe = true;

        base.Start();
        //do moi vao game tat ragdoll nen vo tinh chung tat luon collider cua con dao ,can bat lai de dam player.Nhung ragdoll o start nen can doi 0.1s hay bat collider cua dao
        Invoke(nameof(EnableBoxColliderKnife), 0.1f);
    }

    public void ActiveAttack()
    {
        isSafe = false;
    }

    public void DisableAttack()
    {
        isSafe = false;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (isSafe) return;
        base.OnTriggerEnter(other);
    }

    protected override void TriggerEnterPlayer(Transform playerTransform)
    {
        Player player = playerTransform.GetComponent<Player>();
        player.OnDie();
    }

    private void EnableBoxColliderKnife()
    {
        boxCollider.enabled = true;
    }
}
