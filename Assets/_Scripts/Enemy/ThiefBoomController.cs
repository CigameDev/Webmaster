using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefBoomController : EnemyBase
{
    [SerializeField] GameObject boomPrefab;
    [SerializeField] Transform rightHand;

    protected override void Attack()
    {
        animator.SetTrigger("throw");
    }

    void ThrowBoom()
    {

        GameObject boom = Instantiate(boomPrefab, rightHand.position, Quaternion.identity);

        Player player = playerTransform.GetComponent<Player>();
        Vector3 playerPos = playerTransform.position;
        playerPos.y += player.IsStandStraight() ? 0.5f : -0.5f;

        Vector3 direction = (playerPos - boom.transform.position).normalized;
        direction.z = 0;

        BoomController boomController = boom.GetComponent<BoomController>();
        boomController.direction = direction;
        boomController.parentEnemy = transform;
    }
}
