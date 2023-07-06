using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SM/NPC/ChasePlayer")]
public class NPCChasePlayerStateData : PixelAnimatorStateData
{
    public float lastSeenPosReachedWaitTime = 2f;
    public float waitHeadTurnInterval = 0.5f;

    public float playerSpotRange = 8f;

    public float playerCatchRange = 1f;
    public LayerMask obstacleLayerMask;

    public float playerChaseSpeed = 3.5f;

    [Header("Animations")]
    public PixelAnimationClip4D idleAnim;
    public PixelAnimationClip4D walkingAnim;

    public override PixelAnimatorState ConstructState(PixelAnimator animator)
    {
        return new NPCChasePlayerState(stateName, this, animator);
    }
}

public class NPCChasePlayerState : PixelAnimatorState
{
    new NPCChasePlayerStateData data;
    PixelAnimator animator;
    Rigidbody2D rb;
    NPCData npc;

    Vector2 lastPlayerBreadcrumb;

    public NPCChasePlayerState(string name, PixelAnimatorStateData data, PixelAnimator animator) : base(name, data)
    {
        this.data = data as NPCChasePlayerStateData;
        this.animator = animator;

        rb = animator.GetComponent<Rigidbody2D>();
        npc = animator.GetComponent<NPCData>();
    }

    public override void OnEnter(PixelAnimator animator)
    {
        npc.SetHeadDisplay(NPCData.HeadDisplay.alert);

        npc.CreateNewPath(npc.Player.transform.position);
        npc.FollowCurrentPath = true;

        npc.CurrentMoveSpeed = data.playerChaseSpeed;
    }

    public override void OnExit(PixelAnimator animator)
    {
        npc.CurrentMoveSpeed = npc.Speed;
    }

    public override void OnUpdate(PixelAnimator animator)
    {
        float distanceToPlayer = Vector2.Distance(rb.transform.position, npc.Player.transform.position);

        RaycastHit2D hit = Physics2D.Linecast(rb.transform.position, npc.Player.transform.position, data.obstacleLayerMask);

        if (distanceToPlayer < data.playerSpotRange && hit.collider == null)
        {
            lastPlayerBreadcrumb = npc.Player.transform.position;
            npc.CreateNewPath(lastPlayerBreadcrumb);
        }

        Debug.DrawLine(rb.transform.position, lastPlayerBreadcrumb);

        float distanceToBreadcrumb = Vector2.Distance(rb.transform.position, lastPlayerBreadcrumb);
        float distanceToLastPathPoint = Vector2.Distance(rb.transform.position, npc.CurrentPathTarget);
        if (distanceToPlayer < data.playerCatchRange)
        {
            npc.Player.SetDead();
        }
        else if (distanceToBreadcrumb < npc.TargetDistanceLeniency || distanceToLastPathPoint < npc.TargetDistanceLeniency)
        {
            animator.StartCoroutine(LastSeenPlayerPosReached());
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

            float distanceToBreadcrumb = Vector2.Distance(rb.transform.position, lastPlayerBreadcrumb);
            if (distanceToBreadcrumb > npc.TargetDistanceLeniency)
            {
                animator.SwitchState("ChasePlayer");
            }
        }

        npc.SetHeadDisplay(NPCData.HeadDisplay.none);

        animator.SwitchState("Patrol");
    }
}
