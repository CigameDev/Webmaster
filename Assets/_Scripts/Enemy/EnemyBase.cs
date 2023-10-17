using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public abstract class EnemyBase : MonoBehaviour
{
    private float pullSpeedEnemy = 10f;
    #region Attack Parameter Variable
    [Header("===== Attack =====")]
    [Tooltip("Kẻ địch sẽ tấn công ngay lập tức khi player vào tầm ngắm lần đầu")]
    [SerializeField] protected bool quickAttack;

    [Tooltip("Kẻ địch có 1 tấm chắn bảo vệ khỏi đòn tấn công đầu tiên")]
    [SerializeField] protected bool barricade;

    [Tooltip("Thời gian hồi đòn đánh")]
    [SerializeField] protected float delayAttack;

    [Tooltip("Tốc độ xoay người khi người chơi lọt vào tầm ngắm")]
    [SerializeField] protected float speedTurnToPlayer;

    //Bộ đếm tung đòn đánh
    float countDelayAttack;
    #endregion

    #region Get Attack Parameter Variable
    [Tooltip("Lực văng khi enemy chết")]
    [SerializeField] protected float force;

    //Trạng thái sống
    protected bool isAlive;
    public bool IsAlive => isAlive;

    //Các sự kiện khi trạng thái thay đổi
    protected delegate void OnStateChange();
    protected OnStateChange OnDeath, OnBrokeBarricade;
    #endregion

    #region Moving Parameter Variable
    [Header("===== Moving =====")]
    [Tooltip("Tốc độ di chuyển")]
    [SerializeField] protected float speed;

    [Tooltip("Khoảng cách di chuyển qua lại so với vị trí ban đầu")]
    [SerializeField] protected float unitMove;

    [Tooltip("Nếu chọn thì khi di chuyển qua lại kẻ địch sẽ di chuyển sang bên trái trước")]
    [SerializeField] protected bool moveLeftFirst;

    [Tooltip("Thời gian chờ quay đầu lại khi đi hết 1 bên")]
    [SerializeField] protected float waitForTurnAround;

    [Tooltip("Tốc độ xoay người khi di chuyển bình thường")]
    [SerializeField] protected float speedMoveTurnAround;

    //Trạng thái di chuyển
    protected bool isMoving;

    //Vị trí khi bắt đầu game
    protected Vector3 firstPosition;

    //Vị trí mục tiêu đang di chuyển tới
    protected Vector3 target;

    //Khoảng cách để kẻ địch chuyển hướng đi
    float distanceTargetToChangeDirection;

    //Các sự kiện di chuyển
    protected delegate void OnMovingState();
    protected OnMovingState OnStopMove, OnBeforeUpdateMove, OnAfterUpdateMove, OnChangeMoveTarget;

    #endregion

    #region Rigging Parameter Variable
    [Header("===== Rigging =====")]
    [SerializeField] protected Rig[] rigs;
    [SerializeField] protected Transform[] rigTargets;
    [SerializeField] protected float speedTargetToPlayer;

    bool onTarget;
    #endregion

    #region Common Variable
    //Thông tin của player
    [Header("===== Common Information =====")]
    [SerializeField] Rigidbody root;
    [SerializeField] Collider radaCollider;

    protected Transform playerTransform;
    protected Animator animator;

    Rigidbody rb;
    Collider[] enemyCol;

    Rigidbody[] ragdollRb;

    Collider[] ragdollCol;

    Transform obj;
    #endregion

    #region Data Processing
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        enemyCol = GetComponents<Collider>();

        ragdollRb = GetComponentsInChildren<Rigidbody>();
        ragdollCol = GetComponentsInChildren<Collider>();

        animator = GetComponent<Animator>();

        DisableRagDoll();
        SetupDataBase();


        if (radaCollider != null) radaCollider.enabled = true;
    }

   

    protected void SetupDataBase()
    {
        //Lấy thông tin player trong màn hình
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        //nếu bật quickAttack thì sẽ khiến thời gian bộ đếm bằng thời gian hồi để có thể tung đòn ngay lập tức khi player vào tầm ngắm
        if (quickAttack) countDelayAttack = delayAttack;

        isAlive = true;
        isMoving = true;
        firstPosition = transform.position;
        distanceTargetToChangeDirection = speed / 2f;
        SetFirstMovingTarget();

        rb.isKinematic = false;
        foreach (var collider in enemyCol)
        {
            collider.enabled = true;
        }
    }

    void SetFirstMovingTarget()
    {
        target = firstPosition;
        if (moveLeftFirst)
            target.x -= unitMove;
        else
            target.x += unitMove;
    }
    #endregion

    #region Attack
    void CountDelayAttack()
    {
        if (DataFireBaseConfig.Instance.isLoaded == false) return;
        //Đếm thời gian hồi đòn đánh
        if (countDelayAttack < delayAttack) countDelayAttack += Time.deltaTime ;
        else
        {
            //Nếu đã đủ thời gian hồi thì tấn công và reset thời gian hồi
            Attack();
            countDelayAttack = 0;
        }
    }

    protected virtual void LookAtPlayer()
    {
        float x = playerTransform.position.x - transform.position.x;

        //Chờ 3d + anim
        if (x > 0)
        {
            //Quay phải
            Quaternion rotGoal = Quaternion.LookRotation(Vector3.right);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, speedTurnToPlayer);
        }
        else
        {
            //Quay trái
            Quaternion rotGoal = Quaternion.LookRotation(Vector3.left);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, speedTurnToPlayer);
        }
    }

    //Kích hoạt Rigging
    protected void SmoothEnableRigging()
    {
        foreach (Rig rig in rigs)
        {
            if (rig.weight < 1f)
            {
                rig.weight += Time.deltaTime  * 2f;
            }
        }
    }

    //Tắt Rigging
    protected void SmoothDisableRigging()
    {
        foreach (Rig rig in rigs)
        {
            if (rig.weight > 0f)
            {
                rig.weight -= Time.deltaTime  * 2f;
            }
        }
    }

    protected void DisableRigging()
    {
        foreach (Rig rig in rigs)
        {
            rig.weight = 0f;

        }
    }

    //Nhìn và nhắm về phía player
    void TargetRiggingKeepOnPlayer()
    {
        foreach (Transform targetRigging in rigTargets)
        {
            Player player = playerTransform.GetComponent<Player>();
            Vector3 playerPos = playerTransform.position;
            playerPos.y += player.IsStandStraight() ? 0.5f : -0.5f;

            Vector3 direction = playerPos - targetRigging.position;
            targetRigging.position += speedTargetToPlayer * Time.deltaTime  * direction;
        }
    }

    //Thực hiện đòn tấn công
    protected abstract void Attack();
    #endregion

    #region Get Attack
    protected virtual void BrokeBarricade()
    {
        //Hủy tấm chắn của kẻ địch
        barricade = false;

        //Chạy các sự kiện thêm
        OnBrokeBarricade?.Invoke();
    }

    //Chết
    public void Death(Transform obj)
    {
        if (GameManager.Instance.IsCompleteLevel) return;//player chet roi thi khong giet enemy nua

        if (radaCollider != null) Destroy(radaCollider.gameObject);
        animator.SetTrigger("hit");
        //EnableRagDoll();
        //GameManager.Instance.PlayVfxBlood(transform.position);
        GameManager.Instance.SpawnBlood(transform);
        this.obj = obj;
        //Vector3 fixCurrentPosition = transform.position;
        //fixCurrentPosition.y += 0.5f;

        ////Tính toán hướng văng dựa vào vị trí hiện tại và vị trí của player
        //Vector3 direction = fixCurrentPosition - obj.position;

        ////Normalized để đồng bộ lực văng
        //direction = direction.normalized;

        ////Thực hiện văng
        //root.AddForce(direction * force);

        //Chuyển đổi sang trạng thái chết
        isAlive = false;

        //
        DisableRigging();

        //Chạy các sự kiện thêm
        OnDeath?.Invoke();

        ////Xóa khỏi màn hình
        //Invoke(nameof(DestroyOnDeath), 3f);

        GameManager.Instance.countEnemyDie++;
        GameManager.Instance.countEnemyDieOneTurn++;
        SoundFXManager.Instance.PlayKillEnemy(GameManager.Instance.countEnemyDie);

        Invoke(nameof(EnableRagDoll), 0.11f);

        GameEvent.instance.PostEvent_WinGame();
    }

    void DestroyOnDeath()
    {
        Destroy(gameObject);
    }

    protected void EnableRagDoll()
    {
        animator.enabled = false;

        foreach (var collider in enemyCol) Destroy(collider);

        foreach (var rigidbody in ragdollRb)
        {
            if(rigidbody!=null)
            rigidbody.isKinematic = false;
        }

        foreach (var collider in ragdollCol)
        {
            if (collider != null)
            {
                collider.enabled = true;
                collider.gameObject.layer = LayerMask.NameToLayer(GameConst.Ragdoll_Layer);
            }
        }

        Vector3 fixCurrentPosition = transform.position;
        fixCurrentPosition.y += 0.5f;

        //Tính toán hướng văng dựa vào vị trí hiện tại và vị trí của player
        Vector3 direction = fixCurrentPosition - obj.position;
        if (obj.CompareTag("Player")) direction *= (-1);

        //Normalized để đồng bộ lực văng
        direction = direction.normalized;

        //Thực hiện văng
        root.AddForce(direction * force);

        //Xóa khỏi màn hình
        Invoke(nameof(DestroyOnDeath), 3f);
    }

    protected void DisableRagDoll()
    {
        animator.enabled = true;

        foreach (var rigidbody in ragdollRb)
        {
            rigidbody.isKinematic = true;
        }

        foreach (var collider in ragdollCol)
        {
            collider.enabled = false;
        }
    }
    #endregion

    #region Moving
    protected void FixedUpdate()
    {
        if (speed > 0 && isAlive && isMoving)
        {
            OnBeforeUpdateMove?.Invoke();
            Moving();
            OnAfterUpdateMove?.Invoke();
        }
    }

    protected virtual void Moving()
    {
        Vector3 currentPosition = transform.position;

        //Tính khoảng cách tới điểm đích
        float distanceToTarget = Vector3.Distance(currentPosition, target);

        if (distanceToTarget > distanceTargetToChangeDirection)
        {
            animator.SetBool("walk", true);

            //Nếu chưa tới điểm đích, tiếp tục di chuyển
            Vector3 direction = target - currentPosition;
            direction = direction.normalized;
            direction.y = 0f;
            direction.z = 0f;

            transform.position +=   speed * Time.fixedDeltaTime * direction;

            if (direction.x == 0) return;

            if (direction.x > 0)
            {
                Quaternion rotGoal = Quaternion.LookRotation(Vector3.right);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, speedMoveTurnAround );
            }
            else
            {
                Quaternion rotGoal = Quaternion.LookRotation(Vector3.left);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, speedMoveTurnAround );
            }
        }
        else
        {
            animator.SetBool("walk", false);

            //Nếu đã tới điểm đích, thay đổi mục tiêu
            //transform.position = target;
            StartCoroutine(ChangeTarget());
        }

    }

    IEnumerator ChangeTarget()
    {
        StopMove();
        yield return new WaitForSeconds(waitForTurnAround);

        float offset = firstPosition.x - target.x;
        target.x += offset * 2f;
        StartMove();
    }

    void StartMove()
    {
        isMoving = true;
        OnChangeMoveTarget?.Invoke();
    }

    void StopMove()
    {
        isMoving = false;
        OnStopMove?.Invoke();
    }
    #endregion

    #region Real-time Processing
    private void Update()
    {
        if (isAlive)
        {
            if (onTarget) SmoothEnableRigging();
            else SmoothDisableRigging();
        }
    }
    public void ShowNote()
    {
        GameManager.Instance.textAnimation.SetPositionParent(new Vector3(transform.position.x - 0.25f, transform.position.y + 0.75f, transform.position.z));
        GameManager.Instance.textAnimation.PlayAnimationNoteText();
    }    
    public void DetectPlayer()//phat hien ra player
    {
        if (!isAlive ) return;

        onTarget = true;
        TargetRiggingKeepOnPlayer();
        StopMove();

        //Nếu người chơi trong tầm ngắm thực hiện đếm và tấn công
        CountDelayAttack();

        //Nhìn theo hướng người chơi
        LookAtPlayer();
    }

    public void UndetectPlayer()
    {
        if (!isAlive) return;
        onTarget = false;
        StartMove();

        //Nếu người chơi bay ra ngoài tầm ngắm thì reset thời gian hồi đòn đánh
        if (quickAttack) countDelayAttack = delayAttack;
        else countDelayAttack = 0;
    }

    public void GetDamage()
    {
        if (barricade)
        {
            //Nếu có tấm chắn thì hủy nó khi bị người chơi tấn công lần đầu
            BrokeBarricade();
        }
        else
        {
            //Bị người chơi đá vào
            Death(playerTransform);
        }
    }   

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!isAlive) return;
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();//neu da va cham voi shiled truoc thi khong cho chet
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
        else if (other.CompareTag("Death Trap"))
        {
            Death(other.transform);
            GameManager.Instance.SlowmotionKillEnemy(0.1f,0.75f);
        }
    }
    public void ExceptionKillEnemy(Player player)
    {
        Vector3 posPlayer = player.transform.position;
        float distance = Vector3.Distance(posPlayer,transform.position);
        float time = distance / (pullSpeedEnemy + player.PullSpeedPlayer) +0.02f ;
        StartCoroutine(IEKillEnemy(time, player));  
    }    

    protected IEnumerator IEKillEnemy(float time,Player player)
    {

        //yield return new WaitForSeconds(time);
        yield return new WaitForSecondsRealtime(time);
        if (!isAlive) yield break;
        if (player.firstRaycast != FirstRaycast.Shield)
        {
            GameManager.Instance.uiTextVfx.PlayAnimationAttackText();
            GameManager.Instance.uiTextVfx.SetPositionUITextVfx(player.transform.position, false);
            if (player.canKickEnemy)
            {
                GameManager.Instance.SlowmotionKillEnemy(0.1f, 0.75f);
                player.canKickEnemy = false;
                //player.enemyBase = null;
            }
            GetDamage();
            SoundFXManager.Instance.PlayEnemyAttack();
            player.firstRaycast = FirstRaycast.Otherthing;
        }
    }    
    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (!isAlive) return;
        if (collision.collider.CompareTag("Player"))
        {
            Player player = collision.collider.GetComponent<Player>();
            player.canKickEnemy = false;
            GetDamage();
           
           player.StartCoroutineKinematic();
            
        }
        else if (collision.collider.CompareTag("Death Trap"))
        {
            Death(playerTransform);
            GameManager.Instance.SlowmotionKillEnemy(0.1f,0.75f);
        }
       
    }
    
   
    #endregion

    public void PullEnemy(Vector3 direction)
    {
        rb.velocity = direction*pullSpeedEnemy;
    }    
    public void EnableAnimIdle()
    {
        animator.SetBool("walk", false);
    }    
   
}
