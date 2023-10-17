using DVAH;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodenBoxController : MonoBehaviour
{
    public Player player { get; set; }
    bool prepareBroke;
    Rigidbody rb;
    Collider colliderBox;
    float pullSpeedWoodenBox = 10f;
    Transform[] childs;
    Vector3 directionPush;
    [SerializeField] float forcePull;


    private bool kicked = false;
    private void Start()
    {
        colliderBox = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        int totalChild = transform.childCount;
        childs = new Transform[totalChild];
        for(int i = 0; i < totalChild; i++)
        {
            childs[i] = transform.GetChild(i);
        }
        


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();


            player.StopCoroutineEndPull();

            player.StartCoroutineKinematic();

            SoundFXManager.Instance.PlayWoodenBox();
            GameManager.Instance.uiTextVfx.PlayAnimationbrokenWoodenBox();
            GameManager.Instance.uiTextVfx.SetPositionUITextVfx(transform.position, false);

            player.StartCoroutineChangeVelocityPlayer();

            rb.AddForce(player._directionLine * forcePull);
            prepareBroke = true;
            player.woodenBoxController = null;
            directionPush = player._directionLine;
            this.gameObject.layer = LayerMask.NameToLayer(GameConst.PieceWoodenBox_Layer);//doi layer de khong tinh va cham voi Player nua

            StartCoroutine(IEAutoDestroy());

        }
        else
        {
            if (prepareBroke)
            {
                kicked = true;
                Destroy(rb);
                Destroy(colliderBox);
                PullAllChilds();
            }
        }
    }

    private IEnumerator IEAutoDestroy()
    {
        yield return new WaitForSeconds(0.2f);
        if(this.gameObject == null || kicked)
        {
            yield break;
        }

        Destroy(rb);
        Destroy(colliderBox);
        PullAllChilds();

    }    
    public void KickWoodenBox()
    {
        Vector3 posPlayer = player.transform.position;
        float distance = Vector3.Distance(posPlayer,this.transform.position);
        float delayTime = distance / (player.PullSpeedPlayer +pullSpeedWoodenBox) +0.01f;
        StartCoroutine(IEExceptionAddforceWoodenBox(delayTime));
    }    
    private IEnumerator IEExceptionAddforceWoodenBox(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        if(prepareBroke)
        {
            yield break;
        }
        if (player)
        {
            player.StartCoroutineChangeVelocityPlayer();
            rb.AddForce(player._directionLine * forcePull);
            prepareBroke = true;
            player.woodenBoxController = null;
            directionPush = player._directionLine;
            this.gameObject.layer = LayerMask.NameToLayer(GameConst.PieceWoodenBox_Layer);//doi layer de khong tinh va cham voi Player nua
        }
    }    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();

            player.StopCoroutineEndPull();
           
            Vector2 vec1 = (Vector2)player._directionLine;
            Vector2 vec2 = (Vector2)(this.transform.position - player.playerStartPoint);
            float rad = Vector2.Angle(vec1, vec2);
            if(rad >=-30f && rad <= 30f)
            {

                SoundFXManager.Instance.PlayWoodenBox();
                GameManager.Instance.uiTextVfx.PlayAnimationbrokenWoodenBox();
                GameManager.Instance.uiTextVfx.SetPositionUITextVfx(transform.position, false);
                player.StartCoroutineChangeVelocityPlayer();

                rb.AddForce(player._directionLine * forcePull);
                prepareBroke = true;
                player.woodenBoxController = null;
                directionPush = player._directionLine;
                this.gameObject.layer = LayerMask.NameToLayer(GameConst.PieceWoodenBox_Layer);//doi layer de khong tinh va cham voi Player nua

                StartCoroutine(IEAutoDestroy());
            }

            //SoundFXManager.Instance.PlayWoodenBox();
            //GameManager.Instance.uiTextVfx.PlayAnimationbrokenWoodenBox();
            //GameManager.Instance.uiTextVfx.SetPositionUITextVfx(transform.position,false);
            //player.IEChangeVelocity();

            //rb.AddForce(player._directionLine *forcePull);
            //prepareBroke = true;
            //player.woodenBoxController = null;
            //directionPush = player._directionLine;
            //this.gameObject.layer = LayerMask.NameToLayer(GameConst.PieceWoodenBox_Layer);//doi layer de khong tinh va cham voi Player nua

            //StartCoroutine(IEAutoDestroy());

        }
        else
        {
            if (prepareBroke)
            {
                kicked = true;

                Destroy(rb);
                Destroy(colliderBox);
                PullAllChilds();
            }
        }
    }

    void Pull(Transform obj)
    {
        Vector3 playerPos = obj.position;
        playerPos.y += 0.5f;

        Vector3 direction = transform.position - playerPos;
        rb.AddForce(direction.normalized * forcePull);

        prepareBroke = true;
    }

    void PullAllChilds()
    {
        SoundFXManager.Instance.PlayWoodenBoxBroken();
        foreach(Transform child in childs)
        {
            //Vector3 direction = new(Random.value, Random.value, Random.value);
            Vector3 direction = directionPush;

            Collider colliderChild = child.GetComponent<Collider>();
            colliderChild.isTrigger = false;

            Rigidbody rigidbody = child.GetComponent<Rigidbody>();
            rigidbody.isKinematic = false;

            rigidbody.AddForce(direction * forcePull);
        }

        Invoke(nameof(AutoDestroy), 2f);
    }
    public void ChangeChildLayer()
    {
        foreach(Transform child in childs)
        {
            child.gameObject.layer = 8;//layerEnemyRagdoll
        }    
    }    
    void AutoDestroy()
    {
        Destroy(gameObject);
    }
    public void PullWoodenBox(Vector3 direction)
    {
        if (rb)
        {
            rb.useGravity = false;
            rb.velocity = direction * pullSpeedWoodenBox;
        }
         //rb.AddForce(direction * pullSpeedWoodenBox);
    }    

    //thực hiện logic mới nếu chạm vào Player thì 0.1s sau sẽ vỡ
}
