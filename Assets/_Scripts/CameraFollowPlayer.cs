using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour

{
    private Camera mainCamera;
    [SerializeField] private float startYFollow =-0.5f;
    [SerializeField] private float endYFollow;
    [SerializeField] private float speed;
    [SerializeField] private Transform rolfTop;

    private void Start()
    {
        mainCamera = Camera.main;
        rolfTop = GameObject.FindGameObjectWithTag("Top").transform;
        endYFollow = rolfTop.transform.position.y -4f;
    }
    private void LateUpdate()
    {
        if (!GameManager.Instance.isZoomCamera && !GameManager.Instance.isKillLastEnemy)
        {
            if (this.transform.position.y >= startYFollow && this.transform.position.y <= endYFollow)
            {
                Vector3 direction = transform.position - mainCamera.transform.position;
                direction.z = 0f;
                direction.x = 0f;
                mainCamera.transform.position += speed * Time.unscaledDeltaTime * direction;
            }
        }
        else
        {
            Vector3 playerPos =transform.position;
            playerPos.z -= 5f;
            Vector3 direction = (playerPos - mainCamera.transform.position);
            //direction.z = 0f;
            mainCamera.transform.position += speed  * Time.unscaledDeltaTime * direction;
        }

    }
}
