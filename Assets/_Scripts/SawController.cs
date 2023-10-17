using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag =="Player")
        {
            Player player = other.transform.GetComponent<Player>();
            player.OnDie();
        }
    }
}
