using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Laser : MonoBehaviour
{
    private bool activeLaser = true;
    private LineRenderer lineRenderer;
    private float laserOffDuration = 3f;
    private float laserOnDuration =3f;
    float scrollSpeed = 0.5f;
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        Initlaser();
        ActiveLaser();
        StartCoroutine(LaserRoutine());
    }

    private void Update()
    {
        if (!activeLaser) return;
        float offset = Time.time * scrollSpeed;
        lineRenderer.material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
    }
    private void Initlaser()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, endPoint.position);
    }
    private void ActiveLaser()
    {
        lineRenderer.enabled = true;
        activeLaser = true;
    }   
    
    private void DeactiveLaser()
    {
        lineRenderer.enabled = false;
        activeLaser = false;
    }    

    private IEnumerator LaserRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(laserOnDuration);
            DeactiveLaser();
            yield return new WaitForSeconds(laserOffDuration);
            ActiveLaser() ;

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag =="Player" && activeLaser)
        {
            Player player = other.transform.GetComponent<Player>();
            player.OnDie();
            SoundFXManager.Instance.PlayPlayerDie();
        }    
    }
}
