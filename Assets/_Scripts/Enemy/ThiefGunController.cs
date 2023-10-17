using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefGunController : EnemyBase
{
    [Header ("===== Custom =====")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnTransform;

    protected override void Start()
    {
        base.Start();

        OnStopMove += () =>
        {
            animator.SetBool("walk", false);
        };
    }

    protected override void Attack()
    {
        FireBullet();
    }

    void FireBullet()
    {
        SoundFXManager.Instance.PlayShoot();
        Vector3 positionBullet = bulletSpawnTransform.position;
        Quaternion rotBullet = bulletSpawnTransform.rotation;

        Instantiate(bulletPrefab, positionBullet, rotBullet);
    }
}
