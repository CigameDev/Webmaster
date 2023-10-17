using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ShieldController : EnemyWeapon
{
    [SerializeField] Animator thiefAnimator;
    [SerializeField] float forcePushPlayer;

    BoxCollider boxCollider;
    Rigidbody rb;

    bool isSeted;

    protected override void TriggerEnterPlayer(Transform playerTransform)
    {
        //Player player = playerTransform.GetComponent<Player>();
        ////if (player.OrderCollisionShield == 1)
        ////{
        ////    thiefAnimator.SetTrigger("guard");
        ////    PushPlayer();
        ////    Debug.Log("Bi day nguoc lai");
        ////}
        //GameManager.Instance.isShieldDetector = true;
        //thiefAnimator.SetTrigger("guard");
        //PushPlayer();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            player.SetTriggerCollider(true);
            Vector3 directionTurnback = player._directionLine * (-1f);
            RaycastHit hit;
            if(Physics.Raycast(new Vector3(transform.position.x,transform.position.y,0f),directionTurnback, out hit,50f,1<<6)) 
            {
                player.pointStopTurnBack = hit.point;
            }
            GameManager.Instance.isShieldDetector = true;//da va cham voi Shield truoc
            thiefAnimator.SetTrigger("guard");
            
            PushPlayer();
            player.timeStartTurnBack = Time.time;
        }
    }
   
    protected override void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();

        base.Start();
    }

    private void Update()
    {
        if (isSeted) return;

        boxCollider.enabled = true;
        rb.isKinematic = false;
        isSeted = true;
    }

    void PushPlayer()
    {
        Player player = playerTransform.GetComponent<Player>();
        Vector3 direction = player._directionLine * (-1);
        player.isFlyBack = true;
        Rigidbody playerRb = playerTransform.GetComponent<Rigidbody>();
        player.ReverseTurn();
        player.PlayAnimDangling();
        playerRb.AddForce(direction * forcePushPlayer/10f,ForceMode.Impulse);
        //playerRb.velocity = direction * 10f;
        SoundFXManager.Instance.PlayShield();

    }
    public void ChangeLayerRagdoll()
    {
        this.gameObject.layer = LayerMask.NameToLayer(GameConst.Ragdoll_Layer);
    }
}
