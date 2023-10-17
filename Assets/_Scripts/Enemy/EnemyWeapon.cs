using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyWeapon : MonoBehaviour
{
    protected Transform playerTransform;

    protected virtual void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Player chết
            TriggerEnterPlayer(playerTransform);
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            //Player chết
            TriggerEnterPlayer(playerTransform);
        }
    }

    protected abstract void TriggerEnterPlayer(Transform playerTransform);
}
