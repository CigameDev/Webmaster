using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Hook : MonoBehaviour
{
    Player grapple;
    LineRenderer lineRenderer;
    int percision = 40;//so diem cua day
    float moveTime = 0f;
    float ropeProgressionSpeed = 30f;
    float waveSize = 0f;
    float startWaveSize = 3f;
    public AnimationCurve cure;
    public AnimationCurve ropeProgressionCurve;
    [HideInInspector]public  bool isMove = true;
    Transform hookTransform;
    public enum StateHook
    {
        StraightLine =0,
        MoveTarget,
        NoDraw
    }    
    public StateHook stateHook = StateHook.StraightLine;
    public void Inittialize(Player grapple)
    {
        this.grapple = grapple;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = percision;
        waveSize = startWaveSize;
        SoundFXManager.Instance.PlaySpiderWeb();
    }

    private void Awake()
    {
        hookTransform = this.transform;
    }

    private void Update()
    {
        moveTime += Time.deltaTime;
        if (moveTime >= 1f)
        {
            moveTime -= 1f;
        }
        DrawRope();
    }
  
    //MoveToTarget logic moi la hook se di chuyen den cai vat dau tien no cham phai bao gom enemy,wall/platform,shield,woodenbox
    public void MoveToTarget(Player grapple, Vector3 target)
    {
        if (stateHook != StateHook.MoveTarget) return;
        //transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * ropeProgressionSpeed);
        hookTransform.position = Vector3.MoveTowards(hookTransform.position, target, Time.deltaTime * ropeProgressionSpeed);
        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            if(grapple.firstRaycast == FirstRaycast.Switch)
            {
                hookTransform.position = target;
                int id = grapple.hitSwitch.collider.GetComponent<Switch>().IDSwitch;
                GameEvent.instance.PostEvent_Opengate(id);
                grapple.ResetPositionWeight();
                Destroy(gameObject);
            }   
            else
            {
                grapple.isPullEnemyOrBox = true;
                hookTransform.position = target;
                stateHook = StateHook.StraightLine;
                grapple.StartPull();
                if (grapple.gripDirection == GripDirection.Left || grapple.gripDirection == GripDirection.Right)
                {
                    GameManager.Instance.spiderWeb.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                }
                else if (grapple.gripDirection == GripDirection.Up || grapple.gripDirection == GripDirection.Down)
                {
                    GameManager.Instance.spiderWeb.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                }
                if (grapple.firstRaycast == FirstRaycast.Grapple)
                {
                    GameManager.Instance.spiderWeb.SetActive(true);
                    GameManager.Instance.spiderWeb.GetComponentInChildren<SpiderWeb>().PlayAnimationInto();
                }
                if (grapple.gripDirection == GripDirection.Up)
                {
                    GameManager.Instance.spiderWeb.transform.position = new Vector3(target.x, target.y - 0.05f, 0f);
                }
                else if (grapple.gripDirection == GripDirection.Down)
                {
                    GameManager.Instance.spiderWeb.transform.position = new Vector3(target.x, target.y + 0.05f, 0f);
                }
                else if (grapple.gripDirection == GripDirection.Right)
                {
                    GameManager.Instance.spiderWeb.transform.position = new Vector3(target.x - 0.05f, target.y, 0f);
                }
                else if (grapple.gripDirection == GripDirection.Left)
                {
                    GameManager.Instance.spiderWeb.transform.position = new Vector3(target.x + 0.05f, target.y, 0f);
                }
            }    
        }
    }

  
    private void DrawRopeWaves()
    {
        if (grapple == null || grapple.currentShoot == null) return;
        for (int i = 0; i < percision; i++)
        {
            float delta = (float)i / ((float)percision - 1f);
            Vector2 offset = Vector2.Perpendicular((Vector2)grapple._directionLine).normalized * cure.Evaluate(delta) * waveSize;
            Vector2 targetPosition = Vector2.Lerp(grapple.currentShoot.position, hookTransform.position, delta) + offset;
            Vector2 currentPosition = Vector2.Lerp(grapple.currentShoot.position, targetPosition, ropeProgressionCurve.Evaluate(moveTime) * ropeProgressionSpeed);

            //lineRenderer.SetPosition(i, new Vector3(targetPosition.x,targetPosition.y,transform.position.z));
            lineRenderer.SetPosition(i, new Vector3(currentPosition.x, currentPosition.y, hookTransform.position.z));
        }
    }

   
    private void DrawRopeNoWaves()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, grapple.currentShoot.position);
        lineRenderer.SetPosition(1, hookTransform.position);

    }    
    private void EraseRope()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, grapple.currentShoot.position);
        lineRenderer.SetPosition(1, grapple.currentShoot.position);

    }    
    private void DrawRope()
    {

        if (stateHook == StateHook.MoveTarget)
        {
            DrawRopeWaves();
        }
        else if (stateHook == StateHook.StraightLine)
        {
            DrawRopeNoWaves();
        }
        else
        {
            EraseRope();
        }
    }
    public void SetParentForHook(Transform parent)
    {
        this.gameObject.transform.SetParent(parent);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer ==7 || collision.gameObject.layer ==10)
        {
            Debug.Log("Hook va cham voi enemy hoac wooden box");
        }    
    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    if(other.tag == GameConst.Switch_Tag)
    //    {
    //        Switch nutBam = other.GetComponent<Switch>();
    //        int id = nutBam.IDSwitch;
    //        GameEvent.instance.PostEvent_Opengate(id);
    //    }
    //}
}
