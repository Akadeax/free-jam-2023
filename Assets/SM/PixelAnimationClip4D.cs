using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SM/4D Anim Clip")]
public class PixelAnimationClip4D : ScriptableObject
{
    public PixelAnimationClip up;
    public PixelAnimationClip right;
    public PixelAnimationClip down;
    public PixelAnimationClip left;

    public PixelAnimationClip GetAppropriate(Vector2 direction, float upDownPointMultiplier = 0.75f)
    {
        // TODO: fix this, aka, goddamnit
        float distanceUp = Vector2.Distance(direction, new Vector2(0, upDownPointMultiplier));
        float distanceRight = Vector2.Distance(direction, new Vector2(1, 0));
        float distanceDown = Vector2.Distance(direction, new Vector2(0, -upDownPointMultiplier));
        float distanceLeft = Vector2.Distance(direction, new Vector2(-1, 0));

        float minDistance = Mathf.Min(distanceUp, distanceRight, distanceDown, distanceLeft);

        if (minDistance == distanceDown) return down;
        else if (minDistance == distanceUp) return up;
        else if (minDistance == distanceRight) return right;
        else return left;
    }
}
