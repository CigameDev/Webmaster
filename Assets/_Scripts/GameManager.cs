using DG.Tweening;
using DVAH;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
   public static GameManager Instance { get; private set; }

    [SerializeField] EnemySO enemySO;
    [SerializeField] Camera cam;
    [SerializeField] Camera camUI;
    public  GameObject arrow;
    public GameObject spiderWeb;
    public LayerMask enemyRagdoll;
    public UITextVfx uiTextVfx;
    public Transform emptyPrefab;
    public Transform emptyObject { set; get; }
    [SerializeField] ParticleSystem vfxBlood;
    [SerializeField] ParticleSystem vfxConfettiLeft;
    [SerializeField] ParticleSystem vfxConfettiRight;
    [SerializeField] GameObject vfxExplosion;
    [SerializeField] GameObject vfxBloodPrefab;
    [SerializeField] float limitUnderY = -3.47f;
    [SerializeField] Transform progressBar;
    [SerializeField] GameObject itemEnemyPrefab;
    [SerializeField] bool noSpawnLevel;
    [SerializeField] int levelIndexTest;
    [SerializeField] Transform transformText;
    [SerializeField] GameObject hand;
    private List<GameObject> enemyIconList = new List<GameObject>();
    public bool conditionDetectPlayer { get; set; }= false;
    public bool hitTheWallFirst { get; set; }
    public TextAnimation textAnimation { get; private set; }
    public bool isZoomCamera { get; private set; } = false;
    public bool isKillLastEnemy { get; private set; } = false;
    public float LimitUnderY => limitUnderY;
    public bool Alive { get;  set; } = true;
    public bool ready { get; set; }
    public bool IsCompleteLevel { get; set; }
    public bool isShieldDetector { get; set; } = false;
    
    const int ALL_LEVEL = 50;
    int levelCurrent;
    int randomLevel;
    int numberEnemy;
    int numberEnemyDie;
    int inputTimeVibrate = 100;
    bool conditionZoomOut = false;
    int screenHeight;
    int screenWidth;

    public float startTimeLevel { get; set; }//dem thoi gian hoan thanh level
    public int countShoot { get; set; }//dem so lan nham ban de hoan thanh level
    public int countEnemyDie { get; set; }//count Enemy =2 doubleKill,countEnemy =3 trippleKill
    public int countEnemyDieOneTurn { get; set; }// nếu 1 lượt bắn không giết được enemy nào thì reset lại countEnemyDie về 0
    Coroutine slowmotionKillEnemy;
   
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }    
        else
        {
            Destroy(gameObject);
        }
       
    }

    private void OnEnable()
    {
        FireBaseManager.Instant.LogEventWithParameterAsync("gameplay_start", new Hashtable()
        {
            {
                "id_screen","GAMEPLAY"
            },
            {
                "id_level",DataPlayer.GetLevelValue()
            }
        });
        countShoot = 0;
    }
    private void Start()
    {
        Vibration.Init();
        screenHeight = Screen.height;
        screenWidth = Screen.width;
        textAnimation = transformText.GetComponentInChildren<TextAnimation>();
        levelCurrent = DataPlayer.GetLevelValue();
        if(noSpawnLevel)
        {
            levelCurrent = levelIndexTest;
        }    
        else
        {
            if (DataPlayer.alldata.isNormalMap)//Spawn ra map thuong
            {
                if (levelCurrent <= ALL_LEVEL)
                {
                    var newLevel = Instantiate(Resources.Load<GameObject>("Levels/" + levelCurrent));
                }
                else
                {
                     randomLevel = UnityEngine.Random.Range(1, ALL_LEVEL +1);
                    var newLevel = Instantiate(Resources.Load<GameObject>("Levels/" + randomLevel));
                }    
            }
            else//Spawn ra map tutorial
            {
                var newLevel = Instantiate(Resources.Load<GameObject>("Levels/Tutorial_"+levelCurrent));
                UIController.Instance.HideUIHome();
                UIController.Instance.ShowTutorialPopup();
                if(levelCurrent ==1)
                {
                    hand.SetActive(true);
                }    
            }    

        }
        if(levelCurrent <= ALL_LEVEL)
        {
            numberEnemy = enemySO.listNumberEnemy[levelCurrent - 1];
        }
        else
        {
            numberEnemy = enemySO.listNumberEnemy[randomLevel - 1];
        }

        //var newLevel = Instantiate(Resources.Load<GameObject>("Levels/" + levelCurrent));
        SpawnPartEnemy(numberEnemy);
        GameEvent.instance.OnWinGame +=RegisterEvent_OnWinGame;

        
    }
  
    private void OnDestroy()
    {
        GameEvent.instance.OnWinGame -= RegisterEvent_OnWinGame;
    }
   
    private void FixedUpdate()
    {
        if (isZoomCamera)
        {
            cam.fieldOfView -= Time.fixedUnscaledDeltaTime * 20f;
        }
        else if (conditionZoomOut)
        {
            cam.fieldOfView += Time.fixedUnscaledDeltaTime * 30f;
            if (cam.fieldOfView >= 90f)
            {
                conditionZoomOut = false;
            }
        }

    }
    public void SetPositionHand()
    {
        hand.transform.position = new Vector3(-0.5f, 0f, 0f);
    }    
    private void RegisterEvent_OnWinGame()//luc nao can kiem tra vao EnemyBase kiem tra
    {
        if (!DataPlayer.alldata.isNormalMap)
        {
            UIController.Instance.WaitShowEndTutorial();
            ready = false;
            hand.SetActive(false);

            return;
        }
        else
        {
            if (IsCompleteLevel == true) return;//da chet roi thi khong the win
            numberEnemy--;
            HidePartEnemy(numberEnemyDie);
            numberEnemyDie++;
            if (numberEnemy == 0)
            {
                StopAllCoroutines();
                ChangeTimeScale(1f);
                StartCoroutine(SlowmotionKillLastEnemy(0.005f, 0.5f));
                UIController.Instance.HideIngamePopup();
                DataPlayer.SetCountFail(0);//Reset lại số lần chết của player =0,để đếm số lần để ShowInterFail 1 3 5 7
                IsCompleteLevel = true;
                int levelCurrent = DataPlayer.GetLevelValue();
                levelCurrent = levelCurrent + 1;
                if (levelCurrent == 2 || levelCurrent == 3 ||levelCurrent ==11 || levelCurrent ==31)//ket thuc level 2 va 3 se chuyen ve dang maptutorial
                {
                    DataPlayer.SetIsNormalMap(false);
                }
                DataPlayer.SetLevelValue(levelCurrent);
                PlayVfxConfetti();

                int  timePlay = (int)Math.Ceiling(Time.unscaledTime - startTimeLevel);
                
                FireBaseManager.Instant.LogEventWithParameterAsync("gameplay_win", new Hashtable()
                {
                    {
                        "id_screen","GAMEPLAY"
                    },
                    {
                        "id_level",levelCurrent -1
                    },
                    {
                        "level_time",timePlay
                    },
                    {
                        "level_action",countShoot
                    }
                });
            }
        }
    }
  
    private IEnumerator WaitReady()
    {
        yield return null;
        ready = true;
    }
    public void StartWaitReady()
    {
        StartCoroutine(WaitReady());
    }
    public void PlayVfxBlood(Vector3 point)
    {
        if (vfxBlood != null)
        {
            vfxBlood.transform.position = new Vector3(point.x, point.y +0.2f, -2f);
            vfxBlood.gameObject.SetActive(true);
        }
    }    
    public void PlayVfxExplosion(Vector3 point)
    {
        if(vfxExplosion != null)
        {
            vfxExplosion.transform.position = new Vector3(point.x, point.y, 0f);
            vfxExplosion.gameObject.SetActive(true);
        }    
    }    
    public void PlayVfxConfetti()
    {
        float posY = GetHeightVfxWin();
        vfxConfettiLeft.gameObject.transform.position = new Vector3(-1.5f, posY, 0f);
        vfxConfettiRight.gameObject.transform.position = new Vector3(1.5f,posY, 0f);
        vfxConfettiLeft.Play();
        vfxConfettiRight.Play();
    }
    public void SpawnBlood(Transform transform)
    {
        if (vfxBloodPrefab)
        {
            var vfx = Instantiate(vfxBloodPrefab, transform);
            vfx.transform.localPosition = new Vector3(0f, 0.5f, 0f);
        }
    }

    public void SlowmotionKillEnemy(float timeScale,float duration)
    {
        // Bị lỗi player di chuyển chậm ,thử dùng cách này để fix ,chưa test được có hoạt động hay không
        if(slowmotionKillEnemy != null)
        {
            StopCoroutine(slowmotionKillEnemy);
            ChangeTimeScale(1f);//vi neu chua ket thuc coroutine ,timeScale khác 1,khi bị dừng giữa chừng thì TimeScale #1 và sẽ duy trì mãi nên cần phải trả về 1
        }
        slowmotionKillEnemy = StartCoroutine(Slowmotion(timeScale,duration));

    }    
    // Đổi timeScale trong khoảng thời gian duration
    private IEnumerator Slowmotion(float timeScale,float duration)
    {
        ChangeTimeScale(timeScale);
        yield return new WaitForSeconds(duration * timeScale);
        ChangeTimeScale(1f);
    }    

    private IEnumerator SlowmotionKillLastEnemy(float timeScale,float duration)
    {
        conditionZoomOut = true;
        float time = timeScale;
        ChangeTimeScale(timeScale);
        isZoomCamera = true;
        isKillLastEnemy = true;
        uiTextVfx.PlayAnimationRateText();
        uiTextVfx.SetPositionUITextVfx(new Vector3((float)screenWidth/2f, (float)screenHeight * 4f / 5f, 0f), true);

        yield return new WaitForSeconds(duration * timeScale);
        isZoomCamera = false;
        while (time < 1f)
        {
            yield return null;
            time += 10f * Time.deltaTime;
            if (time > 1f) time = 1f;
            ChangeTimeScale(time);
        }
        PlayVfxConfetti();
        yield return new WaitForSeconds(1.5f);
        isKillLastEnemy = false;

        if (DataFireBaseConfig.Instance.CanShowRate())
        {
            Debug.Log("Se show rate");
            DVAH3rdLib.Instant.ShowPopUpRate();//Show rate trước sau ,tắt rate đi mới show inter nếu có thể
        }    
        else
        {
            UIController.Instance.ShowWinPopup();
        }
    }    
    public void ChangeTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
        Time.maximumDeltaTime = timeScale;
        Time.fixedDeltaTime = timeScale * 0.02f;
    }    
    private void SpawnPartEnemy(int quantity)
    {
        for(int i = 0; i < quantity; i++)
        {
            var part = Instantiate(itemEnemyPrefab, progressBar);
            enemyIconList.Add(part);
        }
    }    
    private void HidePartEnemy(int index)
    {
        enemyIconList[index].transform.GetChild(0).gameObject.SetActive(false);
        enemyIconList[index].transform.GetChild(1).gameObject.SetActive(true);
    }    
    private float GetHeightVfxWin()
    {
        Vector2 worldPos = cam.ViewportToWorldPoint(new Vector2(0.5f,0.5f));
        return worldPos.y;
    }    

    public Vector3 GetWorldPointFromScreenPoint(Vector3 screenPoint)
    {
        screenPoint.z = 10f;
        Vector3 worldPos = cam.ScreenToWorldPoint(screenPoint);
        worldPos.z = 0;
        return worldPos;
    }    
    public Vector3 GetScreenPoint(Vector3 worldPoint)
    {
        worldPoint.z = 10;
        Vector3 screenPoint =cam.WorldToScreenPoint(worldPoint);
        return new Vector3(screenPoint.x, screenPoint.y, 0f);
    }

 
    public void InitEmptyObject(Transform tf,Vector3 point)
    {
        emptyObject = Instantiate(emptyPrefab,point,Quaternion.identity);
        emptyObject.SetParent(tf);
    }


   
    public void TapVibrate()
    {
        if (DataPlayer.GetHasVibration())
        {
            Vibration.VibrateAndroid(inputTimeVibrate);
        }
    }

    private void OnApplicationQuit()
    {
        DataPlayer.SetCountFail(0);
    }
    private void OnApplicationFocus(bool focus)//nhấn home thoát ra ngoài
    {
        if (focus)
        {
            Time.timeScale = 1f;
        }
        else
        {
            Time.timeScale = 0f;
        }
    }
}
