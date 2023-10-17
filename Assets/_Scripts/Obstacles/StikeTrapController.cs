using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StikeTrapController : MonoBehaviour
{
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.collider.CompareTag("Player"))
    //    {
    //        Debug.Log("va chm voi player");
    //        if (collision.gameObject.GetComponent<Player>().conditionDiePlayer)
    //        {
    //            Player player = collision.transform.GetComponent<Player>();
    //            player.OnDie();
    //        }
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<Player>().conditionDiePlayer)
            {
                Player player = other.transform.GetComponent<Player>();
                player.OnDie();
            }
        }
    }
}
