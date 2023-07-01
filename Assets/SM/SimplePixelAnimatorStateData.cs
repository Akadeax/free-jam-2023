using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SM/Simple State")]
public class SimplePixelAnimatorStateData : PixelAnimatorStateData
{
    [Header("Animations")]
    public PixelAnimationClip clip;

    public override PixelAnimatorState ConstructState(PixelAnimator animator)
    {
        return new SimplePixelAnimatorState(stateName, this);
    }
}

public class SimplePixelAnimatorState : PixelAnimatorState
{
    new SimplePixelAnimatorStateData data;

    public SimplePixelAnimatorState(string name, PixelAnimatorStateData data) : base(name, data)
    {
        this.data = data as SimplePixelAnimatorStateData;
    }

    public override void OnEnter(PixelAnimator animator)
    {
        animator.SetCurrentAnimation(data.clip);
    }
}