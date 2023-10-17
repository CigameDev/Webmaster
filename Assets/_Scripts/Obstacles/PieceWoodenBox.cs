using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PieceWoodenBox : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.collider.CompareTag("Enemy"))
        {
            GetComponentInParent<WoodenBoxController>().ChangeChildLayer();
        }
    }
}
