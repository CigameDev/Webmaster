using DG.Tweening;
using DVAH;
using RootMotion.Demos;
using RootMotion.FinalIK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] public bool flying = false;
    [HideInInspector] public RaycastHit hitAll,hitStop, hitEnemy,hitWoodenBox,hitShield,hitSwitch;//hitStop la hit bam vao tuong
    [SerializeField] float pullSpeedPlayer = 10f;
    public float PullSpeedPlayer => pullSpeedPlayer;
    [SerializeField] GameObject hookPrefab;
    [SerializeField] Transform shootRight;
    [SerializeField] Transform shootLeft;
    [SerializeField] Transform targetHand;
    [SerializeField] Transform targetBody;//body theo doi thang nay
    [SerializeField] Transform bodyTransform;//SpineM
    [SerializeField] LayerMask grappleLayer, enemyLayer,woodenBoxLayer,shieldLayer,switchLayer;
    [SerializeField] ColliderCenterPlayerSO colliderCenter;
    [SerializeField] SkinnedMeshRenderer meshBody;
    [SerializeField] SkinnedMeshRenderer meshItem;
    [SerializeField] SkinConfig skinConfig;
    [SerializeField]
    AnimationClip idle_R, idle_L, cling_L, cling_R, dangling_L, dangling_R, cling_idle_L, cling_idle_R, idle_ngoi_L, idle_ngoi_R, dung_Reaction_L, dung_Reaction_R,
        ngoi_Reaction_L, ngoi_Reaction_R;//dangling chinh la shoot
    [SerializeField] AnimationClip[] kickLeft;
    [SerializeField] AnimationClip[] kickRight;
    private float ConstPullSpeedPlayer = 10f;
    private Vector3 pointHoldHand;
    private Vector3 startPointMouse;
    private Vector3 endPointMouse;
    private Vector3 startPointTarget;
    private Vector3 directionLine;
    private Vector3 oldDirectionLine;
    private bool isShootingRight;
    private float offset = 0.5f;
    private float limitLeftX = -2.2f;
    private float limitRightX = 2.2f;
    private float lastPositionX;//xem lai su dung de tinh toan vi tri su dung OnTrigger 
    private float deltaX = 0.1f;//0.164f
    private float deltaY = 0.1f;//0.0536f
    private float transitionDurationAnim = 0.2f;
    private float displacementX;
    private float absDisplacementX;
    private float displacementY;
    private float absDisplacementY;
    private float divX;
    private float divY;
    private float distanceToStop;
    private Vector3 lastPositionPlayer;
    private Vector3 lastPositionTurnBack;//vị trí cuối cùng trước khi bị bắn bay ngược lại 
    private string lastAnimName;
    private string lastAnimNameTurnBack;
    private float lastPosYCamera;
    private Vector3 lastCenterCollider;
    private bool freeFall;
    private float timeStartFly;//thoi gian player bat dau bay
    private bool isArrowActive;
    public Vector3 playerStartPoint { get;private set; }
    public Vector3 pointStopTurnBack { get; set;}
    public float timeStartTurnBack { get; set; }

    private bool isMouseDownOnAir = false;
    public bool isMouseUpOnAir { get; set; } = false;
    public bool isFlyBack { get; set; } = false;
    public bool isPullEnemyOrBox { get; set; }
    public bool pulling { get; set; }
    public FirstRaycast firstRaycast { get; set; } = FirstRaycast.Otherthing;

    public WoodenBoxController woodenBoxController { get; set; }
    public EnemyBase enemyBase { get; set; }
    public bool conditionDiePlayer { get; private set; } = true;
    public bool canKickEnemy { get; set; }
    public Transform currentShoot { get; private set; }
    public Vector3 _directionLine => directionLine;
    private Vector3 oldHitPoint = new Vector3(-10f,-10f,0f);
    private Vector3 hitPoint;
    private Vector3 hitPointEnemy;
    private Vector3 sizePlayer;
    private int instanceId;
    private Hook hook;
    private FullBodyBipedIK ik;
    private Rigidbody rigid;
    private CapsuleCollider capsuleCollider;
    private GripDirection _gripDirection;
    public GripDirection gripDirection=>_gripDirection;
    private GripDirection oldGripDirection;
    private Animator animator;
    private bool isEndPull;
    public bool isColliding = false;

    private int orderCollisionBox, orderCollisionShield, orderCollisionEnemy, orderCollisionGrapple,orderCollisionSwitch,orderCollisionLaser;
    public int OrderCollisionShield => orderCollisionShield;
    public bool isTriggerWithPlatform = false;//kiểm tra xem player có va chạm IsTrigger với platform không,nếu có thì không cho nhấn chuột,nếu không thì cho nhấn chuột
    private Vector3 tempPoint;
    private int countEnemyDie;//count Enemy =2 doubleKill,countEnemy =3 trippleKill
    private int countEnemyDieOneTurn;// nếu 1 lượt bắn không giết được enemy nào thì reset lại countEnemyDie về 0
    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        ik = GetComponent<FullBodyBipedIK>();
    }
    private void Start()
    {
        LastStatus();
        lastAnimNameTurnBack = GetAnimationName();
        lastPositionTurnBack = this.transform.position;
        sizePlayer = capsuleCollider.bounds.size;
        rigid.useGravity = false;
        pulling = false;
        animator.Rebind();
        animator.Update(0f);
        if(transform.position.x > 0)
        {
            animator.Play(dung_Reaction_R.name);
        }    
        else
        {
            animator.Play(dung_Reaction_L.name);
        }    
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);

        GameEvent.instance.OnChangeSkin += RegisterEvent_ChangeSkin;
        int skinvalueCurrent = DataPlayer.GetSkinCurrentvalue();
        RegisterEvent_ChangeSkin(skinvalueCurrent);

        GameEvent.instance.OnRevive += RegisterEvent_OnRevive;
    }

    private void RegisterEvent_OnRevive()
    {
        this.GetComponent<RagdollOnOff>().RagdollModeOff();
        transform.position = lastPositionPlayer;
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, lastPosYCamera, -10f);
        capsuleCollider.center = lastCenterCollider;
        animator.Play(lastAnimName);
        GameManager.Instance.StartWaitReady();
        GameManager.Instance.Alive = true;
        ik.enabled = true;
    }

    private void OnDestroy()
    {
        GameEvent.instance.OnChangeSkin -= RegisterEvent_ChangeSkin;
        GameEvent.instance.OnRevive -= RegisterEvent_OnRevive;
    }

    private void LastStatus()
    {
        lastPositionPlayer = transform.position;
        lastAnimName = GetAnimationName();
        lastPosYCamera = Camera.main.transform.position.y;
        lastCenterCollider = capsuleCollider.center;
    }

   
    private void Update()
    {
        if (!GameManager.Instance.Alive || GameManager.Instance.IsCompleteLevel) return;
        if (!GameManager.Instance.ready) return;

        FreeFall();
        MouseHanding();
       
        
        if (hook != null && GameManager.Instance.emptyObject !=null)
        {
            hook.MoveToTarget(this, GameManager.Instance.emptyObject.position);
            hook.SetParentForHook(hitAll.transform);
        }
       if(canKickEnemy && enemyBase !=null && isPullEnemyOrBox)
        {
            enemyBase.PullEnemy(directionLine * (-1));
        }
       if(woodenBoxController != null && isPullEnemyOrBox)
        {
            woodenBoxController.PullWoodenBox(directionLine * (-1));
        }

        PullPlayer();

        if (isColliding)
        {
            rigid.velocity = Vector3.zero;
        }
    }


    #region MouseHanding
    private float timeMouseDown =0f;
    private void MouseHanding()
    {
        if (firstRaycast == FirstRaycast.WoodenBox || firstRaycast == FirstRaycast.Enemy || !DVAH3rdLib.Instant.CheckInternetConnection() || isTriggerWithPlatform) return;
       
        if (Input.GetMouseButtonDown(0))
        {

            if (GameManager.Instance.countEnemyDieOneTurn == 0)
            {
                GameManager.Instance.countEnemyDie = 0;
            }
            GameManager.Instance.countEnemyDieOneTurn = 0;

            timeMouseDown = Time.unscaledTime;

            if (directionLine != Vector3.zero)
            {
                oldDirectionLine = directionLine;
            }


            //nhan chuot khi dang o tren khong trung
            if (GetAnimationName() == dangling_R.name || GetAnimationName() == dangling_L.name || flying)
            {
                isMouseDownOnAir = true;
                freeFall = true;
            }
            else
            {
                LastStatus();
                lastAnimNameTurnBack = GetAnimationName();
                lastPositionTurnBack = this.transform.position;
                isMouseDownOnAir = false;

            }

            isShootingRight = IsShootRightHand();

            startPointMouse = Input.mousePosition;
            DestroyHook();

            if (GetAnimationName() == cling_L.name)
            {
                animator.Play(cling_idle_L.name);
                animator.Update(0);//dat trang thai animator ve thoi diem 0
            }
            else if (GetAnimationName() == cling_R.name)
            {
                animator.Play(cling_idle_R.name);
                animator.Update(0);//dat trang thai animator ve thoi diem 0
            }
            else if (flying)
            {
                if (isShootingRight)
                {
                    animator.Play(dangling_R.name, -1, 0f);
                    animator.Update(0);
                }
                else
                {
                    animator.Play(dangling_L.name, -1, 0f);
                    animator.Update(0);
                }
                capsuleCollider.center = colliderCenter.shoot;

                EndPull();

            }



            if (isShootingRight)
            {
                pointHoldHand = shootLeft.position;
                startPointTarget = shootRight.position;
                targetHand.position = startPointTarget;
                currentShoot = shootRight;
                ik.solver.leftHandEffector.target = targetHand;
                ik.solver.leftHandEffector.positionWeight = 1f;
                ik.solver.rightHandEffector.position = pointHoldHand;
                ik.solver.rightHandEffector.positionWeight = 1f;
            }
            else
            {
                pointHoldHand = shootRight.position;
                startPointTarget = shootLeft.position;
                targetHand.position = startPointTarget;
                currentShoot = shootLeft;
                ik.solver.rightHandEffector.target = targetHand;
                ik.solver.rightHandEffector.positionWeight = 1f;
                ik.solver.leftHandEffector.position = pointHoldHand;
                ik.solver.leftHandEffector.positionWeight = 1f;
            }
            GameManager.Instance.arrow.transform.SetParent(currentShoot);
            GameManager.Instance.arrow.transform.localPosition = Vector3.zero;
            GameManager.Instance.ChangeTimeScale(0.2f);
        }

        if (Input.GetMouseButton(0))
        {
            if (isShootingRight)
            {
                ik.solver.rightHandEffector.position = pointHoldHand;
                ik.solver.rightHandEffector.positionWeight = 1f;
            }
            else
            {
                ik.solver.leftHandEffector.position = pointHoldHand;
                ik.solver.leftHandEffector.positionWeight = 1f;
            }

            Vector3 currentPointMouse = Input.mousePosition;

            displacementX = (currentPointMouse.x - startPointMouse.x);
            absDisplacementX = Mathf.Abs(displacementX);
            displacementY = (currentPointMouse.y - startPointMouse.y);
            absDisplacementY = Mathf.Abs(displacementY);

            Vector3 plusOffset;//tang targetHand len 1 khoang

            string nameAnim = GetAnimationName();

            if (oldGripDirection == GripDirection.Up)//treo nguoc
            {
                if (displacementY < -0.4f || displacementY > 0.4f)
                {
                    divY = absDisplacementY / 0.4f;
                }
                if (displacementX < -0.6f || displacementX > 0.6f)
                {
                    divX = absDisplacementX / 0.6f;
                }
                if (divX <= 1f && divY <= 1f)
                {
                    targetHand.position = startPointTarget + (currentPointMouse - startPointMouse);
                }
                else
                {
                    plusOffset = (currentPointMouse - startPointMouse) / (divX >= divY ? divX : divY);
                    targetHand.position = startPointTarget + plusOffset;
                }
            }
            else//khong bi treo nguoc
            {
                if (displacementY > 0.6f)
                {
                    divY = absDisplacementY / 0.6f;
                }
                else if (displacementY < -0.25f)
                {
                    divY = absDisplacementY / 0.25f;
                }
                if (isShootingRight)
                {
                    if (displacementX > 0.8f)
                    {
                        divX = absDisplacementX / 0.8f;
                    }
                    else if (displacementX < -0.2f)
                    {
                        divX = absDisplacementX / 0.2f;
                    }
                }
                else
                {
                    if (displacementX < -0.8f)
                    {
                        divX = absDisplacementX / 0.8f;
                    }
                    else if (displacementX > 0.2f)
                    {
                        divX = absDisplacementX / 0.2f;
                    }
                }

                if (divX <= 1f && divY <= 1f)
                {
                    targetHand.position = startPointTarget + (currentPointMouse - startPointMouse);
                }
                else
                {
                    plusOffset = (currentPointMouse - startPointMouse) / (divX >= divY ? divX : divY);
                    targetHand.position = startPointTarget + plusOffset;
                }
            }

            Vector2 currentDirecion = (Vector2)(currentPointMouse - startPointMouse).normalized;
            if(currentDirecion != Vector2.zero && !isArrowActive)
            {
                GameManager.Instance.arrow.SetActive(true);
                isArrowActive = true;
            }    
            float angel = Vector2.SignedAngle(new Vector2(-1, 0), currentDirecion);
            GameManager.Instance.arrow.transform.rotation = Quaternion.Euler(0, 0, angel + 90f);
        }

        if (hook == null && Input.GetMouseButtonUp(0))
        {
            GameManager.Instance.countShoot++;// dem so lan nguoi nhen di chuyen tren khong
            if(Time.unscaledTime - timeMouseDown >=0.05f)
            {
                freeFall = false;
            }
            GameManager.Instance.arrow.SetActive(false);
            isArrowActive=false;
            GameManager.Instance.ChangeTimeScale(1f);
            endPointMouse = Input.mousePosition;
            directionLine = (endPointMouse - startPointMouse).normalized;
            if(directionLine == Vector3.zero)
            {
                if (isMouseDownOnAir || GetAnimationName() == dangling_L.name || GetAnimationName() == dangling_R.name)
                {
                    directionLine = oldDirectionLine;
                    capsuleCollider.isTrigger = true;
                }
            }
            StopAllCoroutines();
            GameManager.Instance.ChangeTimeScale(1f);
            pulling = false;
            Vector3 pointShoot = new Vector3(currentShoot.position.x, currentShoot.position.y, 0f);
            hook = Instantiate(hookPrefab, pointShoot, Quaternion.identity).GetComponent<Hook>();


            //ban raycast de xac dinh diem bam dinh,neu tim duoc diem bam cuoi cung(tuong hoac platform)
            if (Physics.Raycast(pointShoot, directionLine, out hitStop, 100f, grappleLayer) && ConditionShootRaycast(currentShoot.position, 0.1f, grappleLayer))
            {
                hitPoint = hitStop.point;
                distanceToStop = hitStop.distance;
                GetGripDirection(hitStop);
                hook.stateHook = Hook.StateHook.MoveTarget;
                instanceId = hitStop.collider.GetInstanceID();
                if (hitPoint.y == oldHitPoint.y && (GetAnimationName() == idle_L.name || GetAnimationName() == idle_R.name || GetAnimationName() == dung_Reaction_L.name || GetAnimationName() == dung_Reaction_R.name))
                {
                    DestroyHook();
                    if (oldHitPoint.x < hitPoint.x)
                    {
                        animator.CrossFade(dung_Reaction_L.name, 0.2f);
                    }
                    else
                    {
                        animator.CrossFade(dung_Reaction_R.name, 0.2f);
                    }
                    EndPull();

                    transform.DOMove(new Vector3(hitPoint.x, hitPoint.y, 0f), 0.2f, false);

                    return;
                }

                SortHit(pointShoot, directionLine, distanceToStop);

                //tim xem gap phai vat nao dau tien wall/platorm ,enemy,shield,woodenbox
                firstRaycast = GetFirstRaycast(pointShoot, directionLine, distanceToStop);

                if (orderCollisionEnemy == 1)
                {
                    if (hitEnemy.collider.GetComponent<EnemyBase>()?.GetType() == typeof(ThiefShieldController))
                    {
                        hitEnemy.collider.GetComponent<ThiefShieldController>().ShieldFell();
                    }
                    canKickEnemy = true;
                    Invoke(nameof(EnemyLookAtPlayer), 0.1f);
                    hitPointEnemy = hitEnemy.point;
                    enemyBase = hitEnemy.collider.GetComponent<EnemyBase>();
                }
                else if (orderCollisionBox == 1)
                {

                    woodenBoxController = hitWoodenBox.collider.GetComponent<WoodenBoxController>();
                    woodenBoxController.player = this;
;                   if (orderCollisionEnemy == 2)
                    {
                        canKickEnemy = true;
                    }
                }
                else if (orderCollisionShield == 1)
                {
                    if(isMouseDownOnAir)
                    {
                        isMouseUpOnAir = true;
                    }
                    else
                    {
                        isMouseUpOnAir = false;
                    }
                }
                else
                {

                }
            }
            else// neu nhu ban chui vao tuong ,khong tim duoc diem bam dinh ( nguoi choi co tinh)
            {
                if(GetAnimationName()== dangling_R.name || GetAnimationName() == dangling_L.name)
                {
                    Collider col = null;
                    if(hook!= null)
                    {
                        col = GetCollider(hook.transform);
                    }    
                    if(col != null)
                    {
                        float sizeColliderX = col.bounds.size.x;
                        float sizeColliderY = col.bounds.size.y;
                        if(col.CompareTag("Wall"))
                        {
                            if(this.transform.position.x < -1)
                            {
                                //wall left
                                Debug.Log("wall left");
                                animator.Play(cling_R.name);
                                capsuleCollider.center = colliderCenter.idle_L;
                            }
                            else
                            {
                                //wall right
                                Debug.Log("wall right");
                                animator.Play(cling_L.name);
                                capsuleCollider.center = colliderCenter.idle_R;
                            } 
                                
                        }  
                        else if(col.CompareTag("Top"))
                        {
                            Debug.Log("Top");
                            animator.Play(ngoi_Reaction_L.name);
                            capsuleCollider.center = colliderCenter.ngoi_L;
                            this.transform.position = new Vector3(this.transform.position.x, col.transform.position.y - sizeColliderY / 2f, 0f);
                        }
                        else if(col.CompareTag("Platform"))
                        {
                            //platform chia làm 3 trường hợp left,right,up,diffirent
                            if(oldGripDirection == GripDirection.Left)
                            {
                                Debug.Log("left");
                                animator.Play(cling_R.name);
                                capsuleCollider.center = colliderCenter.idle_L;

                                this.transform.position = new Vector3(col.transform.position.x + sizeColliderX / 2f + 0.4f, this.transform.position.y, 0f);
                                if (this.transform.position.y < col.transform.position.y - sizeColliderY / 2f)
                                {
                                    this.transform.position = new Vector3(this.transform.position.x, col.transform.position.y - sizeColliderY / 2f, 0f);
                                }
                            }
                            else if(oldGripDirection == GripDirection.Right)
                            {
                                Debug.Log("right");
                                animator.Play(cling_L.name);
                                capsuleCollider.center = colliderCenter.idle_R;

                                this.transform.position = new Vector3(col.transform.position.x - sizeColliderX / 2f - 0.4f, this.transform.position.y, 0f);
                                if (this.transform.position.y < col.transform.position.y - sizeColliderY / 2f)
                                {
                                    this.transform.position = new Vector3(this.transform.position.x, col.transform.position.y - sizeColliderY / 2f, 0f);
                                }
                            }
                            else if(oldGripDirection == GripDirection.Up)
                            {
                                Debug.Log("Up");
                                animator.Play(ngoi_Reaction_L.name);
                                capsuleCollider.center = colliderCenter.ngoi_L;
                                this.transform.position = new Vector3(this.transform.position.x, col.transform.position.y - sizeColliderY / 2f, 0f);
                            }
                            else
                            {
                                //tất cả các trường hợp còn lại là đang bị treo ngược sau đó nhả tay ra bắn ,sau đó ngay lập tức nhấn chuột xuống 
                                Debug.Log("Diffirent");
                                animator.Play(ngoi_Reaction_L.name);
                                capsuleCollider.center = colliderCenter.ngoi_L;
                                this.transform.position = new Vector3(this.transform.position.x, col.transform.position.y - sizeColliderY / 2f, 0f);
                            }
                        }    
                    }
                }
                DestroyHook();
                hitStop = default(RaycastHit);
                hitEnemy = default(RaycastHit);
                hitWoodenBox = default(RaycastHit);
                hitShield = default(RaycastHit);
                hitAll = default(RaycastHit);
                ResetPositionWeight();
            }
            if (hook != null)
            {
                hook.Inittialize(this);
            }
        }
    }
    #endregion
    private Collider GetCollider(Transform transform)
    {
        //từ vị trí transform vẽ ra hình cầu đường kính 0.1, tính xem hình cầu đó có va chạm với collider nào dạng grapple không
        Collider[] collider = Physics.OverlapSphere(transform.position, 0.1f, 1 << 6);
        if (collider == null || collider.Length == 0) return null;
        return collider[0];
    }    
    private void FreeFall()
    {
        if (!freeFall) return;
        float posY = transform.position.y - Time.deltaTime *1.5f;
        this.transform.position = new Vector3(transform.position.x, posY, transform.position.z);
        startPointTarget = new Vector3(startPointTarget.x,startPointTarget.y - Time.deltaTime*1.5f *1.5f, startPointTarget.z);
    }
    private void EnemyLookAtPlayer()
    {
        if (!hitEnemy.Equals(default(RaycastHit)))
        {
            hitEnemy.transform?.LookAt(transform);
        }
    }
    #region StartPull +EndPull
    public void StartPull()
    {
        playerStartPoint = transform.position;
        isEndPull = false;
        if(enemyBase != null)
        {
            enemyBase?.ExceptionKillEnemy(this);
        }
        if(woodenBoxController != null)
        {
            woodenBoxController.KickWoodenBox();
        }    
        timeStartFly = Time.time;
        TriggerColliderPlayer(true);
        pulling = true;
        flying = true;//nhan vat bay
        if (!hitEnemy.Equals(default(RaycastHit)) && Between(currentShoot.transform.position, hitEnemy.point, hitStop.point))
        {
            ResetPositionWeight();
            float angel = Vector2.SignedAngle(new(-1, 0), new Vector2(-_directionLine.x, -_directionLine.y));
           
            if ((angel >= -120f && angel <= -60f) || (angel >= 60f && angel <= 120f))
            {
                if (oldHitPoint.x < hitPoint.x)
                {
                    animator.CrossFade(kickLeft[0].name, transitionDurationAnim);
                    capsuleCollider.center = colliderCenter.kick_L;
                }
                else
                {
                    animator.CrossFade(kickRight[0].name, transitionDurationAnim);
                    capsuleCollider.center = colliderCenter.kick_R;
                }
            }
            else if(oldGripDirection != GripDirection.Up)
            {
                int rand = UnityEngine.Random.Range(0, kickLeft.Length);
                if (oldHitPoint.x < hitPoint.x)
                {
                    if (rand == 0)
                    {
                        animator.CrossFade(kickLeft[0].name, transitionDurationAnim);
                        capsuleCollider.center = colliderCenter.kick_L;
                    }
                    else
                    {
                        animator.CrossFade(kickLeft[rand].name, transitionDurationAnim);
                        capsuleCollider.center = colliderCenter.kick_L2;
                    }
                }
                else
                {
                    if(rand == 0)
                    {
                        animator.CrossFade(kickRight[0].name, transitionDurationAnim);
                        capsuleCollider.center = colliderCenter.kick_R;
                    }
                    else
                    {
                        animator.CrossFade(kickRight[rand].name, transitionDurationAnim);
                        capsuleCollider.center = colliderCenter.kick_R2;
                    }
                }
            }
        }
        else
        {
            ik.solver.bodyEffector.target = targetBody;
            if (isShootingRight)
            {
                ik.solver.rightHandEffector.positionWeight = 0f;
                if (oldGripDirection != GripDirection.Up)
                {
                    ik.solver.bodyEffector.positionWeight = 1f;
                    targetBody.position = new Vector3(bodyTransform.position.x - deltaX, bodyTransform.position.y + deltaY, bodyTransform.position.z);
                }
            }
            else
            {
                ik.solver.leftHandEffector.positionWeight = 0f;
                if (oldGripDirection != GripDirection.Up)
                {
                    ik.solver.bodyEffector.positionWeight = 0.5f;
                    targetBody.position = new Vector3(bodyTransform.position.x + deltaX, bodyTransform.position.y + deltaY, bodyTransform.position.z);
                }
            }
        }

        float distance = Vector3.Distance(this.transform.position, hitPoint);
        float time = distance / pullSpeedPlayer + 0.02f;

        
        coroutineEndPull = StartCoroutine(IEExceptionEndPull(time));
    }
   
    public void EndPull()
    {
        if (!flying)
        {
            LastStatus();
        }

        isPullEnemyOrBox = false;
        firstRaycast = FirstRaycast.Otherthing;
        oldGripDirection = _gripDirection;
        GameManager.Instance.spiderWeb.GetComponentInChildren<SpiderWeb>().PlayAnimationEnd();
        TriggerColliderPlayer(false);
        flying = false;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        if(transform.position.x < limitLeftX)
        {
            transform.position = new Vector3(limitLeftX, transform.position.y, 0f);
        }    
        else if(transform.position.x > limitRightX)
        {
            transform.position = new Vector3(limitRightX, transform.position.y, 0f);
        }    
        DestroyHook();
        rigid.velocity = Vector3.zero;
        oldHitPoint = hitPoint;
        _gripDirection = GripDirection.None;
        hitStop = default(RaycastHit);
        hitEnemy = default(RaycastHit);
        hitWoodenBox = default(RaycastHit);
        hitShield = default(RaycastHit);
        ResetPositionWeight();
        ik.solver.leftHandEffector.target = null;
        ik.solver.rightHandEffector.target = null;
        ik.solver.bodyEffector.target = null;

    }

   
    #endregion

    private void DestroyHook()
    {
        if (hook == null) return;
        pulling = false;
        Destroy(hook.gameObject);
        hook = null;
    }
    private void RotatePlayer()
    {
        if (_gripDirection == GripDirection.None) return;
        freeFall = false;
        if (_gripDirection == GripDirection.Left)
        {
            animator.Play(cling_R.name);
            capsuleCollider.center = colliderCenter.idle_L;
            this.transform.position = new Vector3(transform.position.x + offset, transform.position.y, 0f);
            if (Mathf.Abs(transform.position.x - limitLeftX) < 0.5f)
            {
                this.transform.position = new Vector3(limitLeftX, transform.position.y, 0f);
            }
        }
        else if (_gripDirection == GripDirection.Right)
        {
            animator.Play(cling_L.name);
            capsuleCollider.center = colliderCenter.idle_R;
            this.transform.position = new Vector3(transform.position.x - offset, transform.position.y, 0f);
            if (Mathf.Abs(transform.position.x - limitRightX) < 0.5f)
            {
                this.transform.position = new Vector3(limitRightX, transform.position.y, 0f);
            }
        }
        else if (_gripDirection == GripDirection.Down)
        {
            if (oldHitPoint.x < hitPoint.x)
            {
                animator.Play(dung_Reaction_L.name);
            }
            else
            {
                animator.Play(dung_Reaction_R.name);
            }
            capsuleCollider.center = colliderCenter.posCol_Idle;
            this.transform.position = hitPoint;
            if (transform.position.y < GameManager.Instance.LimitUnderY)
            {
                this.transform.position = new Vector3(transform.position.x, GameManager.Instance.LimitUnderY, 0f);
            }
        }
        else if (_gripDirection == GripDirection.Up)
        {
            if (oldHitPoint.x < hitPoint.x)
            {
                animator.Play(ngoi_Reaction_R.name);
                capsuleCollider.center = colliderCenter.ngoi_R;
            }
            else
            {
                animator.Play(ngoi_Reaction_L.name);
                capsuleCollider.center = colliderCenter.ngoi_L;
            }
            this.transform.position = new Vector3(this.transform.position.x, hitPoint.y, 0f);
        }
    }

    public void ReverseTurn()
    {
        isPullEnemyOrBox = false;
        oldGripDirection = _gripDirection;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        DestroyHook();
        rigid.velocity = Vector3.zero;
        oldHitPoint = hitPoint;

        flying = false;
        hitStop = default(RaycastHit);
        hitEnemy = default(RaycastHit);
        hitWoodenBox = default(RaycastHit);
        hitShield = default(RaycastHit);
        ResetPositionWeight();
        ik.solver.leftHandEffector.target = null;
        ik.solver.rightHandEffector.target = null;
        ik.solver.bodyEffector.target = null;
    }

    private void GetGripDirection(RaycastHit hit)
    {
        if (hit.normal == new Vector3(1f, 0f, 0f))
        {
            _gripDirection = GripDirection.Left;
        }
        else if (hit.normal == new Vector3(-1f, 0f, 0f))
        {
            _gripDirection = GripDirection.Right;
        }
        else if (hit.normal == new Vector3(0f, -1f, 0f))
        {
            _gripDirection = GripDirection.Up;
        }
        else if (hit.normal == new Vector3(0f, 1f, 0f))
        {
            _gripDirection = GripDirection.Down;
        }
    }

    private void PullPlayer()
    {

        if (!pulling || hook == null)
        {
            if (flying)
            {
                rigid.velocity = pullSpeedPlayer * directionLine.normalized;
            }
            return;
        }
        rigid.velocity =  pullSpeedPlayer * directionLine.normalized;
    }
    public bool IsShootRightHand()//ban bang tay phai 
    {
        string name = GetAnimationName();
        if (name == idle_R.name || name == dung_Reaction_R.name || name == cling_idle_L.name || name == cling_L.name || name == dangling_R.name ||name == idle_ngoi_L.name ||name == ngoi_Reaction_L.name) return true;//nhin sang trai ,bam tuong trai ban tay phai
        return false;
    }
    private string GetAnimationName()
    {
        if (animator.GetCurrentAnimatorClipInfoCount(0) > 0)
        {
            AnimatorClipInfo clipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];
            AnimationClip currentClip = clipInfo.clip;
            return currentClip.name;
        }
        return "";
    }

    private void ResetConditionDiePlayer()
    {
        conditionDiePlayer = true;
    }
    private Coroutine coroutineEndPull;
    public IEnumerator IEExceptionEndPull(float time)
    {
        yield return new WaitForSeconds(time);

        if (isEndPull || firstRaycast != FirstRaycast.Grapple || GameManager.Instance.isShieldDetector == true )
        {
            yield break;
        }
        string tagName = string.Empty;
        Collider collider = null;
        if(!hitStop.Equals(default(RaycastHit)))
        {
            tagName = hitStop.collider.tag.ToString();
            collider = hitStop.collider;
        }    
        RotatePlayer();
        EndPull();

        if (tagName == GameConst.Platform_Tag)
        {
            if (oldGripDirection == GripDirection.Left)
            {
                transform.position = new Vector3(collider.transform.position.x + collider.bounds.size.x / 2f + 0.37f, collider.transform.position.y - 0.4f, 0f);
            }
            else if (oldGripDirection == GripDirection.Right)
            {
                transform.position = new Vector3(collider.transform.position.x - collider.bounds.size.x / 2f - 0.37f, collider.transform.position.y - 0.4f, 0f);
            }
        }
    }
    public void StopCoroutineEndPull()
    {
        if (coroutineEndPull != null)
        {
            StopCoroutine(coroutineEndPull);
            coroutineEndPull = null;
        }
    }
    #region  Bat Va cham
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(GameConst.Wall_Tag))
        {
            if (isFlyBack == false)
            {
                if (Time.time - timeStartFly > 0.015f)
                {
                    RotatePlayer();
                    EndPull();
                }
            }
            else
            {
                if (transform.position.x < -2f)
                {
                    this.transform.position = new Vector3(limitLeftX, transform.position.y, 0f);
                    animator.Play(cling_R.name);
                    capsuleCollider.center = colliderCenter.idle_L;
                }
                else if (transform.position.x > 2f)
                {
                    this.transform.position = new Vector3(limitRightX, transform.position.y, 0f);
                    animator.Play(cling_L.name);
                    capsuleCollider.center = colliderCenter.idle_R;
                }
                isFlyBack = false;
                EndPull();

            }    
        }
        else if ((LayerMask.GetMask("Grapple") & 1 << other.gameObject.layer) > 0 )
        {
            if(isFlyBack)
            {
                if (Time.time - timeStartTurnBack >= 0.1f)
                {
                    isFlyBack = false;
                    if (isMouseUpOnAir == false)
                    {
                        this.transform.position = lastPositionTurnBack;
                        animator.Play(lastAnimNameTurnBack);
                        capsuleCollider.center = lastCenterCollider;

                    }
                    else
                    {
                        animator.Play(ngoi_Reaction_L.name);
                        capsuleCollider.center = colliderCenter.ngoi_L;
                        this.transform.position = pointStopTurnBack;
                    }
                    EndPull();
                }
            }
            else if(other.GetInstanceID() == instanceId)
            {
                isEndPull = true;
                RotatePlayer();
                EndPull();
                if (other.gameObject.CompareTag(GameConst.Platform_Tag))
                {
                    if (oldGripDirection == GripDirection.Left)
                    {
                        transform.position = new Vector3(other.transform.position.x + other.bounds.size.x / 2f + 0.37f,  this.transform.position.y, 0f);
                        if (this.transform.position.y > other.transform.position.y)
                        {
                            transform.position = new Vector3(this.transform.position.x, other.transform.position.y + other.bounds.size.y / 2f - 0.5f, 0f);
                        }
                        if (this.transform.position.y < other.transform.position.y - other.bounds.size.y/2f)
                        {
                            this.transform.position = new Vector3(this.transform.position.x, other.transform.position.y - other.bounds.size.y / 2f, 0f);
                        }    
                    }
                    else if (oldGripDirection == GripDirection.Right)
                    {
                        transform.position = new Vector3(other.transform.position.x - other.bounds.size.x / 2f - 0.37f, this.transform.position.y , 0f);
                        if(this.transform.position.y > other.transform.position.y)
                        {
                            transform.position = new Vector3(this.transform.position.x, other.transform.position.y + other.bounds.size.y/2f -0.5f, 0f);
                        }
                        if (this.transform.position.y < other.transform.position.y - other.bounds.size.y / 2f)
                        {
                            this.transform.position = new Vector3(this.transform.position.x, other.transform.position.y - other.bounds.size.y / 2f, 0f);
                        }
                    }
                }
            }    
            else if(other.GetInstanceID() != instanceId && other.CompareTag("Platform"))
            {
                //trường hợp va chạm IsTrigger với platform  mà không phải điểm đến cuối cùng thì không cho nhấn chuột
                isTriggerWithPlatform = true;
                Invoke(nameof(SetFalseTriggerWithPlatform),0.1f);

            }    
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer(GameConst.Enemy_Layer) || other.gameObject.layer == LayerMask.NameToLayer(GameConst.Shield_Layer) || other.gameObject.layer == LayerMask.NameToLayer(GameConst.WoodenBox_Layer))
        {
            if (hook != null)
            {
                hook.stateHook = Hook.StateHook.NoDraw;
            }
        }
        else if(other.gameObject.CompareTag("Death Trap")) 
        {
            SoundFXManager.Instance.PlayPlayerDie();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(GameConst.Platform_Tag))
        {
            isTriggerWithPlatform = false;
        }
    }
    private void SetFalseTriggerWithPlatform()
    {
        if(isTriggerWithPlatform == true)
        {
            isTriggerWithPlatform = false;
        }    
    }    
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(GameConst.Platform_Tag) || col.gameObject.CompareTag("Top"))
        {
            if(col.gameObject.CompareTag("Top"))
            {
                StartCoroutineVelocityZero();
            }
            conditionDiePlayer = false;
            Invoke(nameof(ResetConditionDiePlayer), 0.1f);

            float posX = transform.position.x;
            if (posX <= -2 || posX >= 2)
            {
                float sizeY = col.collider.bounds.size.y;
                float sizePlayerY = sizePlayer.y;
                if (hitPoint.y < col.transform.position.y)
                {
                    transform.position = new Vector3(transform.position.x, col.transform.position.y - sizePlayerY - sizeY / 2 - 0.2f, 0f);
                    if(GetAnimationName()== ngoi_Reaction_L.name || GetAnimationName()== idle_ngoi_L.name || GetAnimationName() == ngoi_Reaction_R.name || GetAnimationName() == idle_ngoi_R.name)
                    {
                        if(posX <= -2)
                        {
                            animator.Play(cling_R.name);
                        }    

                        else if(posX >=2)
                        {
                            animator.Play(cling_L.name);
                        }    
                    }    
                   
                }
                else
                {
                    transform.position = new Vector3(transform.position.x, col.transform.position.y + sizeY / 2, 0f);
                }
            }
        }
        else if (col.gameObject.CompareTag(GameConst.Wall_Tag))
        {
            if (oldGripDirection == GripDirection.Up || oldGripDirection == GripDirection.Down)
            {
                if (transform.position.x < -2f)
                {
                    transform.position = new Vector3(limitLeftX, transform.position.y, 0f);
                }
                else if (transform.position.x > 2f)
                {
                    transform.position = new Vector3(limitRightX, transform.position.y, 0f);
                }
            }
          
        }
        else if (col.gameObject.CompareTag(GameConst.Ground_Tag))

                {
                    StartCoroutineVelocityZero();

            float sizeY = col.collider.bounds.size.y;
            transform.position = new Vector3(transform.position.x, col.transform.position.y + sizeY / 2f , 0f);
            string nameAnim = GetAnimationName();

            if (nameAnim == dangling_L.name)
            {
                //xử lý đoạn này sau
                //khi đang trên không rơi dần xuống thì sẽ đi vào đây
                //animator.Play(idle_L.name);
            }
            else if (nameAnim == dangling_R.name)
            {
                //xử lý đoạn này sau
                //animator.Play(idle_R.name);
            }

        }
      
    }
    
 
    #endregion
    private IEnumerator velocityZeroRoutine;
    private IEnumerator IEVelocityZero()

    {
        isColliding = true;
        yield return new WaitForSeconds(0.15f);
        isColliding = false;
    }
    private void StartCoroutineVelocityZero()
    {
        //if (velocityZeroRoutine != null)
        //{
        //    StopCoroutine(velocityZeroRoutine);
        //}
        //velocityZeroRoutine = IEVelocityZero();
        //StartCoroutine(velocityZeroRoutine);
    }
    private IEnumerator kinematicRoutine;
   
    private IEnumerator IEKinematic()
    {
        //khi va chạm với enemy hoặc woodenBox dạng Collision thi cho rigid thành dạng kinematic trong 0.1s
        rigid.isKinematic = true;
        yield return new WaitForSeconds(0.1f);
        rigid.isKinematic = false;
    }
    public void StartCoroutineKinematic()
    {
        if(kinematicRoutine != null)
        {
            StopCoroutine(kinematicRoutine);
        }
        kinematicRoutine = IEKinematic();
        StartCoroutine(kinematicRoutine);
    }    
    public void OnDie()
    {
        if (GameManager.Instance.IsCompleteLevel) return;//da win roi thi khong the chet
        int countFail = DataPlayer.alldata.countFail + 1;
        DataPlayer.SetCountFail(countFail);

        GameManager.Instance.IsCompleteLevel = true;
        GameManager.Instance.ready = false;
        GameManager.Instance.PlayVfxBlood(this.transform.position);
        this.GetComponent<RagdollOnOff>().RagdollModeOn();
        ik.enabled = false;
        GameManager.Instance.Alive = false;
        EndPull();
        oldDirectionLine = Vector3.zero;
        UIController.Instance.InvokeShowFailPopUp();

        int timePlay = (int)Math.Ceiling(Time.unscaledTime - GameManager.Instance.startTimeLevel);
        FireBaseManager.Instant.LogEventWithParameterAsync("gameplay_win", new Hashtable()
        {
            {
                "id_screen","GAMEPLAY"
            },
            {
                "id_level",DataPlayer.GetLevelValue()
            },
            {
                "level_time",timePlay
            },
            {
                "level_action",GameManager.Instance.countShoot
            }
        }) ;
    }
    private bool Between(Vector3 A, Vector3 B, Vector3 C)//kiem tra xem diem B co nam giua diem A va C khong
    {
        Vector2 a = (Vector2)A;
        Vector2 b = (Vector2)B;
        Vector2 c = (Vector2)C;
        Vector2 AB = b - a;
        Vector2 BC = c - b;
        return Vector3.Dot(AB, BC) > 0;
    }
    private void TriggerColliderPlayer(bool isTrigger)
    {
        capsuleCollider.isTrigger = isTrigger;
    }

    public bool IsStandStraight()
    {
        string nameAnim = GetAnimationName();
        if (nameAnim == ngoi_Reaction_L.name || nameAnim == ngoi_Reaction_R.name || nameAnim == idle_ngoi_L.name|| nameAnim == idle_ngoi_R.name)
        {
            return false;
        }
        return true;
    }

    private bool ConditionShootRaycast(Vector3 center, float radius, LayerMask layerMask)
    {
        //dung tren platform ,y cua currentShoot < y cua player khong cho ban raycast
        return !HaveCollider(center, radius, layerMask);
    }

    private bool HaveCollider(Vector3 center, float radius, LayerMask layerMask)//ve 1 hinh tron tam la center ,ban kinh la radius xem co collider nao khong
    {
        Collider collider = Physics.OverlapSphere(center, radius, layerMask).FirstOrDefault();
        return collider != null;
    }

    public void ResetPositionWeight()
    {
        ik.solver.leftHandEffector.positionWeight = 0f;
        ik.solver.rightHandEffector.positionWeight = 0f;
        ik.solver.bodyEffector.positionWeight = 0f;
    }
    //giam van toc cua player khi cham phai box
    private IEnumerator changeVelocityRoutine;
    private IEnumerator IEChangeVelocity()
    {
        pullSpeedPlayer = ConstPullSpeedPlayer / 3f;
        yield return new WaitForSeconds(0.25f);
        pullSpeedPlayer = ConstPullSpeedPlayer;
    }    
    public void StartCoroutineChangeVelocityPlayer()
    {
        if (changeVelocityRoutine != null)
        {
            StopCoroutine(changeVelocityRoutine);
        }
        changeVelocityRoutine = IEChangeVelocity();
        StartCoroutine(changeVelocityRoutine);
    }    
    public void PlayAnimDangling()
    {
        if(isShootingRight)
        {
            animator.CrossFade(dangling_R.name, 0.2f);
        }    
        else
        {
            animator.CrossFade(dangling_L.name, 0.2f);
        }
    }

    private FirstRaycast GetFirstRaycast(Vector3 point, Vector3 dir, float distance)
    {
        if (Physics.Raycast(point, dir, out hitAll, distance, enemyLayer | woodenBoxLayer | shieldLayer | grappleLayer|switchLayer))
        {
            GameManager.Instance.InitEmptyObject(hitAll.collider.transform, hitAll.point);
            GameManager.Instance.hitTheWallFirst = false;

            if (hitAll.collider.gameObject.layer == LayerMask.NameToLayer(GameConst.Enemy_Layer))//ban trung enemy dau tien
            {
                return FirstRaycast.Enemy;
            }
            if (hitAll.collider.gameObject.layer == LayerMask.NameToLayer(GameConst.WoodenBox_Layer))//ban trung thung go dau tien
            {
                return FirstRaycast.WoodenBox;
            }
            else if (hitAll.collider.gameObject.layer == LayerMask.NameToLayer(GameConst.Shield_Layer))//ban trung khien dau tien
            {
                return FirstRaycast.Shield;
            }
            else if (hitAll.collider.gameObject.layer == LayerMask.NameToLayer(GameConst.Switch_Layer))
            {
                return FirstRaycast.Switch;
            }
            else//ban trung tuong /platform dau tien
            {
                GameManager.Instance.hitTheWallFirst = true;
                return FirstRaycast.Grapple;
            }
        }
        return FirstRaycast.Otherthing;
    }

    private void SortHit(Vector3 origin ,Vector3 direction,float maxDistance)
    {
        int getMaskValue = LayerMask.GetMask(GameConst.Enemy_Layer,GameConst.Grapple_Layer,GameConst.WoodenBox_Layer,GameConst.Shield_Layer,GameConst.Switch_Layer,GameConst.Laser_Layer);
        RaycastHit[] hits= Physics.RaycastAll(origin,direction,maxDistance,getMaskValue);
        hits = hits.OrderBy(hit =>hit.distance).ToArray();

        List<RaycastHit> uniqueHits = new List<RaycastHit>();
        List<int>uniqueValueLayers = new List<int>();
        
        for(int i =0;i< hits.Length;i++)
        {
            int value = hits[i].collider.gameObject.layer;
            if (!uniqueValueLayers.Contains(value))
            {
                uniqueHits.Add(hits[i]);
                uniqueValueLayers.Add(value);
            }    
        }    
        ResetOrderCollision();
       

        for (int i=0;i<uniqueValueLayers.Count;i++)
        {
            if (uniqueValueLayers[i] == LayerMask.NameToLayer(GameConst.Grapple_Layer))
            {
                orderCollisionGrapple = i + 1;
                hitStop = hits[i];
            }
            else if (uniqueValueLayers[i] == LayerMask.NameToLayer(GameConst.Enemy_Layer))
            {
                orderCollisionEnemy =i+1;
                hitEnemy = hits[i];
            }
            else if (uniqueValueLayers[i] == LayerMask.NameToLayer(GameConst.WoodenBox_Layer))
            {
                orderCollisionBox = i + 1;
                hitWoodenBox = hits[i];
            }  
            else if (uniqueValueLayers[i] == LayerMask.NameToLayer(GameConst.Switch_Layer))
            {
                orderCollisionSwitch = i + 1;
                hitSwitch = hits[i];
            }    
            else if(uniqueValueLayers[i] == LayerMask.NameToLayer(GameConst.Shield_Layer))
            {
                orderCollisionShield = i+1;
                hitShield = hits[i];
            }    
            else
            {
                orderCollisionLaser = i + 1;
            }
        }
    }    
    private void ResetOrderCollision()
    {
        orderCollisionBox = 0;
        orderCollisionEnemy = 0;
        orderCollisionGrapple = 0;
        orderCollisionShield = 0;
    }

    private void RegisterEvent_ChangeSkin(int id)
    {
        meshBody.sharedMesh = skinConfig.GetMeshBody(id);
        meshBody.material = skinConfig.GetMaterialBody(id);
        meshItem.sharedMesh = skinConfig.GetMeshItem(id);
        meshItem.material = skinConfig.GetMaterialItem(id);
    }    
    public void SetTriggerCollider(bool isTrigger)
    {
        capsuleCollider.isTrigger = isTrigger;
    }    
}
