using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefShieldController : EnemyBase
{
    [SerializeField]ShieldController shieldController;
    protected override void Start()
    {
        base.Start();

        OnChangeMoveTarget += OffGuard;
    }

    void OffGuard()
    {
        animator.SetBool("guard", false);
    }

    protected override void Attack()
    {
        animator.SetBool("walk", false);
    }
    public void ShieldFell()
    {
        //lam cho khien khong va cham voi player nua
        shieldController?.ChangeLayerRagdoll();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (!isAlive) return;
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();//neu da va cham voi shiled truoc thi khong cho chet
            if (player.firstRaycast != FirstRaycast.Shield && GameManager.Instance.isShieldDetector == false)
            {
                GameManager.Instance.uiTextVfx.PlayAnimationAttackText();
                GameManager.Instance.uiTextVfx.SetPositionUITextVfx(other.transform.position, false);
                if (player.canKickEnemy)
                {
                    GameManager.Instance.SlowmotionKillEnemy(0.1f, 0.75f);
                    player.canKickEnemy = false;
                    player.enemyBase = null;
                }
                GetDamage();
                SoundFXManager.Instance.PlayEnemyAttack();
                player.firstRaycast = FirstRaycast.Otherthing;
            }
            GameManager.Instance.isShieldDetector = false;
        }
        else if (other.CompareTag("Death Trap"))
        {
            Death(other.transform);
            GameManager.Instance.SlowmotionKillEnemy(0.1f, 0.75f);
        }
    }
    protected override void OnCollisionEnter(Collision col)
    {
        if (!isAlive) return;
        //if (col.collider.CompareTag("Player"))
        //{
        //    Player player = col.collider.GetComponent<Player>();
        //    col.collider.isTrigger = true;
        //    Debug.Log("va chm voi player va bi chet");
        //}
        if (col.collider.CompareTag("Death Trap"))
        {
            Death(playerTransform);
            GameManager.Instance.SlowmotionKillEnemy(0.1f, 0.75f);
        }
    }
}
