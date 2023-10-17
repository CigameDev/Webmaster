using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "VideoConfig", menuName = "ScriptableObjects/VideoConfig")]
public class VideoConfig : ScriptableObject
{
    public List<VideoClip> videoClips;

    public VideoClip GetVideoClip(int index)
    {
        return videoClips[index];
    }    
}
