using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[CreateAssetMenu(menuName = "SM/NPC/Patrol")]
public class NPCPatrolStateData : PixelAnimatorStateData
{
    public float idleBetweenPatrolPoints = 5f;
    public float suspiciousPlayerDistance = 3f;

    [Header("Animations")]
    public PixelAnimationClip4D idleAnim;
    public PixelAnimationClip4D walkingAnim;


    public override PixelAnimatorState ConstructState(PixelAnimator animator)
    {
        return new NPCPatrolState(stateName, this, animator);
    }
}

public class NPCPatrolState : PixelAnimatorState
{
    new NPCPatrolStateData data;
    Rigidbody2D rb;
    NPCData npc;

    int currentPatrolPointIndex = 0;

    bool isIdling = false;

    public NPCPatrolState(string name, PixelAnimatorStateData data, PixelAnimator animator) : base(name, data)
    {
        this.data = data as NPCPatrolStateData;

        npc = animator.GetComponent<NPCData>();
        rb = animator.GetComponent<Rigidbody2D>();
    }

    public override void OnEnter(PixelAnimator animator)
    {
        npc.FollowCurrentPath = true;
        isIdling = false;
        npc.CreateNewPath(npc.PatrolPoints[currentPatrolPointIndex].position);

        npc.IsBusy = false;
    }

    public override void OnExit(PixelAnimator animator)
    {
        npc.SuspiciousBarAmount = 0f;
        npc.IsBusy = true;
    }

    public override void OnUpdate(PixelAnimator animator)
    {
        float distToPatrolTarget = Vector2.Distance(npc.PatrolPoints[currentPatrolPointIndex].position, rb.transform.position);

        if (distToPatrolTarget < npc.TargetDistanceLeniency && npc.CurrentPathPointIsLast && !isIdling)
        {
            animator.StartCoroutine(ChooseNextPatrolPath());
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

        // Guards don't get suspicious of player standing around them
        if (npc.Type == NPCData.NPCType.Guard) return;

        // Check distance to player
        float playerDistance = Vector2.Distance(rb.transform.position, npc.Player.transform.position);
        npc.ChangeSuspiciousBar(playerDistance < data.suspiciousPlayerDistance);
        
        if (npc.SuspiciousBarAmount > 0f)
        {
            npc.SetHeadDisplay(NPCData.HeadDisplay.eye);
        }
        else
        {
            npc.SetHeadDisplay(NPCData.HeadDisplay.none);
        }

        if (npc.SuspiciousBarAmount >= 1f)
        {
            animator.SwitchState("SeekGuard");
        }
    }

    public IEnumerator ChooseNextPatrolPath()
    {
        rb.velocity = Vector2.zero;

        isIdling = true;
        npc.FollowCurrentPath = false;

        yield return new WaitForSeconds(data.idleBetweenPatrolPoints);

        currentPatrolPointIndex = (currentPatrolPointIndex + 1) % npc.PatrolPoints.Count;
        npc.CreateNewPath(npc.PatrolPoints[currentPatrolPointIndex].position);

        isIdling = false;
        npc.FollowCurrentPath = true;
    }
}
