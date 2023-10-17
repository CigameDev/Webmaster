using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private bool called;
    private void OnCollisionEnter(Collision collision)
    {
        if (called) return;
        if(collision.collider.tag== GameConst.Platform_Tag ||collision.collider.tag == GameConst.Enemy_Tag || collision.collider.tag == "Ground" )
        {
            called = true;
            GameManager.Instance.uiTextVfx.SetPositionUITextVfx(this.transform.position, false);
            GameManager.Instance.uiTextVfx.PlayAnimationFallStellBall();
            SoundFXManager.Instance.PlayBall();
        }    
    }
}
