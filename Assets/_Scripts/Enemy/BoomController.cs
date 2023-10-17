using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoomController : EnemyWeapon
{
    [HideInInspector] public Transform parentEnemy;
    [HideInInspector] public Vector3 direction;

    [SerializeField] float speed;
    [SerializeField] float duration;

    Vector3 posEnemy;
    bool isBack;
    Rigidbody rb;
    bool killedEnemy;
    Transform boomTransform;
    private void Awake()
    {
        boomTransform = this.transform;
    }
    protected override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody>();
        posEnemy = parentEnemy.position;
        posEnemy.y += 0.5f;

        Vector3 randomTorque = new(Random.value, Random.value, Random.value);
        rb.AddTorque(randomTorque * 1000f);
    }

    private void Update()
    {
        //transform.position += speed * Time.deltaTime  * direction;
        boomTransform.position += speed * Time.deltaTime * direction;
    }
    private void DestroyBoom()
    {
        Destroy(gameObject);
    }    
    protected override void TriggerEnterPlayer(Transform playerTransform)
    {
        Player player = playerTransform.GetComponent<Player>();

        if (player.flying)
        {
            isBack = true;
            direction = parentEnemy.position - transform.position;
            direction.y += 0.5f;
            direction = direction.normalized;

            speed *= 5f;
        }
        else if(DataPlayer.alldata.isNormalMap)
        {
            player.OnDie();
            GameManager.Instance.PlayVfxExplosion(this.transform.position);
            Invoke(nameof(DestroyBoom), 0.15f);
        }
    }
   
    protected override void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(!isBack)
            {
                TriggerEnterPlayer(playerTransform);
                Player player = other.GetComponent<Player>();
                if (!player.flying)
                {
                    SoundFXManager.Instance.PlayBoom();
                }
            }

        }    
        else if(other.CompareTag("Enemy"))
        {
            if(isBack && !killedEnemy  )
            {
                EnemyBase enemyBase = parentEnemy.GetComponent<EnemyBase>();
                if (enemyBase.IsAlive)
                {
                    killedEnemy = true;//tranh truong hop ham nay goi 2 lan
                    enemyBase.Death(transform);
                    GameManager.Instance.PlayVfxExplosion(this.transform.position);
                    SoundFXManager.Instance.PlayBoom();
                    Invoke(nameof(DestroyBoom), 0.15f);
                }
            }
        }    
        else if(other.CompareTag("Wall")||other.CompareTag("Ground")||other.CompareTag("Top"))
        {
            GameManager.Instance.PlayVfxExplosion(boomTransform.position);
            SoundFXManager.Instance.PlayBoom();
            Invoke(nameof(DestroyBoom), 0.15f);
        }    
    }
}
