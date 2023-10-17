using System.Collections;
using System.Collections.Generic; 
using UnityEngine;

public class AnimOffset : MonoBehaviour
{
    float scrollSpeed = 0.5f;
    Renderer rend;

    void Start()
    {
        rend = GetComponent<LineRenderer>();
    }

    void Update()
    {
        float offset = Time.time * scrollSpeed;
        rend.material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
    }

}
