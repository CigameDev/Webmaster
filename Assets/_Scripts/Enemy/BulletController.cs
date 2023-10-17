using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : EnemyWeapon
{
    [SerializeField] float speed;

    protected override void TriggerEnterPlayer(Transform playerTransform)
    {
        Player player = playerTransform.GetComponent<Player>();
        player.OnDie();

        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        transform.Translate(speed * Time.fixedDeltaTime  * Vector3.up);
    }
}
