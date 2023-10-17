using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadaController : MonoBehaviour
{
    Transform parent;
    EnemyBase enemyBase;

    private void Start()
    {
        parent = transform.parent;
        enemyBase = parent.GetComponent<EnemyBase>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!DataPlayer.alldata.isNormalMap) return;
        if (other.CompareTag("Player")) enemyBase?.ShowNote();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") &&(GameManager.Instance.conditionDetectPlayer || !DataPlayer.alldata.isNormalMap))
        {
            if (enemyBase == null)
            {
                parent = transform.parent;
                enemyBase = parent.GetComponent<EnemyBase>();
            }
            enemyBase.DetectPlayer();
        } 
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) enemyBase.UndetectPlayer();
    }
}
