using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RootMotion.FinalIK.RagdollUtility;

public class SteelBallController : MonoBehaviour
{
    bool isBroke;

    private void OnTriggerEnter(Collider other)
    {
        if (isBroke) return;
        if (other.CompareTag("Player"))
        {
            isBroke = true;
            Destroy(GetComponent<HingeJoint>());
            this.gameObject.layer = 8;//chuyen ve layer enemy de khong va cham voi Player nua
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (isBroke) return;
        if (col.collider.CompareTag("Player"))
        {
            isBroke = true;
            Destroy(GetComponent<HingeJoint>());
            this.gameObject.layer = 8;//chuyen ve layer enemy de khong va cham voi Player nua
        }
    }
}
