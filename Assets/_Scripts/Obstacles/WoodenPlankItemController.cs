using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodenPlankItemController : MonoBehaviour
{
    Transform parent;
    bool isBroke;

    Transform[] childs;

    [SerializeField] float forceBroke;

    private void Start()
    {
        parent = transform.parent;
        int totalChild = parent.childCount;
        childs = new Transform[totalChild];

        for (int i = 0; i < totalChild; i++)
        {
            childs[i] = parent.GetChild(i);
        }
    }

    void Broke(Transform player)
    {
        foreach(Transform t in childs)
        {
            Rigidbody rb = t.GetComponent<Rigidbody>();
            rb.isKinematic = false;

            Vector3 direction = t.position - player.position;
            rb.AddForce(direction.normalized * forceBroke);
            rb.AddTorque(-1f * forceBroke * direction.normalized);
        }

        isBroke = true;
        Invoke(nameof(AutoDestroy), 3f);
    }

    void AutoDestroy()
    {
        Destroy(parent.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isBroke) return;
        if (collision.collider.CompareTag("Player"))
        {
            Broke(collision.transform);
        }
        else if(collision.gameObject.layer ==15)
        {
            Broke(collision.transform);
        }    
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isBroke) return;
        if (other.CompareTag("Player"))
        {
            Broke(other.transform);
            ChangeLayerByEnemyRagdoll();
            SoundFXManager.Instance.PlayWoodenBox();
        }
    }
    private void ChangeLayerByEnemyRagdoll()
    {
        if(childs == null || childs.Length ==0) return;
        foreach(Transform t in childs)
        {
            t.gameObject.layer = 8;//layerEnemyRagdoll
        }    
    }    
}
