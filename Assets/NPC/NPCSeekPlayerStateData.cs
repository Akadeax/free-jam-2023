using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SM/NPC/SeekPlayer")]
public class NPCSeekPlayerStateData : PixelAnimatorStateData
{
    public float guardTargetDistanceLeniency = 1.5f;

    public float lastSeenPosReachedWaitTime = 2f;
    public float waitHeadTurnInterval = 0.5f;

    public float playerSpotRange = 4f;
    public LayerMask obstacleLayerMask;

    [Header("Animations")]
    public PixelAnimationClip4D idleAnim;
    public PixelAnimationClip4D walkingAnim;

    public override PixelAnimatorState ConstructState(PixelAnimator animator)
    {
        return new NPCSeekPlayerState(stateName, this, animator);
    }
}

public class NPCSeekPlayerState : PixelAnimatorState
{
    new NPCSeekPlayerStateData data;
    PixelAnimator animator;
    Rigidbody2D rb;
    NPCData npc;

    Vector2 lastSeenPlayerPos;
    bool reachedLastSeenPlayerPos = false;

    public NPCSeekPlayerState(string name, PixelAnimatorStateData data, PixelAnimator animator) : base(name, data)
    {
        this.data = data as NPCSeekPlayerStateData;
        this.animator = animator;

        rb = animator.GetComponent<Rigidbody2D>();
        npc = animator.GetComponent<NPCData>();
    }

    public override void OnEnter(PixelAnimator animator)
    {
        npc.FollowCurrentPath = true;
        lastSeenPlayerPos = npc.CurrentPathTarget;
        reachedLastSeenPlayerPos = false;
    }

    public override void OnExit(PixelAnimator animator)
    {
        npc.IsBusy = false;
    }

    public override void OnUpdate(PixelAnimator animator)
    {
        float lastSeenPosDistance = Vector2.Distance(rb.transform.position, lastSeenPlayerPos);

        float leniency = npc.Type == NPCData.NPCType.Normal ? npc.TargetDistanceLeniency : data.guardTargetDistanceLeniency;

        if (lastSeenPosDistance < leniency && !reachedLastSeenPlayerPos)
        {
            reachedLastSeenPlayerPos = true;

            if (npc.Type == NPCData.NPCType.Normal)
            {
                animator.SwitchState("Patrol");
            }
            else
            {
                animator.StartCoroutine(LastSeenPlayerPosReached());
            }
        }


        // Set animation
        bool isMoving = rb.velocity.magnitude != 0;

        if (isMoving)
        {
            animator.SetCurrentAnimation(data.walkingAnim, rb.velocity);
        }
        else
        {
            animator.SetCurrentAnimation(data.idleAnim, npc.FacingDir);
        }


        if (npc.Type != NPCData.NPCType.Guard) return;

        // Look out for player
        float distanceToPlayer = Vector2.Distance(rb.transform.position, npc.Player.transform.position);

        RaycastHit2D hit = Physics2D.Linecast(rb.transform.position, npc.Player.transform.position, data.obstacleLayerMask);

        if (distanceToPlayer < data.playerSpotRange && hit.collider == null)
        {
            animator.SwitchState("ChasePlayer");
        }
    }

    IEnumerator LastSeenPlayerPosReached()
    {
        rb.velocity = Vector2.zero;
        npc.FollowCurrentPath = false;
        npc.SetHeadDisplay(NPCData.HeadDisplay.question);

        int headTurns = (int)(data.lastSeenPosReachedWaitTime / data.waitHeadTurnInterval);
        Vector2 facingDir = new(1, 0);
        for (int i = 0; i < headTurns; i++)
        {
            npc.SetFacingDir(facingDir);
            yield return new WaitForSeconds(data.waitHeadTurnInterval);
            facingDir *= -1;
        }

        npc.SetHeadDisplay(NPCData.HeadDisplay.none);

        animator.SwitchState("Patrol");
    }
}
