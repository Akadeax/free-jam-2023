using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SM/Anim Clip")]
public class PixelAnimationClip : ScriptableObject
{
    public float duration = 1f;
    public List<Sprite> sprites = new();
    public bool isLooping = true;

    public bool IsEmpty
    {
        get
        {
            return sprites.Count == 0;
        }
    }
}
