using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SM/Player/Steal")]
public class PlayerStealStateData : PixelAnimatorStateData
{
    [Header("Animations")]
    public PixelAnimationClip stealStart;
    public PixelAnimationClip stealFail;
    public PixelAnimationClip stealSuccess;

    public override PixelAnimatorState ConstructState(PixelAnimator animator)
    {
        return new PlayerStealState(stateName, this, animator);
    }
}

public class PlayerStealState : PixelAnimatorState
{
    new PlayerStealStateData data;
    PixelAnimator animator;

    PlayerSteal steal;

    public PlayerStealState(string name, PixelAnimatorStateData data, PixelAnimator animator) : base(name, data)
    {
        this.data = data as PlayerStealStateData;
        this.animator = animator;

        steal = animator.GetComponent<PlayerSteal>();
    }

    public override void OnEnter(PixelAnimator animator)
    {
        animator.SetCurrentAnimation(data.stealStart);

        steal.OnPlayerMinigameFinish += Steal_OnPlayerMinigameFinish;
    }

    public override void OnExit(PixelAnimator animator)
    {
        steal.OnPlayerMinigameFinish -= Steal_OnPlayerMinigameFinish;
    }

    private void Steal_OnPlayerMinigameFinish(bool success)
    {
        animator.SetCurrentAnimation(success ? data.stealSuccess : data.stealFail, true);
    }
}
