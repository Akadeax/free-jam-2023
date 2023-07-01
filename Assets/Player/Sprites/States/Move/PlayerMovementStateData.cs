using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SM/Player/Movement")]
public class PlayerMovementStateData : PixelAnimatorStateData
{
    [Header("Animations")]
    public PixelAnimationClip4D idle;
    public PixelAnimationClip4D move;

    public override PixelAnimatorState ConstructState(PixelAnimator animator)
    {
        return new PlayerMovementState(stateName, this, animator);
    }
}

public class PlayerMovementState : PixelAnimatorState
{
    new PlayerMovementStateData data;
    PlayerMovement movement;

    bool isMoving = false;
    Vector2 lastInputDir;

    public PlayerMovementState(string name, PixelAnimatorStateData data, PixelAnimator animator) : base(name, data)
    {
        this.data = data as PlayerMovementStateData;
        movement = animator.GetComponent<PlayerMovement>();
    }

    public override void OnEnter(PixelAnimator animator)
    {
        movement.OnPlayerMoveTick += Movement_OnPlayerMoveTick;
    }


    public override void OnExit(PixelAnimator animator)
    {
        movement.OnPlayerMoveTick -= Movement_OnPlayerMoveTick;
    }

    private void Movement_OnPlayerMoveTick(Vector2 normedInput)
    {
        if (normedInput != Vector2.zero) lastInputDir = normedInput;
        isMoving = normedInput.sqrMagnitude > 0;
    }

    public override void OnUpdate(PixelAnimator animator)
    {
        if (isMoving)
        {
            animator.SetCurrentAnimation(data.move.GetAppropriate(lastInputDir));
        }
        else
        {
            animator.SetCurrentAnimation(data.idle.GetAppropriate(lastInputDir));
        }
    }
}